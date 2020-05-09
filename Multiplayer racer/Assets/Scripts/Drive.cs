using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider wc;
    public float torque = 200f;
    public float maxSteerAngle = 30f;
    public GameObject wheel;

    // Start is called before the first frame update
    void Start()
    {
        wc = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        Go(a, steer);
    }

    public void Go(float acc, float s)
    {
        acc = Mathf.Clamp(acc, -1, 1);
        s = Mathf.Clamp(s, -1, 1) * maxSteerAngle;

        float thrustTorque = acc * torque;
        wc.motorTorque = thrustTorque;
        wc.steerAngle = s;

        Quaternion q;
        Vector3 pos;
        wc.GetWorldPose(out pos,out q);

        wheel.transform.position = pos;
        wheel.transform.rotation = q;

    }
}
