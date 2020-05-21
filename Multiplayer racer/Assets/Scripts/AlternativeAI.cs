using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeAI : MonoBehaviour
{
    public Circuit circuit;
    Vector3 target;
    int currentWP = 0;
    float accuracy = 4f;
    float speed = 20f;
    float rotSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        target = circuit.waypoints[currentWP].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distToTarget = Vector3.Distance(target, transform.position);
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotSpeed);

        transform.Translate(0, 0, speed * Time.deltaTime);
        if (distToTarget < accuracy)
        {
            currentWP++;
            if (currentWP >= circuit.waypoints.Length)
            {
                currentWP = 0;
            }
            target = circuit.waypoints[currentWP].transform.position;
        }
    }
}
