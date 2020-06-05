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
    public GameObject waitingText;

    // Start is called before the first frame update
    void Start()
    {
        racing = false;
        foreach(GameObject g in countDown)
        {
            g.SetActive(false);
        }

        
        gameOverPanel.SetActive(false);
        playerCar = PlayerPrefs.GetInt("PlayerCar");
        startRace.SetActive(false);
        waitingText.SetActive(false);

        GameObject pcar = null;

        int randomStart = Random.Range(0, spawns.Length);
        Vector3 startPos = spawns[randomStart].position;
        Quaternion startRot= spawns[randomStart].rotation;

        if (PhotonNetwork.IsConnected)
        {
            startPos = spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1].position;
            startRot = spawns[PhotonNetwork.LocalPlayer.ActorNumber - 1].rotation;
            if (NetworkedPlayer.localPlayerInstance == null)
            {
                pcar = PhotonNetwork.Instantiate(carsPrefab[playerCar].name, startPos, startRot, 0);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                startRace.SetActive(true);
            }
            else
            {
                waitingText.SetActive(true);
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


    public void BeginGame()
    {
        int numPlayers = PhotonNetwork.CurrentRoom.MaxPlayers - PhotonNetwork.CurrentRoom.PlayerCount;
        string[] names = { "Adrian", "Lee", "Ryan", "Tom", "Kate", "Raj" };
        for (int i = PhotonNetwork.CurrentRoom.PlayerCount; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            Vector3 startPos = spawns[i].position;
            Quaternion startRot = spawns[i].rotation;
            int r = Random.Range(0, carsPrefab.Length);

            object[] instanceData = new object[1];
            instanceData[0] = (string)names[Random.Range(0, names.Length)];

            GameObject AICar = PhotonNetwork.Instantiate(carsPrefab[r].name, startPos, startRot, 0, instanceData);
            AICar.GetComponent<AIController>().enabled = true;
            AICar.GetComponent<Drive>().enabled = true;
            AICar.GetComponent<Drive>().netName = (string)instanceData[0];
            AICar.GetComponent<PlayerController>().enabled = false;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All, null);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        StartCoroutine(PlayCountDown());
        startRace.SetActive(false);
        waitingText.SetActive(false);


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

    //bool raceOver = false;
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        raceOver = true;
    //    }
    //}


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

        if(finishCount == carsCPM.Length )//|| raceOver)
        {
            HUD.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }

    [PunRPC]
    public void RestartGame()
    {
        PhotonNetwork.LoadLevel("Tracks");
    }

    public void RestartLevel()
    {
        racing = false;
        if (PhotonNetwork.IsConnected)
            photonView.RPC("RestartGame", RpcTarget.All, null);
        else
            SceneManager.LoadScene("Tracks");
    }
}
