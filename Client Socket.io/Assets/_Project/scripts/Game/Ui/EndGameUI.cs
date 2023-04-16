using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] GameObject WinUi;
    [SerializeField] GameObject LoseUi;
    [SerializeField] GameObject QuitButton;
    [SerializeField] List<int> targetLayer=new List<int>();   
    private void Awake()
    {
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        if (UnitManager.Instance.GetFriendlyUnitList().Count > 0 && UnitManager.Instance.GetEnemyUnitList().Count > 0)
            return;
        foreach(Transform g in transform.parent)
        {
            if(g == transform)
            {
                continue;
            }
            g.gameObject.SetActive(false);
        }
        QuitButton.SetActive(true);
        if (UnitManager.Instance.GetFriendlyUnitList().Count <= 0)
        {
            LoseUi.SetActive(true);
        }
        else if(UnitManager.Instance.GetEnemyUnitList().Count <= 0)
        {
            WinUi.SetActive(true);
        }
    }


    public void BackToMenu()
    {
        // Find all objects of type GameObject in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Loop through all the objects
        for (int i = 0; i < allObjects.Length; i++)
        {
            // Check if the object's layer matches the target layer
            if (targetLayer.Contains(allObjects[i].layer))
            {
                // Destroy the object
                Destroy(allObjects[i]);
            }
        }
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
