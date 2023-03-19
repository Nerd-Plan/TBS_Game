using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{

    protected bool isActive;
    protected Unit unit;
    protected Action onActionComplete;

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;



    protected virtual void Awake()
    {
        unit = transform.GetComponent<Unit>();
    }
    public abstract string GetActionName();
    public abstract void TakeAction(GridPosition gridPosition, Action OnActionComplete);
    public virtual bool IsValidAtGridPosition(GridPosition gridPosition)=> GetValidGridPositionList().Contains(gridPosition);

    public abstract List<GridPosition> GetValidGridPositionList();
    public virtual int GetActionPointsCost() => 1;

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete?.Invoke();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }
    public Unit GetUnit() => unit;

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        }
        else
        {
            // No possible Enemy AI Actions
            return null;
        }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

}
