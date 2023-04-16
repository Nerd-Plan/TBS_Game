using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] Transform BullatPrefab;
    [SerializeField] Transform BullatParent;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
        if (TryGetComponent<IceSwordsAction>(out IceSwordsAction iceSwordsAction))
        {
            iceSwordsAction.OnCastAbility += iceSwordsAction_OnCastAbility;
        }
    }

    private void iceSwordsAction_OnCastAbility()=>animator.SetTrigger("IceSowrdsCast");
    
    private void MoveAction_OnStartMoving()=>animator.SetBool("IsWalking", true);
    private void MoveAction_OnStopMoving()=>animator.SetBool("IsWalking", false);
    
    private void ShootAction_OnShoot(Vector3 targetpos)
    {
        animator.SetTrigger("Shoot");
        Transform bullat = Instantiate(BullatPrefab, BullatParent);
        targetpos.y = 1.5f;
        bullat.GetComponent<BullatTrail>().SetTargetPosition(targetpos);
    }

}


