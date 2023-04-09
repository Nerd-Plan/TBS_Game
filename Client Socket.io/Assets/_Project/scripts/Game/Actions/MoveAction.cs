using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] int max_move_distance;

    private Vector3 targetPosition;
    public Vector3 GetTargetPosition()=>targetPosition;
    public Vector3 SetTargetPosition(Vector3 t) => targetPosition=t;

    float stoppingDistance = .1f;
    float moveSpeed = 4f;
    float rotationspeed = 8;

    public event Action OnStartMoving;
    public event Action OnStopMoving;

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }
    private void Update()
    {
        if (!isActive) 
            return;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            OnStopMoving?.Invoke();
            isActive = false;
            onActionComplete();
        }
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationspeed);
    }


    public override List<GridPosition> GetValidGridPositionList()
    {
       List<GridPosition> validgridPositions = new List<GridPosition>();
        GridPosition unitgridposition = unit.GetGridPosition();
        for (int x = -max_move_distance; x <= max_move_distance; x++)
        {
            for (int z = -max_move_distance; z <= max_move_distance; z++)
            {
                GridPosition offest = new GridPosition(x, z);
                GridPosition testGridposition= unitgridposition+offest;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridposition) || (x == 0 && z == 0) || LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridposition)) { continue; }
                validgridPositions.Add(testGridposition);                
            }
        }
        return validgridPositions;
    }

    public override string GetActionName() => "Move";

    public override void TakeAction(GridPosition gridPosition, Action OnActionComplete)
    {
        OnStartMoving?.Invoke();
        onActionComplete = OnActionComplete;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        isActive = true;
        ActionStart(onActionComplete);
    }
    public override void TakeAction(Action OnActionComplete)
    {
        OnStartMoving?.Invoke();
        onActionComplete = OnActionComplete;
        isActive = true;
        ActionStart(onActionComplete);
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
    public override string GetActionAsString() => $"Move Action , Unit : {GetUnit().name}, Position {GetTargetPosition()} key123";

    public override void SetTarget(GridPosition gridPosition)
    {
        targetPosition= LevelGrid.Instance.GetWorldPosition(gridPosition);
    }
}
