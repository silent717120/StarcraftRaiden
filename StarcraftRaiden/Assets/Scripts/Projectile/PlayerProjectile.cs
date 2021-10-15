using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trail;

    protected virtual void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();

        //子彈往上飛行
        if (moveDirection != Vector2.up)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.up, moveDirection);
        }
    }

    private void OnDisable()
    {
        trail.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PlayerEnergy.Instance.ProjectlieObtain(PlayerEnergy.PERCENT);
    }
}
