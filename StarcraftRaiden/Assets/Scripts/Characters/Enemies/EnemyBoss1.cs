using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoss1 : Enemy
{
    [Header("����")]
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startTargePosition;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float minPosX;
     [SerializeField] float maxPosX;

    [Header("�����Ҧ�")]
    [SerializeField] int attackType;
    bool CanAttack = false;
    [SerializeField] bool IsDead = false;

    [Header("���ʳt��")]
    [SerializeField] float moveSpeed = 2f;

    [Header("�o�g��")]
    [SerializeField] FireSystem[] firesystems;

    [Header("����I��")]
    [SerializeField] GameObject BodyCollider;
    [SerializeField] Animator animatior;

    [Header("BOSS UI")]
    [SerializeField] Canvas HP_UI;
    [SerializeField] Material[] bossMat;
    [SerializeField] MeshRenderer[] BossRender;
    [SerializeField] Color colorEMI;

    [Header("�q�s�y��")]
    [SerializeField] AudioData startSound;
    [SerializeField] AudioData[] howlSonuds;
    [SerializeField] float minHowlTime;
    [SerializeField] float maxHowlTime;

    [Header("���`�S��")]
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

        //���`�S������
        DeathVFX.SetActive(false);

        //�ʧ@���m
        animatior.SetTrigger("Start");

        //�����k0
        transform.GetChild(0).localEulerAngles = new Vector3(0,0,0);

        //���}����I��
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = true;

        //UI���}
        HP_UI = GameManager.Instance.bossHP_UI.GetComponent<Canvas>();
        HP_UI.enabled = true;
        GameManager.Instance.bossName_UI.GetComponent<Text>().text = "�H�ͤ���";

        EnemyManager.Instance.AddToList(gameObject); //�[�J�ĤH�C��

        //�X���n��
        StartCoroutine(nameof(StartAudio));

        StartCoroutine(nameof(MovingCoroutine));
        StartCoroutine(nameof(AttackCoroutine));
        StartCoroutine(nameof(HowlCoroutine));
        StartCoroutine(nameof(UpdateMatCoroutine));
    }
    void OnDisable()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //�[�J�ĤH�C��
        StopAllCoroutines();

        HP_UI.enabled = false;
    }

    public override void Die()
    {
        if (IsDead) return;
        HP_UI.enabled = false;
        DeathVFX.SetActive(true);
        animatior.SetTrigger("Dead");
        ScoreManager.Instance.AddScore(scorePoint); //�αo����
        PlayerEnergy.Instance.Obtain(deathEnergyBonus); //���a��o��q
        PlayerLevel.Instance.GetExp(exp); //��o�g���
        GameLog.Instance.KillNum++; //���ļƼW�[
        EnemyManager.Instance.RemoveFromList(gameObject); //�q�ĤH�C������

        firesystems[0].CanFire = false;
        firesystems[1].CanFire = false;

        //�I�������A�_�h�|�@��Ĳ�o���`
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = false;

        IsDead = true;

        health = 0f;
        Debug.Log(1111111111);
        AudioManager.Instance.PlayRandomSFX(deathSFX);

        //�����ͦ���
        if (GameManager.Instance.WaveNumber != 30) //�u��30�i�P�ɨⰦboss�ݭn�ѨM
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

    //�}�l����
    IEnumerator MovingCoroutine()
    {
        if (GameManager.GameState == GameState.GameOver) yield break;

        //�ͦ�
        transform.position = startPosition;
        targetPosition = startTargePosition;

        bool IsStartOver = false;
        //�X��
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsStartOver)
        {
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //���ʨ�ؼ��I
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                IsStartOver = true;
            }
            yield return waitForFixedUpdate;
        }

        //�}�l���k����
        targetPosition = new Vector3(Random.Range(minPosX,maxPosX),transform.position.y);
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsDead)
        {
            //�C�������e���ʤ@��
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                //�ݦV���a
                LookAtPlayer();
                //�o�ʧ���
                if (!CanAttack)
                {
                    CanAttack = true;
                }
            }
            yield return waitForFixedUpdate;
        }
    }

    //�����Ҧ�
    IEnumerator AttackCoroutine()
    {
        yield return waitUntiCanAttack;

        while (gameObject.activeSelf)
        {
            if (GameManager.GameState == GameState.GameOver && !IsDead) yield break;

            firesystems[attackType].gameObject.SetActive(true);
            firesystems[attackType].CanFire = true;

            //����������A�~�򲾰ʡA�U�������覡�ഫ
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

    //�q�s
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

    //�u�۩T�w��V����
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
