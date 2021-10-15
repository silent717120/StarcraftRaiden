using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class EnemyBoss2_ST : MonoBehaviour
{
    [Header("�O�_������ˮ`")]
    [SerializeField] bool IsKeep = false;
    [Header("�ˮ`��")]
    [SerializeField] int damage = 20;

    [Header("�j�h�[�ˮ`�@��")]
    [SerializeField] float damageTime;

    [Header("�R������")]
    [SerializeField] AudioData[] hitSFX;


    float damageCurrTime = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsKeep) return;

        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            Debug.Log("����y��ˮ`:"+damage);
            player.TakeDamage(damage);
            this.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsKeep) return;

        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            Debug.Log("�Q�Q��");
            damageCurrTime += Time.fixedDeltaTime;
            if (damageCurrTime >= damageTime)
            {
                damageCurrTime = 0;
                //����
                character.TakeDamage(damage);
                //����
                AudioManager.Instance.PlayRandomSFX(hitSFX);
            }
        }
    }

    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
    }
}
