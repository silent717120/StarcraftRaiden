using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile_AOE : MonoBehaviour
{
    [Header("�R���S��")]
    [SerializeField] GameObject hitVFX;
    [Header("�R������")]
    [SerializeField] AudioData[] hitSFX;
    [Header("�ˮ`��")]
    [SerializeField] float damage;
    [Header("���ʳt��")]
    [SerializeField] float moveSpeed = 5;
    [Header("�j�h�[�ˮ`�@��")]
    [SerializeField] float damageTime;
    [Header("����ɶ�")]
    [SerializeField] float keepTime;
    [Header("�l�u�ҫ�")]
    [SerializeField] GameObject model;
    [Header("�r���S��")]
    [SerializeField] GameObject AOE_VFX;

    float nowTime = 0; //�p�ⲣ�ͪ��ɶ�
    float damageCurrTime = 0;

    bool IsArrive = false; //��F�z���I

    CircleCollider2D colliderT;

    Vector3 targetPosition; //�ؼ�


    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        colliderT = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        damageCurrTime = damageTime;
        nowTime = 0;
        colliderT.radius = 0;
        model.SetActive(true);
        IsArrive = false;

        SetTarget(GameObject.FindGameObjectWithTag("Player"));
        StartCoroutine(MoveDirectly());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    //�o�g�Ჾ��
    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            //���ʨ��z���I
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime && !IsArrive)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (!IsArrive)
                {
                    IsArrive = true;
                    model.SetActive(false);
                    AOE_VFX.SetActive(true);
                }
                //�z������ɶ�
                nowTime += Time.fixedDeltaTime;
                if (nowTime > keepTime)
                {
                    gameObject.SetActive(false);
                }
            }
            //�I���ܤj
            if (colliderT.radius < 1.5f)
            {
                colliderT.radius += 0.01f;
            }
            yield return waitForFixedUpdate;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        //TryGetComponent���լO�_���o�S�wComponent
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            damageCurrTime += Time.fixedDeltaTime;
            if ( damageCurrTime >= damageTime)
            {
                damageCurrTime = 0;
                //����
                character.TakeDamage(damage * GameManager.Instance.EnemyPower);
                //����
                AudioManager.Instance.PlayRandomSFX(hitSFX);
            }
        }
    }

    //�]�w�ؼ�
    protected void SetTarget(GameObject target)
    {
        this.targetPosition = target.transform.position;
    }
}
