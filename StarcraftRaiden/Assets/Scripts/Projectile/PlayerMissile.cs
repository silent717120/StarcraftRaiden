using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ɼu
public class PlayerMissile : PlayerProjectileOverdrive
{
    //[SerializeField] AudioData targetAcquiredVoice = null;
    [Header("�t�׳]�w")]
    [SerializeField] float lowSpeed = 8f;

    [SerializeField]  float highSpeed = 25f;

    [SerializeField] float varibaleSpeedDelay = 0.5f;
    [Header("�z���S��")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData[] explosionSFX = null;
    [Header("�ˮ`��")]
    [SerializeField] float explosionDamage = 5f;
    [Header("�z���d��")]
    [SerializeField] float explosionRadius = 3f;
    [Header("�h��")]
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
        //���ɼu�z��  �z���S��  ����
        PoolManager.Release(explosionVFX, transform.position);
        AudioManager.Instance.PlayRandomSFX(explosionSFX);


        //�d��y���ˮ`   OverlapCircleAll��J���I�A�b�|��  �S�w�h�ŸI�쪺Collider���|��^
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);

        foreach(var collider in colliders)
        {
            if(collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage *  PlayerLevel.Instance.MissilePower );
            }
        }
    }
    //�s�边�M��  �d��i����
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
