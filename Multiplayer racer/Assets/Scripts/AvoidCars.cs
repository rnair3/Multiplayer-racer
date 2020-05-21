using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCars : MonoBehaviour
{
    public float avoidPath;
    public float avoidTime;
    public float wanderDist = 4;        //avoid dist
    public float avoidLength = 1f;      //1sec

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Car") return;
        avoidTime = 0;
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "Car") return;

        Rigidbody rbOtherCar = collision.rigidbody;
        avoidTime = Time.time + avoidLength;

        Vector3 otherCarTarget = transform.InverseTransformPoint(rbOtherCar.gameObject.transform.position);
        float angleOtherCar = Mathf.Atan2(otherCarTarget.x, otherCarTarget.z);
        avoidPath = wanderDist * -Mathf.Sign(angleOtherCar);
    }
}
