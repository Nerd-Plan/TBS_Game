using System;
using TBS.NetWork;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    [SerializeField] Transform NormalUnit;
    [SerializeField] Transform EnemyUnit;
    [SerializeField] Transform LevelGridGrid;

    GameObject grid;
    public static bool IsGameStarted=false;
    private void Start()
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

        if (UnitActionSystem.Instance != null)
        {
            UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        }
    }

    public void GameClient_OnGameMove(bool obj)
    {
        Debug.Log(obj);
        if (!obj) return;
        TurnSystem.Instance.NextTurn();
    }

    private void UnitActionSystem_OnActionStarted()
    {
       Unit unit= UnitActionSystem.Instance.GetSelectedUnit();
       BaseAction baseAction= UnitActionSystem.Instance.GetSelectedAction();
    }


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
            GameObject.FindObjectOfType<CameraController>().transform.SetPositionAndRotation(new Vector3(10, 0, 20), new Quaternion(0, 180, 0, 0));

        }
    }

    private void InstantiateNormalUnitOnSideOne()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            Instantiate(NormalUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, 0)),Quaternion.identity);
        }
    }
    private void InstantiateEnemyUnitOnSideOne()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
           Instantiate(EnemyUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, 0)), Quaternion.identity);
        }
    }
    private void InstantiateNormalUnitOnSideTwo()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            Instantiate(NormalUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, LevelGrid.Instance.GetHeight()-1)), new Quaternion(0, 180, 0, 0));
        }
    }
    private void InstantiateEnemyUnitOnSideTwo()
    {
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            Instantiate(EnemyUnit, LevelGrid.Instance.GetWorldPosition(new GridPosition(i, LevelGrid.Instance.GetHeight()-1)), new Quaternion(0, 180, 0, 0));
        }
    }
    #endregion
}
