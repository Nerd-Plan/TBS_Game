using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    private const int ACTION_POINTS_MAX = 2;
    [SerializeField] GridPosition gridPosition;
    [SerializeField] MoveAction moveAction;
    [SerializeField] SpinAction spinAction;
    [SerializeField] ShootAction shootaction;
    [SerializeField] Health health;
    BaseAction[] baseActionsarray;
    private int actionpoints = ACTION_POINTS_MAX;

    public static event Action OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] bool isenemy;
    public bool IsPlayer() => !isenemy;
    private void Awake()
    {
        baseActionsarray = GetComponents<BaseAction>();
    }
    private void Start()
    {
        gridPosition= LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        health.OnDie += health_OnDie;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void health_OnDie()
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    private void Update()
    {
        GridPosition newgridposition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (gridPosition != newgridposition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newgridposition;
            LevelGrid.Instance.UnitMovedFromGridPositionToGridPosition(this, oldGridPosition, newgridposition);

        }
    }

    private void TurnSystem_OnTurnChanged()
    {
        if ((IsPlayer() && TurnSystem.Instance.IsPlayerTurn()) || (!IsPlayer() && !TurnSystem.Instance.IsPlayerTurn()))
        {
            actionpoints = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke();
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionsarray)
        {
            if (baseAction is T)
                return (T)baseAction;
        }
        return null;
    }
    public GridPosition GetGridPosition()=>gridPosition;
    public BaseAction[] GetBaseActionsArray() => baseActionsarray;

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;      
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) => (actionpoints >= baseAction.GetActionPointsCost());

    private void SpendActionPoints(int amount)
    {
        actionpoints -= amount;
        OnAnyActionPointsChanged?.Invoke(); 
    }
    

    public int GetActionPoints()=> actionpoints;
    public Vector3 GetWorldPosition()=> transform.position;

    public float GetHealthNormalized()
    {
        return health.GetHealthNormalized();
    }

    public void TakeDamge(int damgepoints)
    {
        health.TakeDamge(damgepoints);
    }
}
