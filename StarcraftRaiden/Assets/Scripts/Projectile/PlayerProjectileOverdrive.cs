using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileOverdrive : PlayerProjectile
{
    [Header("導彈系統")]
    [SerializeField] ProjectileGuidanceSystem guidanceSystem;

    protected override void OnEnable()
    {
        //設定隨機敵人
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;

        //若沒敵人
        if (target == null)
        {
            base.OnEnable(); //子彈基底會自動固定方向移動
        }
        else
        {
            StartCoroutine(guidanceSystem.HomingCoroutine(target)); //追蹤敵人
        }
    }
}
