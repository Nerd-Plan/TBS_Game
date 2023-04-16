using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSwordsAction : BaseAction
{
    private static object lockObject = new object(); // Object used for locking

    [SerializeField] Transform iceSwords;
     Unit targetUnit;
    [SerializeField] Transform defaultweapon;
    public Action OnCastAbility;
    [SerializeField] int MaxThrowDistance=3;
    public override string GetActionAsString() => $"IceSwords Action , Unit : {GetUnit().name}, Position {targetUnit.GetWorldPosition()} key123";

    public override string GetActionName() => "IceSwords";
    public int GetMaxThrowDistance() => MaxThrowDistance;
    private void Update()
    {
        if (!isActive)
            return;
        
    }
    public override int GetActionPointsCost()
    {
        return 3;
    }
    public override void TakeAction(GridPosition gridPosition, Action OnActionComplete)
    {
            SetTarget(gridPosition);
            TakeAction(OnActionComplete);
    }
    public override void TakeAction(Action OnActionComplete)
    {
        transform.LookAt(targetUnit.GetWorldPosition());
        defaultweapon.gameObject.SetActive(false);
        OnCastAbility?.Invoke();
        ActionStart(OnActionComplete);
    }
    void IceSwordsCast()
    {
        StartCoroutine(nameof(HitTargetUnit));
    }
    private IEnumerator HitTargetUnit()
    {
        Transform g = Instantiate(iceSwords, targetUnit.GetWorldPosition() + Vector3.up * 20, Quaternion.identity);
        Destroy(g.gameObject, 10);
        yield return new WaitForSeconds(1.25f);
        targetUnit.TakeDamge(50);
        ActionComplete();
        yield return new WaitForSeconds(.25f);
        defaultweapon.gameObject.SetActive(true);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) => new EnemyAIAction { gridPosition = gridPosition, actionValue = 2000, };


    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -MaxThrowDistance; x <= MaxThrowDistance; x++)
        {
            GridPosition offsetGridPosition = new GridPosition(x, 0);
            GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
            if (testGridPosition.Equals(unitGridPosition))
                continue;
            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }
            if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; }
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
            if (targetUnit.IsPlayer() == unit.IsPlayer()) { continue; }
            validGridPositionList.Add(testGridPosition);         
        }
        for (int z = -MaxThrowDistance; z <= MaxThrowDistance; z++)
        {
            GridPosition offsetGridPosition = new GridPosition(0,z);
            GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
            if (testGridPosition.Equals(unitGridPosition))
                continue;
            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }
            if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; }
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
            if (targetUnit.IsPlayer() == unit.IsPlayer()) { continue; }
            validGridPositionList.Add(testGridPosition);
        }

        return validGridPositionList;

    }
    public override void SetTarget(GridPosition gridPosition)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }
}
