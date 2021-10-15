using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Header("得分")]
    [SerializeField] protected int scorePoint = 100;
    [Header("經驗值")]
    [SerializeField] protected int exp = 40;
    [Header("是否碰撞死亡")]
    [SerializeField] bool CanCollision =true;
    [Header("碰撞玩家傷害")]
    [SerializeField] int CollisionDamage = 20;
    [Header("死亡出現的道具")]
    [SerializeField] GameObject[] DropItems;
    [Header("掉落機率")]
    [SerializeField] protected float DropProbability = 0.1f;
    [Header("死亡獲得的能量")]
    [SerializeField] protected int deathEnergyBonus = 3;
    [Header("是否可獲得資源")]
    [SerializeField] protected bool CanGetAward = true;

    protected bool IsCollision = false; //碰撞到玩家死亡

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
            ScoreManager.Instance.AddScore(scorePoint); //或得分數
            PlayerEnergy.Instance.Obtain(deathEnergyBonus); //玩家獲得能量
            PlayerLevel.Instance.GetExp(exp); //獲得經驗值
            GameLog.Instance.KillNum++; //殺敵數增加
        }
        EnemyManager.Instance.RemoveFromList(gameObject); //從敵人列表中移除
        base.Die();
    }

    public void Exit()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //從敵人列表中移除
        gameObject.SetActive(false);
    }
}
