using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    //���o�@���H���ĤH�A�M�椺�ƶq������0�ɥi���o �_�h�Onull
    public GameObject RandomEnemy =>enemyList.Count ==  0 ? null : enemyList[Random.Range(0, enemyList.Count)];

    [Header("�ĤH�w�s��")]
    [SerializeField] public int[] ememyNum;
    [SerializeField]public GameObject[] ememyPrefabs;

    [Header("�ͦ����ݶ��j")]
    [SerializeField]public float timeBetweenSpawns = 1f;

    [Header("�̤p�̤j�ƶq�ĤH")]
    [SerializeField]public int minEnemyAmount = 4;
    [SerializeField]public int maxEnemyAmount = 10;

    //�Ӫi�ƶ��ͦ����ƶq
    int enemyAmount;
    //�ͦ��ΨӺʱ����ĤH�C��(���`��q�C����)
    public List<GameObject> enemyList;

    //�ͦ����ݶ��j�ɶ�
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

    //�T�w�ͦ�
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

        //���Ǫ����T�w���͵����~�i����
        GameManager.Instance.IsSpawnEnd = true;
    }

    //�H���ͦ�
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

        //���Ǫ����T�w���͵����~�i����
        GameManager.Instance.IsSpawnEnd = true;
    }

    //�t�~�гy�@�W�ĤH
    public void CreateSpecialEnemy(GameObject specialEnemy)
    {
        PoolManager.Release(specialEnemy);
    }

    //�[�J�b���W�C���ĤH
    public void AddToList(GameObject enemy) => enemyList.Add(enemy);

    //�������b���W�C���ĤH
    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);

    //�]�w���j�ɶ�
    public void SetBetweenTime(float BetweenTime) => waitTimeBetweenSpwans = new WaitForSeconds(BetweenTime);

    //�������b���W�C���ĤH
    public void RemoveAllFromList()
    {
        for(int i = enemyList.Count -1; i >= 0; i--)
        {
            enemyList[i].SetActive(false);
        }
    }

    //��������ͦ�
    public void StopCreat()
    {
        StopAllCoroutines();
        GameManager.Instance.IsSpawnEnd = true;
    }
}
