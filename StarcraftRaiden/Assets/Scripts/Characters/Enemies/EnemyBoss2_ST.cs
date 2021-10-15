using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class EnemyBoss2_ST : MonoBehaviour
{
    [Header("是否為持續傷害")]
    [SerializeField] bool IsKeep = false;
    [Header("傷害值")]
    [SerializeField] int damage = 20;

    [Header("隔多久傷害一次")]
    [SerializeField] float damageTime;

    [Header("命中音效")]
    [SerializeField] AudioData[] hitSFX;


    float damageCurrTime = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsKeep) return;

        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            Debug.Log("受到尖刺傷害:"+damage);
            player.TakeDamage(damage);
            this.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsKeep) return;

        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            Debug.Log("被噴到");
            damageCurrTime += Time.fixedDeltaTime;
            if (damageCurrTime >= damageTime)
            {
                damageCurrTime = 0;
                //受傷
                character.TakeDamage(damage);
                //音效
                AudioManager.Instance.PlayRandomSFX(hitSFX);
            }
        }
    }

    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
    }
}
