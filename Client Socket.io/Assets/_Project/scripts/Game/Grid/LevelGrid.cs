using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }  

    GridSystem gridSystem;
    [SerializeField] Transform griddebugobject;
    public event EventHandler OnAnyUnitMovedGridPosition;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gridSystem = new GridSystem(10, 10,2);
        //gridSystem.CreateDebugObjects(griddebugobject);
    }
    #region UnitAtGridPosition

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject=gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    #endregion

    public void UnitMovedFromGridPositionToGridPosition(Unit unit,GridPosition fromgridPosition, GridPosition togridPosition)
    {
        RemoveUnitAtGridPosition(fromgridPosition,unit);
        AddUnitAtGridPosition(togridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    #region passing Functions From grid system
    public GridPosition GetGridPosition(Vector3 position)=>gridSystem.GetGridPosition(position);
    public Vector3 GetWorldPosition(GridPosition position) => gridSystem.GetWorldPosition(position);
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
    #endregion


    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)=>gridSystem.GetGridObject(gridPosition).HasAnyUnit();
    public Unit GetUnitAtGridPosition(GridPosition testGridposition)=>gridSystem.GetGridObject(testGridposition).GetUnit();
}
