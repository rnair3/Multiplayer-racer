using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive ds;

    // Start is called before the first frame update
    void Start()
    {
        ds = GetComponent<Drive>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");

        ds.Go(a, steer, brake);

        ds.CheckSkidding();
        ds.CalculateEngineSound();
    }
}
