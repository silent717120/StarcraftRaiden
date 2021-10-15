using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile_AOE : MonoBehaviour
{
    [Header("命中特效")]
    [SerializeField] GameObject hitVFX;
    [Header("命中音效")]
    [SerializeField] AudioData[] hitSFX;
    [Header("傷害值")]
    [SerializeField] float damage;
    [Header("移動速度")]
    [SerializeField] float moveSpeed = 5;
    [Header("隔多久傷害一次")]
    [SerializeField] float damageTime;
    [Header("持續時間")]
    [SerializeField] float keepTime;
    [Header("子彈模型")]
    [SerializeField] GameObject model;
    [Header("毒物特效")]
    [SerializeField] GameObject AOE_VFX;

    float nowTime = 0; //計算產生的時間
    float damageCurrTime = 0;

    bool IsArrive = false; //到達爆炸點

    CircleCollider2D colliderT;

    Vector3 targetPosition; //目標


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

    //發射後移動
    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            //移動到爆炸點
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
                //爆炸持續時間
                nowTime += Time.fixedDeltaTime;
                if (nowTime > keepTime)
                {
                    gameObject.SetActive(false);
                }
            }
            //碰撞變大
            if (colliderT.radius < 1.5f)
            {
                colliderT.radius += 0.01f;
            }
            yield return waitForFixedUpdate;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        //TryGetComponent測試是否取得特定Component
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            damageCurrTime += Time.fixedDeltaTime;
            if ( damageCurrTime >= damageTime)
            {
                damageCurrTime = 0;
                //受傷
                character.TakeDamage(damage * GameManager.Instance.EnemyPower);
                //音效
                AudioManager.Instance.PlayRandomSFX(hitSFX);
            }
        }
    }

    //設定目標
    protected void SetTarget(GameObject target)
    {
        this.targetPosition = target.transform.position;
    }
}
