using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance;

    public event Action OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;

    [SerializeField] Unit selected_unit;
    [SerializeField] LayerMask unit_layer;

    BaseAction selected_action;

    bool isBusy;

    public event Action OnActionStarted;

    public event Action<bool> OnBusyChanged;

    public void SetBusy() { isBusy = true; OnBusyChanged?.Invoke(isBusy); }
    public void ClearBusy() { isBusy = false; OnBusyChanged?.Invoke(isBusy); }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        if(selected_unit)
        SetSelectedAction(selected_unit.GetAction<MoveAction>());
    }
    private void Update()
    {
       if (isBusy) return;
        if (!TurnSystem.Instance.IsPlayerTurn()) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
       if(TryHandleUnitSelection())return;
       HandleSelctedAction();
    }

    private bool TryHandleUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) 
            return false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unit_layer))
        {
           if(!raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                return false;
            if(unit==selected_unit||!unit.IsPlayer())
                return false;
            SetSelectedUnit(unit);
            return true;
        }
        return false; 
    }

    private void SetSelectedUnit(Unit unit)
    {
        if (unit == null) return;
        selected_unit = unit;
        SetSelectedAction(selected_unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke();
    }

    public Unit GetSelectedUnit()=>selected_unit;
    
    public void SetSelectedAction(BaseAction action)
    {
        selected_action = action;
        OnSelectedActionChanged?.Invoke(this,EventArgs.Empty);
    }

    void HandleSelctedAction()
    {
        if(!Input.GetMouseButtonDown(0))
        return;
        if (selected_unit == null) return;
        GridPosition mousegridposition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        if (!selected_action.IsValidAtGridPosition(mousegridposition))
        return;
        if (!selected_unit.TrySpendActionPointsToTakeAction(selected_action)) 
        return;
        //Send To server
        SetBusy();
        selected_action.TakeAction(mousegridposition, ClearBusy);
        OnActionStarted?.Invoke();
    }

    public BaseAction GetSelectedAction()=>selected_action;
}
