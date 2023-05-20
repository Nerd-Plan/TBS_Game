using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{

    [SerializeField] private Button endTurnBtn;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    private void Start()
    {
        endTurnBtn.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {
        if(TurnSystem.Instance.IsEndGame) { return; }
        turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateEnemyTurnVisual()
    {
        if (TurnSystem.Instance.IsEndGame) { return; }
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        if (TurnSystem.Instance.IsEndGame) { return; }
        endTurnBtn.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }


}
