using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive ds;
    float lastTimeMoving = 0;
    Vector3 lastPos;
    Quaternion lastRot;

    CheckpointManager cpm;
    float finishSteer;

    // Start is called before the first frame update
    void Start()
    {
        ds = GetComponent<Drive>();
        GetComponent<Ghost>().enabled = false;
        lastPos = ds.rb.gameObject.transform.position;
        lastRot = ds.rb.gameObject.transform.rotation;
        finishSteer = Random.Range(-1f, 1f);
    }

    public void ResetLayer()
    {
        ds.rb.gameObject.layer = 0;
        GetComponent<Ghost>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cpm == null)
        {
            cpm = ds.rb.GetComponent<CheckpointManager>();
        }

        if (cpm.lap == RaceMonitor.totalLaps + 1)
        {
            ds.acceleration.Stop();
            ds.Go(0, finishSteer, 0);
            return;
        }

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

        if(Time.time > lastTimeMoving + 4 || ds.rb.gameObject.transform.position.y < -5)
        {
            
            ds.rb.gameObject.transform.position = cpm.lastCP.transform.position + Vector3.up * 2; ;
            ds.rb.gameObject.transform.rotation = cpm.lastCP.transform.rotation;
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
