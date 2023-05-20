using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripting : MonoBehaviour
{

    [SerializeField] private List<GameObject> hider1List;
    [SerializeField] private List<GameObject> hider2List;
    [SerializeField] private List<GameObject> hider3List;
    [SerializeField] private List<GameObject> enemy1List;
    [SerializeField] private List<GameObject> enemy2List;
    [SerializeField] private List<Door> doors;

    private bool hasShownFirstHider = false;
    public bool GetHasShowFirstHider() => hasShownFirstHider;
    private void Start()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        foreach (Door door in doors)
        {           
            door.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider2List, false);
            };
        }
        
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        if (e.toGridPosition.z == 5 && !hasShownFirstHider)
        {
            hasShownFirstHider = true;
            SetActiveGameObjectList(hider1List, false);
            SetActiveGameObjectList(enemy1List, true);
           
        }
    }

    private void SetActiveGameObjectList(List<GameObject> gameObjectList, bool isActive)
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            gameObject.SetActive(isActive);
        }
    }

}
