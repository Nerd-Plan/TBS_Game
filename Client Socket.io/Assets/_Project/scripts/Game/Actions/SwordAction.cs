using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    [SerializeField] Transform sword;
    Unit targetUnit;
    public override string GetActionAsString()=> $"Sword Action , Unit : {GetUnit().name}, Position {targetUnit.GetWorldPosition()} key123";
        
    public override string GetActionName()=>"Sword";
    public int GetMaxSwordDistance() => 1;
    private void Update()
    {
        if (!isActive)
            return;
        ActionComplete();
    }
    public override void TakeAction(GridPosition gridPosition, Action OnActionComplete)
    {
        SetTarget(gridPosition);
        TakeAction(OnActionComplete);
    }
    public override void TakeAction(Action OnActionComplete)
    {
        ActionStart(OnActionComplete);
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) => new EnemyAIAction { gridPosition = gridPosition, actionValue = 0, };


    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                if (offsetGridPosition.Equals(new GridPosition(0, 0)))
                    continue;
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){continue;}
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > 2){continue;}
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; }
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsPlayer() == unit.IsPlayer()) { continue; }
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;

    }
    public override void SetTarget(GridPosition gridPosition)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
    }
}
