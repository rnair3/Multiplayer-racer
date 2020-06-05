using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameUIController : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI lapsDisp;
    public Transform target;
    CanvasGroup canvasGrp;
    public Renderer rendr;
    CheckpointManager c;

    int carReg;
    bool regoSet = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        playerName = GetComponent<TextMeshProUGUI>();
        canvasGrp = GetComponent<CanvasGroup>();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(!RaceMonitor.racing) { canvasGrp.alpha = 0; return; }
        if (!regoSet)
        {
            carReg = Leaderboard.RegisterCar(playerName.text);
            regoSet = true;
            return;
        }
        if (rendr == null) return;
        
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, rendr.bounds);
        canvasGrp.alpha = carInView ? 1 : 0;

        transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 2f);

        if(c == null)
        {
            c = target.GetComponent<CheckpointManager>();
        }

        Leaderboard.SetPosition(carReg, c.lap, c.checkpoint, c.time);
        string position = Leaderboard.GetPosition(carReg);

        lapsDisp.text = "Position: " + position + " Lap: " + c.lap;
    }
}
