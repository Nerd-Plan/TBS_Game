using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{

    float totalspintamount;
    private void Update()
    {
        if(!isActive)
        return;
        if (totalspintamount <= 360)
        {
            transform.eulerAngles += new Vector3(0, 360 * Time.deltaTime, 0);
            totalspintamount += 360 * Time.deltaTime;
        }
        else
        {
            isActive = false;
            onActionComplete();
        }
    }
    public override void TakeAction(GridPosition gridPosition,Action OnActionComplete)
    {
        onActionComplete = OnActionComplete;
        isActive = true;
        totalspintamount = 0;
        ActionStart(onActionComplete);
    }
    public override void TakeAction(Action OnActionComplete)
    {
        onActionComplete = OnActionComplete;
        isActive = true;
        totalspintamount = 0;
        ActionStart(onActionComplete);
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)=> new EnemyAIAction{gridPosition = gridPosition,actionValue = 0,};
    

    public override string GetActionName() => "Spin";

    public override List<GridPosition> GetValidGridPositionList()
    {
        GridPosition unitgridposition = unit.GetGridPosition();
        return new List<GridPosition>(){unit.GetGridPosition()};
    }

    public override string GetActionAsString()=> $"Spin Action , Unit : {GetUnit().name}, Position {GetUnit().GetWorldPosition()} key123";

    public override void SetTarget(GridPosition gridPosition)
    {
        return;
    }
}
