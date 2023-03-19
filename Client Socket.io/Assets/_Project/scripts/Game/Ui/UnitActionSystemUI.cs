using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] Transform actionbuttonPrefab;
    [SerializeField] Transform actionbuttonContainor;
    [SerializeField] TextMeshProUGUI actionpointstext;

    List<ActionButtonUI> buttons;
    private void Awake()
    {
        buttons = new List<ActionButtonUI>();
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanges;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanges;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;


        UpdateActionPoints();
        CreateUnitActionButtons();
        UpdateSelectedButtonsVisual();
    }

    private void TurnSystem_OnTurnChanged()
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged()
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanges(object sender,EventArgs e)
    {
        UpdateSelectedButtonsVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanges()
    {
        CreateUnitActionButtons();
        UpdateSelectedButtonsVisual();
        UpdateActionPoints();
    }
    private void UnitActionSystem_OnActionStarted()
    {
        UpdateActionPoints();
    }
    private void UpdateActionPoints()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == null)
            return;       
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            actionpointstext.text = "Action Points: " + selectedUnit.GetActionPoints();
        
    }

    public void CreateUnitActionButtons()
    {
        foreach (Transform button in actionbuttonContainor)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
        if (UnitActionSystem.Instance.GetSelectedUnit() == null)
            return;
        Unit selectedunit = UnitActionSystem.Instance.GetSelectedUnit();
        foreach (BaseAction action in selectedunit.GetBaseActionsArray())
        {
            Transform actionbutton= Instantiate(actionbuttonPrefab,actionbuttonContainor);
            ActionButtonUI actionButton = actionbutton.GetComponent<ActionButtonUI>();
            actionButton.SetBaseAction(action);
            buttons.Add(actionButton);
        }
    }
    public void UpdateSelectedButtonsVisual()
    {
        foreach(ActionButtonUI button in buttons)
        {
            button.UpdateSelectedVisual();
        }
    }


}
