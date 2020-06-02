using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceMonitor : MonoBehaviour
{
    public GameObject[] countDown;
    public static bool racing = false;
    public static int totalLaps = 1;
    public GameObject gameOverPanel;
    public GameObject HUD;

    CheckpointManager[] carsCPM;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject g in countDown)
        {
            g.SetActive(false);
        }

        StartCoroutine(PlayCountDown());
        gameOverPanel.SetActive(false);

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsCPM = new CheckpointManager[cars.Length];

        for(int i = 0; i < cars.Length; i++)
        {
            carsCPM[i] = cars[i].GetComponent<CheckpointManager>();
        }
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
    void LateUpdate()
    {
        int finishCount = 0;

        foreach(CheckpointManager cpm in carsCPM)
        {
            if(cpm.lap == totalLaps + 1)
            {
                finishCount++;
            }
        }

        if(finishCount == carsCPM.Length)
        {
            HUD.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Tracks");
    }
}
