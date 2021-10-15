using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoss1 : Enemy
{
    [Header("移動")]
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startTargePosition;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float minPosX;
     [SerializeField] float maxPosX;

    [Header("攻擊模式")]
    [SerializeField] int attackType;
    bool CanAttack = false;
    [SerializeField] bool IsDead = false;

    [Header("移動速度")]
    [SerializeField] float moveSpeed = 2f;

    [Header("發射器")]
    [SerializeField] FireSystem[] firesystems;

    [Header("本體碰撞")]
    [SerializeField] GameObject BodyCollider;
    [SerializeField] Animator animatior;

    [Header("BOSS UI")]
    [SerializeField] Canvas HP_UI;
    [SerializeField] Material[] bossMat;
    [SerializeField] MeshRenderer[] BossRender;
    [SerializeField] Color colorEMI;

    [Header("吼叫語音")]
    [SerializeField] AudioData startSound;
    [SerializeField] AudioData[] howlSonuds;
    [SerializeField] float minHowlTime;
    [SerializeField] float maxHowlTime;

    [Header("死亡特效")]
    [SerializeField] GameObject DeathVFX;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    WaitUntil waitUntiCanAttack;

    protected override void Awake()
    {
        base.Awake();

        waitUntiCanAttack = new WaitUntil(() => CanAttack);
    }

    protected override void OnEnable()
    {
        onHeadHealthBar = GameManager.Instance.bossHP_UI.GetComponent<StateBar>();
        base.OnEnable();
        CanGetAward = true;
        IsCollision = false;
        IsDead = false;

        //死亡特效關閉
        DeathVFX.SetActive(false);

        //動作重置
        animatior.SetTrigger("Start");

        //角度歸0
        transform.GetChild(0).localEulerAngles = new Vector3(0,0,0);

        //打開身體碰撞
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = true;

        //UI打開
        HP_UI = GameManager.Instance.bossHP_UI.GetComponent<Canvas>();
        HP_UI.enabled = true;
        GameManager.Instance.bossName_UI.GetComponent<Text>().text = "寄生王蟲";

        EnemyManager.Instance.AddToList(gameObject); //加入敵人列表中

        //出場聲音
        StartCoroutine(nameof(StartAudio));

        StartCoroutine(nameof(MovingCoroutine));
        StartCoroutine(nameof(AttackCoroutine));
        StartCoroutine(nameof(HowlCoroutine));
        StartCoroutine(nameof(UpdateMatCoroutine));
    }
    void OnDisable()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //加入敵人列表中
        StopAllCoroutines();

        HP_UI.enabled = false;
    }

    public override void Die()
    {
        if (IsDead) return;
        HP_UI.enabled = false;
        DeathVFX.SetActive(true);
        animatior.SetTrigger("Dead");
        ScoreManager.Instance.AddScore(scorePoint); //或得分數
        PlayerEnergy.Instance.Obtain(deathEnergyBonus); //玩家獲得能量
        PlayerLevel.Instance.GetExp(exp); //獲得經驗值
        GameLog.Instance.KillNum++; //殺敵數增加
        EnemyManager.Instance.RemoveFromList(gameObject); //從敵人列表中移除

        firesystems[0].CanFire = false;
        firesystems[1].CanFire = false;

        //碰撞關閉，否則會一直觸發死亡
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = false;

        IsDead = true;

        health = 0f;
        Debug.Log(1111111111);
        AudioManager.Instance.PlayRandomSFX(deathSFX);

        //關閉生成器
        if (GameManager.Instance.WaveNumber != 30) //只有30波同時兩隻boss需要解決
        {
            EnemyManager.Instance.StopCreat();
            EnemyManager.Instance.RemoveAllFromList();
        }

        StartCoroutine(nameof(DeadOverCoroutine));
    }
    IEnumerator DeadOverCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);

        EnemyManager.Instance.RemoveAllFromList();
    }

    IEnumerator StartAudio()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayRandomSFX(startSound);
    }

    //開始移動
    IEnumerator MovingCoroutine()
    {
        if (GameManager.GameState == GameState.GameOver) yield break;

        //生成
        transform.position = startPosition;
        targetPosition = startTargePosition;

        bool IsStartOver = false;
        //出場
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsStartOver)
        {
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //移動到目標點
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                IsStartOver = true;
            }
            yield return waitForFixedUpdate;
        }

        //開始左右移動
        targetPosition = new Vector3(Random.Range(minPosX,maxPosX),transform.position.y);
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsDead)
        {
            //每次攻擊前移動一次
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                //看向玩家
                LookAtPlayer();
                //發動攻擊
                if (!CanAttack)
                {
                    CanAttack = true;
                }
            }
            yield return waitForFixedUpdate;
        }
    }

    //攻擊模式
    IEnumerator AttackCoroutine()
    {
        yield return waitUntiCanAttack;

        while (gameObject.activeSelf)
        {
            if (GameManager.GameState == GameState.GameOver && !IsDead) yield break;

            firesystems[attackType].gameObject.SetActive(true);
            firesystems[attackType].CanFire = true;

            //攻擊結束後，繼續移動，下次攻擊方式轉換
            yield return new  WaitForSeconds(10);

            CanAttack = false;

            firesystems[attackType].CanFire = false;
            firesystems[attackType].gameObject.SetActive(false);

            attackType++;
            if(attackType == 3)
            {
                attackType = 0;
            }

            targetPosition = new Vector3(Random.Range(minPosX, maxPosX), transform.position.y);

            yield return waitUntiCanAttack;
        }
    }

    //吼叫
    IEnumerator HowlCoroutine()
    {
        while (gameObject.activeSelf)
        {
            float HowlTime = Random.Range(minHowlTime, maxHowlTime);
            yield return new WaitForSeconds(HowlTime);
            if (GameManager.GameState == GameState.GameOver && !IsDead) yield break;

            AudioManager.Instance.PlayRandomSFX(howlSonuds[Random.Range(0,howlSonuds.Length)]);
        }
    }

    //沿著固定方向移動
    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

    public void LookAtPlayer()
    {
        moveDirection = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.FromToRotation(Vector2.down, moveDirection), 0.5f * Time.fixedDeltaTime);
    }

    IEnumerator UpdateMatCoroutine()
    {
        float nowEMI = 1f;
        float add = 0.1f;

        while (gameObject.activeSelf)
        {
            if (nowEMI >= 3f)
            {
                add = -0.15f;
            }
            else if (nowEMI <= 0.5f)
            {
                add = 0.1f;
            }
            nowEMI += add;

            for (int i = 0; i < bossMat.Length; i++)
            {
                bossMat[i].SetColor("_EmissionColor", colorEMI * nowEMI);
                BossRender[i].material = bossMat[i];
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
