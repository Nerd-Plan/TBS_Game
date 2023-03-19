using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] Button button;
    [SerializeField] GameObject button_selected_visual;

    BaseAction baseaction;
    public void SetBaseAction(BaseAction action)
    {
        baseaction=action;
        textMeshProUGUI.text=action.GetActionName().ToUpper();
        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(action);
        });
    }
    public void UpdateSelectedVisual()
    {
        BaseAction selectedbaseaction=UnitActionSystem.Instance.GetSelectedAction();
        button_selected_visual.SetActive(selectedbaseaction == baseaction);
    }

}
