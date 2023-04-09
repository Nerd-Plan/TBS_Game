using System;
using System.Collections.Generic;
using System.Linq;
using TBS.NetWork;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class GameManger : MonoBehaviour
{
    #region Prop's
    [SerializeField] Transform NormalUnit;
    [SerializeField] Transform EnemyUnit;
    [SerializeField] Transform LevelGridGrid;

    GameObject grid;
    public static bool IsGameStarted=false;


    #endregion

    #region Start functions
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(IsGameStarted)return;

        ListenToClientsEvents();
        if (!GameObject.FindGameObjectWithTag("Grid"))
        {
            grid =  Instantiate(LevelGridGrid, transform).gameObject;
        }
        IsGameStarted=true;

    }


    private void ListenToClientsEvents()
    {
        Client.Instance.GetGameClient().OnSpawnUnitsOnSide += GameClient_SpwanUnitsOnSide;
        Client.Instance.GetGameClient().OnPlayerStartGameMove += GameClient_OnGameMove;
        Client.Instance.GetGameClient().OnUnitDoAction += GameClient_OnUnitDoAction;
        Client.Instance.GetGameClient().OnSwitchTurns += GameClient_OnSwitchTurns;


    }

    
    #endregion

    #region Player Action
    private void GameClient_OnUnitDoAction(string msg)
    {
        string action_name = string.Empty;
        GridPosition unitgridposition = new GridPosition(-1, -1);
        GridPosition targetPosition = new GridPosition(-1, -1);

        // Get the action name and target position
        (action_name, targetPosition) = ParseActionString(msg);

        // Get the unit position from the msg string
        string unitString = GetUnitNameFromString(msg);
        unitgridposition = GetGridPositionFromString(unitString);

        Debug.Log($"Action Name : {action_name} , Unit at Grid Postion {unitgridposition} ,target Position {targetPosition}");
        Unit unit = UnitManager.Instance.GetUnitList().Find(u => u.name.Contains(unitString));
        while(unit == null)
        {
            unit = UnitManager.Instance.GetUnitList().Find(u => u.name.Contains(unitString));
        }

        Type componentType = Type.GetType(action_name.Trim().Replace(" ",""));
        Component component = unit.GetComponent(componentType);
        BaseAction action = component as BaseAction;
        action.SetTarget(targetPosition);
        action.TakeAction(UnitActionSystem.Instance.ClearBusy);
        unit.TrySpendActionPointsToTakeAction(action);



    }
    string GetUnitNameFromString(string str)
    {
        int start = str.IndexOf("Unit: (") + "Unit: (".Length;
        int end = str.IndexOf(')', start);
        return str.Substring(start, end - start);
    }
    (string, GridPosition) ParseActionString(string msg)
    {
        int start = msg.IndexOf("Position (") + "Position (".Length;
        int end = msg.IndexOf(')', start);
        string positionString = msg.Substring(start, end - start);

        string[] actionStrings = msg.Split(',');
        string actionString = actionStrings[0].Trim();

        GridPosition targetPosition = GetGridPositionFromString(positionString);
        return(actionString, targetPosition);
    }
    GridPosition GetGridPositionFromString(string str)
    {
        Vector3Int vector3 = GetVector3FromString(str);     
        return new GridPosition(vector3.x/2, vector3.z / 2);
    }
    Vector3Int GetVector3FromString(string str)
    {
        str = str.Replace("(", "").Replace(")", "");
        string[] components = str.Split(',');
        if (components.Length == 3 && float.TryParse(components[0], out float x) && float.TryParse(components[1], out float y) && float.TryParse(components[2], out float z))
        {
            return new Vector3Int((int)x, (int)y, (int)z);
        }
        return Vector3Int.zero;
    }
    #endregion


    #region FirstTurnOfPlayer
    bool isplayermovefirst = true;
    public void GameClient_OnGameMove(bool playermovefirst)
    {
        isplayermovefirst = playermovefirst;
        Debug.Log(playermovefirst);
        Invoke("PlayerStartGame", .3f);
    }

    private void PlayerStartGame()
    {
        if (!isplayermovefirst) return;
        TurnSystem.Instance.SetEnemyTurn();
    }
    private void GameClient_OnSwitchTurns(byte obj)
    {
        TurnSystem.Instance.NextTurn();
    }
    #endregion


    #region Spawn Clients
    private void GameClient_SpwanUnitsOnSide(int i = 0)
    {
        if (i == 1)
        {
            InstantiateNormalUnitOnSideOne();
            InstantiateEnemyUnitOnSideTwo();
        }
        else if(i==2)
        {
            InstantiateEnemyUnitOnSideOne();
            InstantiateNormalUnitOnSideTwo();
            FindObjectOfType<CameraController>().transform.SetPositionAndRotation(new Vector3(10, 0, 20), new Quaternion(0, 180, 0, 0));

        }
    }

    private void InstantiateNormalUnitOnSideOne()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            if(i%2==0)
            Instantiate(NormalUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, 0)),Quaternion.identity).name=$"Unit: {LevelGrid.Instance.GetWorldPosition(new GridPosition(i, 0))}";
        }
    }
    private void InstantiateEnemyUnitOnSideOne()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            if (i % 2 == 0)
                Instantiate(EnemyUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, 0)), Quaternion.identity).name = $"Unit: {LevelGrid.Instance.GetWorldPosition(new GridPosition(i, 0))}";
        }
    }
    private void InstantiateNormalUnitOnSideTwo()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            if (i % 2 == 0)
                Instantiate(NormalUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, LevelGrid.Instance.GetHeight()-1)), new Quaternion(0, 180, 0, 0)).name = $"Unit: {LevelGrid.Instance.GetWorldPosition(new GridPosition(i, LevelGrid.Instance.GetHeight() - 1))}";
        }
    }
    private void InstantiateEnemyUnitOnSideTwo()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            if (i % 2 == 0)
                Instantiate(EnemyUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, LevelGrid.Instance.GetHeight()-1)), new Quaternion(0, 180, 0, 0)).name = $"Unit: {LevelGrid.Instance.GetWorldPosition(new GridPosition(i, LevelGrid.Instance.GetHeight() - 1))}";
        }
    }
    #endregion
}
