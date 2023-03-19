using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endturnbtn;
    [SerializeField] private TextMeshProUGUI turnnumbertext;
    [SerializeField] private GameObject enemy_turn_ui;

    private void Start()
    {
        endturnbtn.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisubality();
    }

    private void TurnSystem_OnTurnChanged()
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisubality();
    }

    private void UpdateTurnText()
    {
        turnnumbertext.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }
    public void UpdateEnemyTurnVisual()
    {
        enemy_turn_ui.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }
    public void UpdateEndTurnButtonVisubality()
    {
        endturnbtn.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());   
    }

}
