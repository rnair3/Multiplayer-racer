using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRoll : MonoBehaviour
{
    public float antiRoll = 5000;
    public WheelCollider leftFront;
    public WheelCollider rightFront;
    public WheelCollider leftRear;
    public WheelCollider rightRear;

    Rigidbody rb;
    public GameObject centreOfMass;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreOfMass.transform.localPosition;
    }

    public void GroundWheels(WheelCollider left, WheelCollider right)
    {
        WheelHit hit;
        float travelL = 1f;
        float travelR = 1f;

        bool groundedLeft = left.GetGroundHit(out hit);
        if (groundedLeft)
        {
            travelL = (-left.transform.InverseTransformPoint(hit.point).y - left.radius) / left.suspensionDistance;
        }


        bool groundedRight = right.GetGroundHit(out hit);
        if (groundedRight)
        {
            travelR = (-right.transform.InverseTransformPoint(hit.point).y - right.radius) / right.suspensionDistance;
        }

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedLeft)
        {
            rb.AddForceAtPosition(left.transform.up * -antiRollForce, left.transform.position);
        }

        if (groundedRight)
        {
            rb.AddForceAtPosition(right.transform.up * antiRollForce, right.transform.position);
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        GroundWheels(leftFront, rightFront);
        GroundWheels(leftRear, rightRear);
    }
}
