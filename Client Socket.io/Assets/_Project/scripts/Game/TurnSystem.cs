using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public event Action OnTurnChanged;


    private int turnNumber = 1;

    bool is_player_turn=true;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        is_player_turn = true;
    }


    public void NextTurn()
    {
        turnNumber++;
        is_player_turn = !is_player_turn;
        OnTurnChanged?.Invoke();
    }

    public int GetTurnNumber()=> turnNumber;

    public bool IsPlayerTurn() => is_player_turn;

}
