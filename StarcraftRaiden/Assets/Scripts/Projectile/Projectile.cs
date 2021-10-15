using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("命中特效")]
    [SerializeField] GameObject hitVFX;
    [Header("命中音效")]
    [SerializeField] AudioData[] hitSFX;
    [Header("傷害值")]
    [SerializeField] public float damage;
    [Header("移動速度")]
    [SerializeField] public float moveSpeed = 10;

    [Header("移動方向")]
    [SerializeField] public Vector2 moveDirection; //protected 可繼承

    protected GameObject target; //目標

    //開啟時就移動
    protected virtual void OnEnable()
    {
        StartCoroutine(MoveDirectly());
    }

    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            Move();

            yield return null;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //TryGetComponent測試是否取得特定Component
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            if(character.gameObject.tag == "Player")
            {
                Debug.Log("玩家受傷");
                character.TakeDamage(damage * GameManager.Instance.EnemyPower);
            }
            else
            {
                Debug.Log("蟲族受傷");
                character.TakeDamage(damage);
            }

            //GetContact 兩個碰撞體的第一個接觸點
            var contactPoint = collision.GetContact(0);
            PoolManager.Release(hitVFX, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));

            //音效
            if (!character.IsNoHurt)
            {
                AudioManager.Instance.PlayRandomSFX(hitSFX);
            }

            gameObject.SetActive(false);
        }
    }

    //設定目標
    protected void SetTarget(GameObject target)
    {
        this.target = target;
    }

    //沿著固定方向移動
    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
}
