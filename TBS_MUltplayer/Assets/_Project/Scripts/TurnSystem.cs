using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour
{

    public static TurnSystem Instance { get; private set; }


    public event EventHandler OnTurnChanged;
    public bool IsEndGame=false;    

    private int turnNumber = 0;
    private bool isPlayerTurn = true;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if(SceneManager.GetActiveScene().name.Contains("GameScene 1"))
        {
            turnNumber = 1;
        }
    }


    public void NextTurn()
    {
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void SetEnemyTurn()
    {
        isPlayerTurn = false;
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }
}
