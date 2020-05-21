using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceMonitor : MonoBehaviour
{
    public GameObject[] countDown;
    public static bool racing = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject g in countDown)
        {
            g.SetActive(false);
        }

        StartCoroutine(PlayCountDown());
    }

    IEnumerator PlayCountDown()
    {
        yield return new WaitForSeconds(2f);
        foreach(GameObject g in countDown)
        {
            g.SetActive(true);
            yield return new WaitForSeconds(1f);
            g.SetActive(false);
        }
        racing = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
