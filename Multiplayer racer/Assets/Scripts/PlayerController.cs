using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive ds;
    float lastTimeMoving = 0;
    Vector3 lastPos;
    Quaternion lastRot;

    // Start is called before the first frame update
    void Start()
    {
        ds = GetComponent<Drive>();
        GetComponent<Ghost>().enabled = false;
    }

    public void ResetLayer()
    {
        ds.rb.gameObject.layer = 0;
        GetComponent<Ghost>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");

        if (ds.rb.velocity.magnitude > 1 || !RaceMonitor.racing)
            lastTimeMoving = Time.time;

        RaycastHit hit;
        if(Physics.Raycast(ds.rb.gameObject.transform.position, -Vector3.up, out hit, 10))
        {
            if (hit.collider.gameObject.tag == "Road")
            {
                lastPos = ds.rb.gameObject.transform.position;
                lastRot = ds.rb.gameObject.transform.rotation;
            }

        }

        if(Time.time > lastTimeMoving + 4)
        {
            ds.rb.gameObject.transform.position = lastPos;
            ds.rb.gameObject.transform.rotation = lastRot;
            ds.rb.gameObject.layer = 9;
            GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if (!RaceMonitor.racing) a = 0;

        ds.Go(a, steer, brake);

        ds.CheckSkidding();
        ds.CalculateEngineSound();
    }
}
