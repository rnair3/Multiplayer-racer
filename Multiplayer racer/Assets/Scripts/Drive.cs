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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            skidParticle[i] = Instantiate(smoke);
            skidParticle[i].Stop();
        }
        ActivateDeactivateBrakeLights(false);
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

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");

        Go(a, steer, brake);

        CheckSkidding();
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

        if(b != 0)
        {
            ActivateDeactivateBrakeLights(true);
        }
        else
        {
            ActivateDeactivateBrakeLights(false);
        }

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
