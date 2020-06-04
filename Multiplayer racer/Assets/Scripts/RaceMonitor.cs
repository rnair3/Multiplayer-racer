using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;

public class RaceMonitor : MonoBehaviourPunCallbacks
{
    public GameObject[] countDown;
    public static bool racing = false;
    public static int totalLaps = 1;
    public GameObject gameOverPanel;
    public GameObject HUD;

    public GameObject[] carsPrefab;
    public Transform[] spawns;

    CheckpointManager[] carsCPM;

    int playerCar;
    public GameObject startRace;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject g in countDown)
        {
            g.SetActive(false);
        }

        
        gameOverPanel.SetActive(false);
        playerCar = PlayerPrefs.GetInt("PlayerCar");
        startRace.SetActive(false);

        GameObject pcar = null;

        int randomStart = Random.Range(0, spawns.Length);
        Vector3 startPos = spawns[randomStart].position;
        Quaternion startRot= spawns[randomStart].rotation;

        if (PhotonNetwork.IsConnected)
        {
            startPos = spawns[PhotonNetwork.CurrentRoom.PlayerCount - 1].position;
            startRot = spawns[PhotonNetwork.CurrentRoom.PlayerCount - 1].rotation;
            if (NetworkedPlayer.localPlayerInstance == null)
            {
                pcar = PhotonNetwork.Instantiate(carsPrefab[playerCar].name, startPos, startRot, 0);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                startRace.SetActive(true);
            }
        }
        else
        {

            pcar = Instantiate(carsPrefab[playerCar]);
            pcar.transform.position = startPos;
            pcar.transform.rotation = startRot;

            foreach (Transform t in spawns)
            {
                if (t == spawns[randomStart]) continue;
                GameObject c = Instantiate(carsPrefab[Random.Range(0, carsPrefab.Length)]);
                c.transform.position = t.position;
                c.transform.rotation = t.rotation;
            }

            StartGame();
        }

        

        SmoothFollow.playerCar = pcar.gameObject.GetComponent<Drive>().rb.transform;
        pcar.GetComponent<AIController>().enabled = false;
        pcar.GetComponent<Drive>().enabled = true;
        pcar.GetComponent<PlayerController>().enabled = true;

        
    }

    public void StartGame()
    {
        StartCoroutine(PlayCountDown());
        startRace.SetActive(false);


        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsCPM = new CheckpointManager[cars.Length];

        for (int i = 0; i < cars.Length; i++)
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
        if (!racing) return;
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
