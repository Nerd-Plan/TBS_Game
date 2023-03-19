using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManger : MonoBehaviour
{
    [SerializeField] GameObject ShootCamreaGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideShootCamera();
    }

    public void ShowShootCamrea()
    {
        ShootCamreaGameObject.SetActive(true);
    }
    public void HideShootCamera()
    {
        ShootCamreaGameObject.SetActive(false);
    }
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;
                Vector3 actionCameraPosition =shooterUnit.GetWorldPosition() +cameraCharacterHeight +shoulderOffset +(shootDir * -1);

                ShootCamreaGameObject.transform.position = actionCameraPosition;
                ShootCamreaGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                ShowShootCamrea();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideShootCamera();
                break;
        }
    }


}
