using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{

    private GridSystem gridSystem;
    private GridPosition gridPosition;
    
    List<Unit> unitList;
    public void AddUnit(Unit unit) { unitList.Add(unit); }
    public void RemoveUnit(Unit unit) { unitList.Remove(unit); }
    public List<Unit> GetUnitList() =>unitList;


    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitsname = "";
        foreach(Unit unit in unitList)
        {
            unitsname+=unit+"\n";
        }
        return gridPosition.ToString()+"\n"+ unitList.Count;
    }
    public bool HasAnyUnit() => unitList.Count > 0;
    public Unit GetUnit()
    {
        if(HasAnyUnit())
        return unitList[0];

       return null;           
    }

}
