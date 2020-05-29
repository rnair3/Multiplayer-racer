using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    public float brakeSensitivity;
    Drive ds;
    public float steeringSensitivity = 0.01f;
    public float accSensitivity = 0.3f;
    Vector3 target;
    Vector3 nextTarget;
    int currentWP = 0;
    float totalDistance;

    GameObject tracker;
    int currentTrackerWP = 0;
    float lookAhead = 10f;

    float lastTimeMoving = 0;

    CheckpointManager cpm;


    // Start is called before the first frame update
    void Start()
    {
        ds = GetComponent<Drive>();
        target = circuit.waypoints[currentWP].transform.position;
        nextTarget = circuit.waypoints[currentWP + 1].transform.position;
        totalDistance = Vector3.Distance(target, ds.rb.gameObject.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.transform.position = ds.rb.gameObject.transform.position;
        tracker.transform.rotation = ds.rb.gameObject.transform.rotation;
        tracker.GetComponent<MeshRenderer>().enabled = false;

        GetComponent<Ghost>().enabled = false;
    }

    public void ProgressTracker()
    {
        Debug.DrawLine(ds.rb.gameObject.transform.position, tracker.transform.position);
        if(Vector3.Distance(ds.rb.gameObject.transform.position,tracker.transform.position) > lookAhead)  return;
        
        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, 1f);//speed

        if(Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Length)
            {
                currentTrackerWP = 0;
            }
            
        }
    }

    public void ResetLayer()
    {
        ds.rb.gameObject.layer = 0;
        GetComponent<Ghost>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
            return;
        }
        ProgressTracker();
        Vector3 localTarget;
        float targetAngle;
        
        if(ds.rb.velocity.magnitude > 1)
        {
            lastTimeMoving = Time.time;
        }

        if(Time.time > lastTimeMoving + 4)
        {
            if(cpm == null)
            {
                cpm = ds.rb.GetComponent<CheckpointManager>();
            }
            ds.rb.gameObject.transform.position = cpm.lastCP.transform.position + Vector3.up * 2;
            ds.rb.gameObject.transform.rotation = cpm.lastCP.transform.rotation;
            //    circuit.waypoints[currentTrackerWP].transform.position + Vector3.up * 2 + new Vector3(Random.Range(-1,1), 0, Random.Range(-1, 1));
            tracker.transform.position = cpm.lastCP.transform.position;
                //circuit.waypoints[currentTrackerWP].transform.position;
            ds.rb.gameObject.layer = 9;
            GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if(Time.time < ds.rb.GetComponent<AvoidCars>().avoidTime)
        {
            localTarget = tracker.transform.right * ds.rb.GetComponent<AvoidCars>().avoidPath;  
        }
        else
        {
            localTarget = ds.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }

        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ds.currentSpeed);

        //float distFactor = distToTarget / totalDistance;
        float speedFactor = ds.currentSpeed / ds.maxSpeed;
        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90;

        float brake = 0;

        if(corner > 10 && speedFactor > 0.1f)
        {
            brake = Mathf.Lerp(0, 1 + speedFactor * brakeSensitivity, cornerFactor);
        }
        float acc = 1f;
        if (corner > 20 && speedFactor > 0.2f)
        {
            acc = Mathf.Lerp(0, 1 * accSensitivity, 1 - cornerFactor);
        }


        ds.Go(acc, steer, brake);
        ds.CalculateEngineSound();
        ds.CheckSkidding();
    }
}
