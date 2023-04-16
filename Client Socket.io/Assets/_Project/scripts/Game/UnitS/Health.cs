using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health_points;
    private int healthMax;


    private void Awake()
    {
        healthMax = health_points;
    }

    public event Action OnDie;
    public event Action OnDamaged;
    public void TakeDamge(int damge_points)
    {
        health_points = Mathf.Max(health_points - damge_points,0);
        Debug.Log("HP: " + health_points);
        OnDamaged?.Invoke();
        if (health_points <= 0)
        {
            Die();
        }    
    }
    void Die()
    {
        OnDie?.Invoke();
    }
    public float GetHealthNormalized()
    {
        return (float)health_points / healthMax;
    }

}
