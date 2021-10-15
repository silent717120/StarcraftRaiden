using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("�R���S��")]
    [SerializeField] GameObject hitVFX;
    [Header("�R������")]
    [SerializeField] AudioData[] hitSFX;
    [Header("�ˮ`��")]
    [SerializeField] public float damage;
    [Header("���ʳt��")]
    [SerializeField] public float moveSpeed = 10;

    [Header("���ʤ�V")]
    [SerializeField] public Vector2 moveDirection; //protected �i�~��

    protected GameObject target; //�ؼ�

    //�}�ҮɴN����
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
        //TryGetComponent���լO�_���o�S�wComponent
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            if(character.gameObject.tag == "Player")
            {
                Debug.Log("���a����");
                character.TakeDamage(damage * GameManager.Instance.EnemyPower);
            }
            else
            {
                Debug.Log("�αڨ���");
                character.TakeDamage(damage);
            }

            //GetContact ��ӸI���骺�Ĥ@�ӱ�Ĳ�I
            var contactPoint = collision.GetContact(0);
            PoolManager.Release(hitVFX, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));

            //����
            if (!character.IsNoHurt)
            {
                AudioManager.Instance.PlayRandomSFX(hitSFX);
            }

            gameObject.SetActive(false);
        }
    }

    //�]�w�ؼ�
    protected void SetTarget(GameObject target)
    {
        this.target = target;
    }

    //�u�۩T�w��V����
    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
}
