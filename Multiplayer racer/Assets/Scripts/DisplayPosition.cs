using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPosition : MonoBehaviour
{
    public TextMeshProUGUI first;
    public TextMeshProUGUI second;
    public TextMeshProUGUI third;
    public TextMeshProUGUI fourth;

    private void Start()
    {
        Leaderboard.ResetValues();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //List<string> places = Leaderboard.GetPositions();
        //first.text = places[0];
        //second.text = places[1];
        //third.text = places[2];
        //fourth.text = places[3];
    }
}
