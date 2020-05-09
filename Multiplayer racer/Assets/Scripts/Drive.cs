using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider[] wc;
    public float torque = 200f;
    public float maxSteerAngle = 30f;
    public GameObject[] wheel;
    public float maxBrakeTorque = 500f;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");

        Go(a, steer, brake);

        
    }

    public void Go(float acc, float s, float b)
    {
        acc = Mathf.Clamp(acc, -1, 1);
        s = Mathf.Clamp(s, -1, 1) * maxSteerAngle;
        b = Mathf.Clamp(b, 0, 1) * maxBrakeTorque;

        float thrustTorque = acc * torque;


        for (int i = 0; i < wc.Length; i++)
        {
            wc[i].motorTorque = thrustTorque;
            
            if (i < 2)
            {
                wc[i].steerAngle = s;
            }
            else
            {
                wc[i].brakeTorque = b;
            }

            Quaternion q;
            Vector3 pos;

            wc[i].GetWorldPose(out pos, out q);

            wheel[i].transform.position = pos;
            wheel[i].transform.rotation = q;
        }

        

        

    }
}
