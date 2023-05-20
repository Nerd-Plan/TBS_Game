using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using TBS.NetWork;
using TBS.Threading;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] GameObject end_game_ui;
    [SerializeField] GameObject start_game_ui;
    [SerializeField] TMP_Text winner_loser_text;
    private void Start()
    {
        Unit.OnAnyUnitDead += OnCheckWinner;  
    }

    private void OnCheckWinner(object sender, EventArgs e)
    {
        if (UnitManager.Instance.GetEnemyUnitList().Count != 0&& UnitManager.Instance.GetFriendlyUnitList().Count != 0) { return; }
        if (SceneManager.GetActiveScene().name.StartsWith("GameScene 1"))
        {
            if (!FindObjectOfType<LevelScripting>().GetHasShowFirstHider())
                return;
        }    
        end_game_ui.SetActive(true);    
        start_game_ui.SetActive(false);
        TurnSystem.Instance.IsEndGame = true;
        winner_loser_text.text = UnitManager.Instance.GetEnemyUnitList().Count == 0 ? "YOU win" : "YOu Lose";
    }
    public void GoToMenu()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("MultplayerGameScene"))
        { 
        if (Client.Instance.GetGameClient().IsOwner)
            Destroy(FindObjectOfType<Server>().gameObject);         
        Destroy(FindObjectOfType<Client>().gameObject);
        Destroy(FindObjectOfType<MainThreadDispatcher>().gameObject);
        Destroy(FindObjectOfType<GameManger>().gameObject);
        }


        SceneManager.LoadScene(0);
    }
}
