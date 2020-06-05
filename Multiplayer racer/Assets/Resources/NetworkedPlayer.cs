using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class NetworkedPlayer : MonoBehaviourPunCallbacks
{
    public static GameObject localPlayerInstance;
    public GameObject playerNamePrefab;
    public Rigidbody rb;
    public Renderer carMesh;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            localPlayerInstance = gameObject;
        }
        else
        {
            GameObject playerName = Instantiate(playerNamePrefab);
            playerName.GetComponent<NameUIController>().target = rb.gameObject.transform;
            string setName = null;
            if (photonView.InstantiationData != null)
            {
                setName = (string)photonView.InstantiationData[0];
            }
            if (setName != null)
                playerName.GetComponent<TextMeshProUGUI>().text = setName;
            else
                playerName.GetComponent<TextMeshProUGUI>().text = photonView.Owner.NickName;
            playerName.GetComponent<NameUIController>().rendr = carMesh;
        }
    }
}
