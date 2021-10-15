using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class EnemyBoss2 : Enemy
{
    [Header("����")]
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startTargePosition;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float minPosX;
    [SerializeField] float maxPosX;

    [Header("�����Ҧ�")]
    [SerializeField] int attackType = 0;  //0  1 �o�g�l�u   2 Ĳ��  3�ļ�
    [SerializeField]  bool CanAttack = false;
    [SerializeField]  bool CanMove = true;
    [SerializeField] bool IsDead = false;


    [Header("���ʳt��")]
    [SerializeField] float moveSpeed = 2f;

    [Header("�o�g��")]
    [SerializeField] FireSystem[] firesystems;
    [Header("����I��")]
    [SerializeField] GameObject BodyCollider;
    [SerializeField] GameObject AssaultCollider;
    [SerializeField] Animator animatior;
    [Header("Ĳ��I��")]
    [SerializeField] Animator[] STanimator;
    [SerializeField] CapsuleCollider2D[] STcolliders;
    [Header("�Q�g")]
    [SerializeField] GameObject FireVFX;
    [SerializeField] CapsuleCollider2D FireColliders;

    [Header("�n��")]
    [SerializeField] AudioData startSound;
    [SerializeField] AudioData[] howlSonuds;
    [SerializeField] AudioData attakckSound;
    [SerializeField] AudioData FireSound;

    [Header("BOSS UI")]
    [SerializeField] Canvas HP_UI;
    [SerializeField] Material[] bossMat;
    [SerializeField] MeshRenderer[] BossRender;
    [SerializeField] Color colorEMI;

    [Header("���`�S��")]
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

        //���`�S������
        DeathVFX.SetActive(false);

        //�ʧ@���m
        animatior.SetTrigger("Start");

        //�����k0
        transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);

        //UI���}
        HP_UI = GameManager.Instance.bossHP_UI.GetComponent<Canvas>();
        HP_UI.enabled = true;
        GameManager.Instance.bossName_UI.GetComponent<Text>().text = "�Q���Z";

        //����Ĳ��I��
        for (int i = 0; i < STcolliders.Length; i++)
        {
            STcolliders[i].enabled = false;
        }

        //���}����I��
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = true;

        EnemyManager.Instance.AddToList(gameObject); //�[�J�ĤH�C��

        //�X���n��
        StartCoroutine(nameof(StartAudio));

        StartCoroutine(nameof(MovingCoroutine));
        StartCoroutine(nameof(AttackCoroutine));
        StartCoroutine(nameof(UpdateMatCoroutine));
    }
    void OnDisable()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //�[�J�ĤH�C��
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
        Debug.Log("���mmaterial");
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
        targetPosition = new Vector3(Random.Range(minPosX, maxPosX), transform.position.y);
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsDead)
        {
            if (CanMove)
            {
                //�C�������e���ʤ@��
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

    //�����Ҧ�
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

            //����������A�U�������覡�ഫ

            CanAttack = false;

            if (attackType == 0 || attackType == 1)
            {
                AudioManager.Instance.PlayRandomSFX(howlSonuds[Random.Range(0, howlSonuds.Length)]);
                firesystems[attackType].CanFire = false;
                firesystems[attackType].gameObject.SetActive(false);
            }else if(attackType == 2)
            {
                CanMove = true;

                //����Ĳ��I��
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

    //�y��
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
        //��w���a��m
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsDead)
        {
            //��w���a��m
            targetPosition = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y);
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //���ʨ�ؼ��I
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * 1.2f * Time.fixedDeltaTime);
            }
            yield return waitForFixedUpdate;
        }
    }

    //�ļ�
    public void AssaultAttack()
    {
        StartCoroutine(nameof(AssaultAttackCoroutine));
    }

    IEnumerator AssaultAttackCoroutine()
    {
        bool IsOver = false;
        float FollowTime = 3f;
        //��w���a��m
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsOver && !IsDead)
        {
            //��w���a��m
            targetPosition = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y);
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //���ʨ�ؼ��I
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed *  1.2f * Time.fixedDeltaTime);
            }
            FollowTime -= Time.fixedDeltaTime;
            if (FollowTime < 0) IsOver = true;
             yield return waitForFixedUpdate;
        }

        //�}�l�ļ�
        CapsuleCollider2D[] myColliders = BodyCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in myColliders) bc.enabled = false;
        CapsuleCollider2D[] otColliders = AssaultCollider.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D bc in otColliders) bc.enabled = true;

        AudioManager.Instance.PlayRandomSFX(attakckSound);
        animatior.SetTrigger("AssaultAttack");

        AssaultEnd = true;
    }

    //�o�g����
    public void FireMove()
    {
        StartCoroutine(nameof(FireMoveCoroutine));
    }

    IEnumerator FireMoveCoroutine()
    {
        //�q�̥k��}�l
        targetPosition = new Vector3(maxPosX-0.1f, transform.position.y);

        bool IsOver = false;
        //��F�o�g��m
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsOver && !IsDead)
        {
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //���ʨ�ؼ��I
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

        //�}�l�_��
        CameraShakeInstance shake =  CameraShaker.Instance.StartShake(3f, 3f, 1f);

        //��F�o�g������m
        while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver && !IsOver)
        {
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //���ʨ�ؼ��I
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

        //�����_��
        shake.StartFadeOut(1f);
    }


    //�u�۩T�w��V����
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
