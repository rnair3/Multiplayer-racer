using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{

    CanvasGroup grp;
    float HUDAlpha = 0;

    // Start is called before the first frame update
    void Start()
    {
        grp = GetComponent<CanvasGroup>();
        grp.alpha = HUDAlpha;

        if (PlayerPrefs.HasKey("HUD"))
        {
            HUDAlpha = PlayerPrefs.GetFloat("HUD");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RaceMonitor.racing)
        {
            grp.alpha = HUDAlpha;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            grp.alpha = grp.alpha == 1 ? 0 : 1;
            HUDAlpha = grp.alpha;
            PlayerPrefs.SetFloat("HUD", HUDAlpha);
        }
    }
}
