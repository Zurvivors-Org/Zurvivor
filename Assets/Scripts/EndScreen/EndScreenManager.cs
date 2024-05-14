using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private int currentLevels;
    void Start()
    {
        if (PlayerPrefs.HasKey("currentStage"))
        {
            currentLevels = PlayerPrefs.GetInt("currentStage");
        }
        else
        {
            currentLevels = -1;
        }

        scoreTxt.SetText("you survived " + currentLevels + " rounds");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
