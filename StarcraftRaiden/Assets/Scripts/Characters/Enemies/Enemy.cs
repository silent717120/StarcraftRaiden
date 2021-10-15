using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Header("�o��")]
    [SerializeField] protected int scorePoint = 100;
    [Header("�g���")]
    [SerializeField] protected int exp = 40;
    [Header("�O�_�I�����`")]
    [SerializeField] bool CanCollision =true;
    [Header("�I�����a�ˮ`")]
    [SerializeField] int CollisionDamage = 20;
    [Header("���`�X�{���D��")]
    [SerializeField] GameObject[] DropItems;
    [Header("�������v")]
    [SerializeField] protected float DropProbability = 0.1f;
    [Header("���`��o����q")]
    [SerializeField] protected int deathEnergyBonus = 3;
    [Header("�O�_�i��o�귽")]
    [SerializeField] protected bool CanGetAward = true;

    protected bool IsCollision = false; //�I���쪱�a���`

    EnemyExplode explode;

    protected virtual void Awake()
    {
        explode = GetComponent<EnemyExplode>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!CanCollision) return;
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            CanGetAward = false;
            IsCollision = true;
            player.TakeDamage(CollisionDamage);
            Die();
        }
    }

    public override void Die()
    {
        if(explode != null && !IsCollision) explode.FourDirExplode();
        if (CanGetAward)
        {
            float Pro = Random.Range(0,1f);
            int Type = Random.Range(0, DropItems.Length);
            if(Pro <= DropProbability) PoolManager.Release(DropItems[Type], transform.position);
            ScoreManager.Instance.AddScore(scorePoint); //�αo����
            PlayerEnergy.Instance.Obtain(deathEnergyBonus); //���a��o��q
            PlayerLevel.Instance.GetExp(exp); //��o�g���
            GameLog.Instance.KillNum++; //���ļƼW�[
        }
        EnemyManager.Instance.RemoveFromList(gameObject); //�q�ĤH�C������
        base.Die();
    }

    public void Exit()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //�q�ĤH�C������
        gameObject.SetActive(false);
    }
}
