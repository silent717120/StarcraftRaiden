using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileOverdrive : PlayerProjectile
{
    [Header("�ɼu�t��")]
    [SerializeField] ProjectileGuidanceSystem guidanceSystem;

    protected override void OnEnable()
    {
        //�]�w�H���ĤH
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;

        //�Y�S�ĤH
        if (target == null)
        {
            base.OnEnable(); //�l�u�򩳷|�۰ʩT�w��V����
        }
        else
        {
            StartCoroutine(guidanceSystem.HomingCoroutine(target)); //�l�ܼĤH
        }
    }
}
