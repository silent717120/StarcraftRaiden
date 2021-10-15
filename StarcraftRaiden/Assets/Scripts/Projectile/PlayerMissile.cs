using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//導彈
public class PlayerMissile : PlayerProjectileOverdrive
{
    //[SerializeField] AudioData targetAcquiredVoice = null;
    [Header("速度設定")]
    [SerializeField] float lowSpeed = 8f;

    [SerializeField]  float highSpeed = 25f;

    [SerializeField] float varibaleSpeedDelay = 0.5f;
    [Header("爆炸特效")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData[] explosionSFX = null;
    [Header("傷害值")]
    [SerializeField] float explosionDamage = 5f;
    [Header("爆炸範圍")]
    [SerializeField] float explosionRadius = 3f;
    [Header("層級")]
    [SerializeField] LayerMask enemyLayerMask = default;

    WaitForSeconds waitVariableSpeedDelay;

    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(varibaleSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(variableSpeedCoroutine));
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        //讓導彈爆炸  爆炸特效  音效
        PoolManager.Release(explosionVFX, transform.position);
        AudioManager.Instance.PlayRandomSFX(explosionSFX);


        //範圍造成傷害   OverlapCircleAll輸入圓點，半徑內  特定層級碰到的Collider都會返回
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);

        foreach(var collider in colliders)
        {
            if(collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage *  PlayerLevel.Instance.MissilePower );
            }
        }
    }
    //編輯器專用  範圍可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    IEnumerator variableSpeedCoroutine()
    {
        moveSpeed = lowSpeed;

        yield return waitVariableSpeedDelay;

        moveSpeed = highSpeed;
    }
}
