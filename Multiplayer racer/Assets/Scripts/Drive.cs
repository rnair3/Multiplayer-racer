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

    public AudioSource skidSound;
    public Transform skidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public ParticleSystem smoke;
    ParticleSystem[] skidParticle = new ParticleSystem[4];

    public GameObject[] brakeLight;

    public AudioSource acceleration;
    public Rigidbody rb;
    public float gearLength = 3f;
    public float currentSpeed { get { return rb.velocity.magnitude * gearLength; } }
    public float lowPitch = 1f;
    public float highPitch = 6f;
    public int numGears = 5;
    float rpm;
    int currentGear = 1;
    float currentGearPerc;
    public float maxSpeed = 200f;  //km/hr

    public GameObject playerNamePrefab;
    public Renderer carMesh;
    public string netName = "";
    string[] names = { "Adrian", "Lee", "Ryan", "Tom", "Kate", "Raj" };

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            skidParticle[i] = Instantiate(smoke);
            skidParticle[i].Stop();
        }
        ActivateDeactivateBrakeLights(false);

        GameObject playerName = Instantiate(playerNamePrefab);
        playerName.GetComponent<NameUIController>().target = rb.gameObject.transform;

        if (GetComponent<AIController>().enabled)
        { 
            if(netName != "")
            {
                playerName.GetComponent<NameUIController>().playerName.text = netName;
            }
            else
            {
                playerName.GetComponent<NameUIController>().playerName.text = names[Random.Range(0, names.Length)];
            }
            
        }
        else
        {
            playerName.GetComponent<NameUIController>().playerName.text = PlayerPrefs.GetString("PlayerName");
        }
        playerName.GetComponent<NameUIController>().rendr = carMesh;
    }

    public void CalculateEngineSound()
    {
        float gearPerc = 1/(float)numGears;
        float targetGearFactor = Mathf.InverseLerp(gearPerc * currentGear, 
            gearPerc * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));

        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearFactor, 1, currentGearPerc);

        float speedPerc = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * (currentGear);

        if(currentGear>0 && speedPerc < downGearMax)
        {
            currentGear--;
        }

        if(speedPerc>upperGearMax && currentGear < (numGears - 1))
        {
            currentGear++;
        }

        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        acceleration.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

    private void ActivateDeactivateBrakeLights(bool enable)
    {
        for (int i = 0; i < 2; i++)
        {
            brakeLight[i].SetActive(enable);
        }
    }

    public void StartSkidTrail(int i)
    {
        if(skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(skidTrailPrefab);
        }

        skidTrails[i].parent = wc[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * wc[i].radius;
    }

    public void StopSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            return;
        }

        Transform holder = skidTrails[i];

        skidTrails[i] = null;
        holder.parent = null;
        holder.localRotation = Quaternion.Euler(90, 0, 0);

        Destroy(holder.gameObject, 30);
    }



    public void CheckSkidding()
    {
        int numSkidding = 0;

        for(int i = 0; i < 4; i++)
        {
            WheelHit hit;

            wc[i].GetGroundHit(out hit);

            if(Mathf.Abs(hit.forwardSlip) >= 0.4f || Mathf.Abs(hit.sidewaysSlip) >= 0.4f)
            {
                numSkidding++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                    
                }
                //StartSkidTrail(i);
                skidParticle[i].transform.position = wc[i].transform.position - wc[i].transform.up * wc[i].radius;
                skidParticle[i].Emit(1);
            }
            else
            {
                //StopSkidTrail(i);
            }
        }

        if(numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }

    public void Go(float acc, float s, float b)
    {
        acc = Mathf.Clamp(acc, -1, 1);
        s = Mathf.Clamp(s, -1, 1) * maxSteerAngle;
        b = Mathf.Clamp(b, 0, 1) * maxBrakeTorque;
        Debug.Log("0 - 1 brake: " + b);
        if (b != 0)
        {
            ActivateDeactivateBrakeLights(true);
        }
        else
        {
            ActivateDeactivateBrakeLights(false);
        }

        float thrustTorque = 0;
        if (currentSpeed < maxSpeed)
        {
            thrustTorque = acc * torque;
        }


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
