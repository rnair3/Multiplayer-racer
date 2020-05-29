using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public int lap = 0;
    public int checkpoint = -1;
    int checkpointCount;
    int nextCheckpoint;
    public GameObject lastCP;
    public float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] cps = GameObject.FindGameObjectsWithTag("CheckPoint");
        checkpointCount = cps.Length;

        foreach(GameObject c in cps)
        {
            if(c.name == "0")
            {
                lastCP = c;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CheckPoint")
        {
            
            int thisCPNumber = int.Parse(other.gameObject.name);
            if(thisCPNumber == nextCheckpoint)
            {
                lastCP = other.gameObject;
                checkpoint = thisCPNumber;
                time = Time.time;
                if (checkpoint == 0) lap++;

                nextCheckpoint++;
                if (nextCheckpoint >= checkpointCount)
                {
                    nextCheckpoint = 0;
                }
            }
        }
    }
}
