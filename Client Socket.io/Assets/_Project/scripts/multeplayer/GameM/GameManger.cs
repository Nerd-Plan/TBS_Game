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
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        ListenToClientsEvents();
        if (!GameObject.FindGameObjectWithTag("Grid"))
        {
            grid =  Instantiate(LevelGridGrid, transform).gameObject;
        }
    }


    private void ListenToClientsEvents()
    {
        Client.Instance.GetGameClient().OnSpawnUnitsOnSideOne += GameClient_SpwanUnitsOnSideOne;
        Client.Instance.GetGameClient().OnSpawnUnitsOnSideTwo += GameClient_SpwanUnitsOnSideTwo;

        Client.Instance.GetGameClient().OnActionHasBeenDone += GameClient_ActionHasBeenDone;
    }

    private void GameClient_ActionHasBeenDone(Tuple<string,Tuple<string, string>> action )
    {
       
    }

    private void GameClient_SpwanUnitsOnSideOne()
    {
        InstantiateNormalUnitOnSideOne();
        InstantiateEnemyUnitOnSideTwo();
    }
    private void GameClient_SpwanUnitsOnSideTwo()
    {
        InstantiateEnemyUnitOnSideOne();
        InstantiateNormalUnitOnSideTwo();
        GameObject.FindObjectOfType<CameraController>().transform.SetPositionAndRotation(new Vector3(10, 0, 20),new Quaternion(0,180,0,0));

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
}
