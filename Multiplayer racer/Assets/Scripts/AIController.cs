using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive ds;
    public float steeringSensitivity = 0.01f;
    Vector3 target;
    int currentWP = 0;


    // Start is called before the first frame update
    void Start()
    {
        ds = GetComponent<Drive>();
        target = circuit.waypoints[currentWP].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localTarget = ds.rb.gameObject.transform.InverseTransformPoint(target);

        float distToTarget = Vector3.Distance(target, ds.rb.gameObject.transform.position);
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ds.currentSpeed);
        float acc = 1f;
        float brake = 0;

        if (distToTarget < 5)
        {
            brake = 0.7f;
            acc = 0.1f;
        }

        ds.Go(acc, steer, brake);

        if (distToTarget < 2)       //threshold if car circles
        {
            currentWP++;
            if (currentWP >= circuit.waypoints.Length)
            {
                currentWP = 0;
            }
            target = circuit.waypoints[currentWP].transform.position;
        }

        ds.CalculateEngineSound();
        ds.CheckSkidding();
    }
}
