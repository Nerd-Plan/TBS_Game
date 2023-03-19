using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BullatTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;

    Vector3 targetposition;
    public void SetTargetPosition(Vector3 Targetpos)=>targetposition = Targetpos;

    [SerializeField] float moveSpeed = 80f;
    private void Update()
    {
        Vector3 moveDir = (targetposition - transform.position).normalized;
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetposition);
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        float distanceAfterMoving = Vector3.Distance(transform.position, targetposition);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetposition;
            transform.parent = null;
            Destroy(gameObject);
            Instantiate(bulletHitVfxPrefab, targetposition, Quaternion.identity);
        }

    }
}
