using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class EnemyBoss2 : Enemy
{
    [Header("移動")]
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startTargePosition;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float minPosX;
    [SerializeField] float maxPosX;

    [Header("攻擊模式")]
    [SerializeField] int attackType = 0;  //0  1 發射子彈   2 觸手  3衝撞
    [SerializeField]  bool CanAttack = false;
    [SerializeField]  bool CanMove = true;
    [SerializeField] bool IsDead = false;


    [Header("移動速度")]
    [SerializeField] float moveSpeed = 2f;

    [Header("發射器")]
    [SerializeField] FireSystem[] firesystems;
    [Header("本體碰撞")]
    [SerializeField] GameObject BodyCollider;
    [SerializeField] GameObject AssaultCollider;
    [SerializeField] Animator animatior;
    [Header("觸手碰撞")]
    [SerializeField] Animator[] STanimator;
    [SerializeField] CapsuleCollider2D[] STcolliders;
    [Header("噴射")]
    [SerializeField] GameObject FireVFX;
    [SerializeField] CapsuleCollider2D FireColliders;

    [Header("聲音")]
    [SerializeField] AudioData startSound;
    [SerializeField] AudioData[] howlSonuds;
    [SerializeField] AudioData attakckSound;
    [SerializeField] AudioData FireSound;

    [Header("BOSS UI")]
    [SerializeField] Canvas HP_UI;
    [SerializeField] Material[] bossMat;
    [SerializeField] MeshRenderer[] BossRender;
    [SerializeField] Color colorEMI;

    [Header("死亡特效")]
    [SerializeField] GameObject DeathVFX;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    WaitUntil waitUntiCanAttack;
    WaitUntil waitUntiFireEnd;
    WaitUntil waitUntAssaultEnd;

    bool fireEnd;
    bool AssaultEnd;

    protected override void Awake()
    {
        base.Awake();

        waitUntiCanAttack = new WaitUntil(() => CanAttack);
        waitUntiFireEnd = new WaitUntil(() => fireEnd);
        waitUntAssaultEnd = new WaitUntil(() => AssaultEnd);
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
        transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);

        //UI打開
        HP_UI = GameManager.Instance.bossHP_UI.GetComponent<Canvas>();
        HP_UI.enabled = true;
        GameManager.Instance.bossName_UI.GetComponent<Text>().text = "利維坦";

        //關閉觸手碰撞
        for (int i = 0; i < STcolliders.Length; i++)
        {
            STcolliders[i].enabled = false;
        }

        //打開身體碰撞
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = true;

        EnemyManager.Instance.AddToList(gameObject); //加入敵人列表中

        //出場聲音
        StartCoroutine(nameof(StartAudio));

        StartCoroutine(nameof(MovingCoroutine));
        StartCoroutine(nameof(AttackCoroutine));
        StartCoroutine(nameof(UpdateMatCoroutine));
    }
    void OnDisable()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //加入敵人列表中
        StopAllCoroutines();

        for(int i = 0; i < bossMat.Length; i++)
        {
            bossMat[i].SetColor("_EmissionColor", colorEMI);
        }
        HP_UI.enabled = false;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < bossMat.Length; i++)
        {
            bossMat[i].SetColor("_EmissionColor", colorEMI);
        }
        Debug.Log("重置material");
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
        targetPosition = new Vector3(Random.Range(minPosX, maxPosX), transform.position.y);
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsDead)
        {
            if (CanMove)
            {
                //每次攻擊前移動一次
                if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);

                    CanAttack = true;
                }
                else
                {
                    targetPosition = new Vector3(Random.Range(minPosX, maxPosX), transform.position.y);
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
            if (GameManager.GameState == GameState.GameOver || IsDead) yield break;

            if (attackType == 0 || attackType == 1)
            {
                firesystems[attackType].gameObject.SetActive(true);
                firesystems[attackType].CanFire = true;
                yield return new WaitForSeconds(10);
            }
            else if(attackType == 2)
            {
                CanMove = false;

                SpikesAttack();
                yield return new WaitForSeconds(10);
            }
            else if (attackType == 3)
            {
                CanMove = false;

                AssaultEnd = false;

                AssaultAttack();
                yield return waitUntAssaultEnd;

                yield return new WaitForSeconds(4f);
            }
            else if (attackType == 4)
            {
                CanMove = false;

                fireEnd = false;

                FireMove();
                yield return new WaitForSeconds(1f);

                yield return waitUntiFireEnd;
            }

            //攻擊結束後，下次攻擊方式轉換

            CanAttack = false;

            if (attackType == 0 || attackType == 1)
            {
                AudioManager.Instance.PlayRandomSFX(howlSonuds[Random.Range(0, howlSonuds.Length)]);
                firesystems[attackType].CanFire = false;
                firesystems[attackType].gameObject.SetActive(false);
            }else if(attackType == 2)
            {
                CanMove = true;

                //關閉觸手碰撞
                for (int i = 0; i < STcolliders.Length; i++)
                {
                    STcolliders[i].enabled = false;
                }
            }
            else if (attackType == 3)
            {
                CanMove = true;

                CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
                foreach (CapsuleCollider2D bc in myColliders) bc.enabled = true;
                CapsuleCollider2D[] otColliders = AssaultCollider.GetComponents<CapsuleCollider2D>();
                foreach (CapsuleCollider2D bc in otColliders) bc.enabled = false;
            }
            else if (attackType == 4)
            {
                CanMove = true;
            }

            attackType++;

            if (attackType == 5)
            {
                attackType = 0;
            }

            yield return waitUntiCanAttack;
        }
    }

    //尖刺
    public void SpikesAttack()
    {
        StartCoroutine(nameof(SpikesAttackCoroutine));
        StartCoroutine(nameof(FollowPlayer));
    }

    IEnumerator SpikesAttackCoroutine()
    {
        int AttackNum = 0;
        int[] AttackOder = new int[] { 1, 0, 2, 3,0,2,1,3,2,1,0,3, 1, 0, 2, 3, 0, 2, 1, 3, 2, 1, 0, 3 };
        while (AttackNum < 20 && GameManager.GameState != GameState.GameOver && !IsDead)
        {
            STcolliders[AttackOder[AttackNum]].enabled = true;
            STanimator[AttackOder[AttackNum]].SetTrigger("Attack");
            AttackNum++;
            yield return new WaitForSeconds(0.5f);
        }
        StopCoroutine(nameof(FollowPlayer));
    }
    IEnumerator FollowPlayer()
    {
        //鎖定玩家位置
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsDead)
        {
            //鎖定玩家位置
            targetPosition = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y);
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //移動到目標點
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * 1.2f * Time.fixedDeltaTime);
            }
            yield return waitForFixedUpdate;
        }
    }

    //衝撞
    public void AssaultAttack()
    {
        StartCoroutine(nameof(AssaultAttackCoroutine));
    }

    IEnumerator AssaultAttackCoroutine()
    {
        bool IsOver = false;
        float FollowTime = 3f;
        //鎖定玩家位置
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsOver && !IsDead)
        {
            //鎖定玩家位置
            targetPosition = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y);
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //移動到目標點
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed *  1.2f * Time.fixedDeltaTime);
            }
            FollowTime -= Time.fixedDeltaTime;
            if (FollowTime < 0) IsOver = true;
             yield return waitForFixedUpdate;
        }

        //開始衝撞
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = false;
        CapsuleCollider2D[] otColliders = AssaultCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in otColliders) bc.enabled = true;

        AudioManager.Instance.PlayRandomSFX(attakckSound);
        animatior.SetTrigger("AssaultAttack");

        AssaultEnd = true;
    }

    //發射光束
    public void FireMove()
    {
        StartCoroutine(nameof(FireMoveCoroutine));
    }

    IEnumerator FireMoveCoroutine()
    {
        //從最右邊開始
        targetPosition = new Vector3(maxPosX-0.1f, transform.position.y);

        bool IsOver = false;
        //到達發射位置
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsOver && !IsDead)
        {
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //移動到目標點
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                IsOver = true;
            }
            yield return waitForFixedUpdate;
        }

        targetPosition = new Vector3(minPosX, transform.position.y);

        IsOver = false;

        FireVFX.GetComponent<ParticleSystem>().Play();
        FireColliders.enabled = true;

        AudioManager.Instance.PlayRandomSFX(FireSound);
        animatior.SetBool("FireAttack",true);

        //開始震動
        CameraShakeInstance shake =  CameraShaker.Instance.StartShake(3f, 3f, 1f);

        //到達發射結束位置
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsOver)
        {
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //移動到目標點
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                IsOver = true;
            }
            yield return waitForFixedUpdate;
        }

        FireVFX.GetComponent<ParticleSystem>().Stop();

        fireEnd = true;

        FireColliders.enabled = false;

        animatior.SetBool("FireAttack", false);

        //結束震動
        shake.StartFadeOut(1f);
    }


    //沿著固定方向移動
    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);


    IEnumerator UpdateMatCoroutine()
    {
        float nowEMI = 2f;
        float add = 0.1f;

        while (gameObject.activeSelf)
        {
            if (nowEMI >= 3.5f)
            {
                add = -0.15f;
            }
            else if(nowEMI <= 0.5f)
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
