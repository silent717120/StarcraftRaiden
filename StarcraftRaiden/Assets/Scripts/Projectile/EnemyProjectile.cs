using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    void Awake()
    {
        if(moveDirection != Vector2.down)
        {
            transform.GetChild(0).rotation =  Quaternion.FromToRotation(Vector2.down, moveDirection);
        }
    }

    public void updateRoate()
    {
        if (moveDirection != Vector2.down)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.down, moveDirection);
        }
    }
}
