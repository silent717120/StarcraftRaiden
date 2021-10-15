using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    //取得一個隨機敵人，清單內數量不等於0時可取得 否則是null
    public GameObject RandomEnemy =>enemyList.Count ==  0 ? null : enemyList[Random.Range(0, enemyList.Count)];

    [Header("敵人預製物")]
    [SerializeField] public int[] ememyNum;
    [SerializeField]public GameObject[] ememyPrefabs;

    [Header("生成等待間隔")]
    [SerializeField]public float timeBetweenSpawns = 1f;

    [Header("最小最大數量敵人")]
    [SerializeField]public int minEnemyAmount = 4;
    [SerializeField]public int maxEnemyAmount = 10;

    //該波數須生成的數量
    int enemyAmount;
    //生成用來監控的敵人列表(死亡後從列表移除)
    public List<GameObject> enemyList;

    //生成等待間隔時間
    WaitForSeconds waitTimeBetweenSpwans;

    protected override void Awake()
    {
        base.Awake();
        enemyList = new List<GameObject>();
        SetBetweenTime(timeBetweenSpawns);
    }

    public void StartCreatEnemy()
    {
        StartCoroutine(nameof(FixedSpawnCoroutine));
    }

    public void StartRandomCreatEnemy()
    {
        StartCoroutine(nameof(RandomlySpawnCoroutine));
    }

    //固定生成
    IEnumerator FixedSpawnCoroutine()
    {
        for(int i = 0;i < ememyNum.Length; i++)
        {
            for(int j = 0; j < ememyNum[i]; j++)
            {
                if (GameManager.GameState == GameState.GameOver) yield break;

                PoolManager.Release(ememyPrefabs[i]);

                yield return waitTimeBetweenSpwans;
            }
        }

        //等怪物都確定產生結束才可關閉
        GameManager.Instance.IsSpawnEnd = true;
    }

    //隨機生成
    IEnumerator RandomlySpawnCoroutine()
    {
        enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount, maxEnemyAmount);

        for (int i = 0; i < enemyAmount; i++)
        {
            if (GameManager.GameState == GameState.GameOver) yield break;

            var enemy = ememyPrefabs[Random.Range(0, ememyPrefabs.Length)];
            PoolManager.Release(enemy);
            yield return waitTimeBetweenSpwans;
        }

        //等怪物都確定產生結束才可關閉
        GameManager.Instance.IsSpawnEnd = true;
    }

    //另外創造一名敵人
    public void CreateSpecialEnemy(GameObject specialEnemy)
    {
        PoolManager.Release(specialEnemy);
    }

    //加入在場上列表的敵人
    public void AddToList(GameObject enemy) => enemyList.Add(enemy);

    //移除不在場上列表的敵人
    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);

    //設定間隔時間
    public void SetBetweenTime(float BetweenTime) => waitTimeBetweenSpwans = new WaitForSeconds(BetweenTime);

    //移除不在場上列表的敵人
    public void RemoveAllFromList()
    {
        for(int i = enemyList.Count -1; i >= 0; i--)
        {
            enemyList[i].SetActive(false);
        }
    }

    //直接停止生成
    public void StopCreat()
    {
        StopAllCoroutines();
        GameManager.Instance.IsSpawnEnd = true;
    }
}
