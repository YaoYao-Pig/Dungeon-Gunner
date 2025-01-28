using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreTextTMP;

    private void Awake()
    {
        scoreTextTMP = GetComponentInChildren<TextMeshProUGUI>();

    }
    private void Start()
    {
        StaticEventHandler.CallScoreChangedEvent(0, 0);
    }

    private void OnEnable()
    {
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;

    }



    private void OnDisable()
    {
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;

    }


    private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs scoreChangedArgs)
    {
        scoreTextTMP.text = "SCORE: " + scoreChangedArgs.score.ToString("###,###0") +"\nMULTIPLIER: x"+scoreChangedArgs.multiplier;
    }

}
