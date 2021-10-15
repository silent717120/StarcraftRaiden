using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZerg : Enemy
{
    //�T�w��V  �H����V  ���ʫᰱ��  ,�ʵe����
    enum MoveType { Fixed, Random, MoveStop,Animation }
    [Header("�ͦ��覡")]
    [SerializeField] bool IsFixed = false;
    [Header("���ʤ覡")]
    [SerializeField] MoveType moveType;

    //���V���a
    [Header("���V���a")]
    [SerializeField] bool lookAtPlayer;

    [Header("���ʤ�V")]
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Vector3 targetPosition;

    [Header("���ʳt��")]
    [SerializeField] float moveSpeed = 2f;

    [Header("�o�g��")]
    [SerializeField] FireSystem[] firesystems;

    [Header("�X���q�s����")]
    [SerializeField] protected AudioData[] howelSFX;

    [SerializeField] Renderer render;

    [SerializeField] float paddingX;
    [SerializeField] float paddingY;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected override void Awake()
    {
        base.Awake();
        var size = render.bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    protected override void OnEnable()
    {
        //��q�]�w
        maxHealth *= GameManager.Instance.EnemyPower;
        health = maxHealth;
        if (showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHealthBar();
        }

        CanGetAward = true;
        IsCollision = false;

        //�X���q�s
        if (howelSFX.Length != 0) StartCoroutine(nameof(StartAudio));

        EnemyManager.Instance.AddToList(gameObject); //�[�J�ĤH�C��

        StartMoving();
    }
    void OnDisable()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //�[�J�ĤH�C��
        StopAllCoroutines();
    }

    IEnumerator StartAudio()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayRandomSFX(howelSFX);
    }

    void StartMoving()
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
    }

    //�H������
    IEnumerator RandomlyMovingCoroutine()
    {
        if (GameManager.GameState == GameState.GameOver) yield break;

        //�ʵe����
        if (moveType == MoveType.Animation)
        {
            //�}�Ҷ}��
            for (int i = 0; i < firesystems.Length; i++)
            {
                firesystems[i].CanFire = true;
            }

            //�O�_���V���a
            while (gameObject.activeSelf && lookAtPlayer)
            {
                LookAtPlayer();
                yield return waitForFixedUpdate;
            }
            yield break;

        }

        //�ͦ�
        if(!IsFixed) transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);

        if (moveType == MoveType.Fixed)//�T�w��V
        {
            //�O�_�¦V���a
            if (lookAtPlayer)
            {
                moveDirection = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
                targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            }
            else
            {
                targetPosition = transform.position + new Vector3(0, -1, 0);
            }
        }
        else
        {
            targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
        }

        
        if (moveType == MoveType.MoveStop)
        {
            moveDirection = (targetPosition - transform.position).normalized;
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.down, moveDirection);
        }

        //�]�w���ʤ覡
        if (moveType == MoveType.Fixed) //�T�w��V
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.down, moveDirection);
            StartCoroutine(MoveDirectly());
            yield break;
        }
        else if (moveType == MoveType.Random) //�H����V
        {
            while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver)
            {
                if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
                {
                    //���ʨ�ؼ��I
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    //��F�ؼ��I���s�s���ؼ��I
                    targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
                }

                yield return waitForFixedUpdate;
            }
        }
        else if (moveType == MoveType.MoveStop) //���ʫᰱ�� ���V���a
        {
            bool IsArrive = false;
            while (gameObject.activeSelf)
            {
                if (GameManager.GameState == GameState.GameOver) yield break;

                if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime && !IsArrive)
                {
                    //���ʨ�ؼ��I
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    if (!IsArrive)
                    {
                        for(int i = 0; i < firesystems.Length; i++)
                        {
                            firesystems[i].CanFire = true;
                        }
                        IsArrive = true;
                    }
                    if (lookAtPlayer)
                    {
                        LookAtPlayer();
                    }
                    else
                    {
                        yield break;
                    }
                }
                yield return waitForFixedUpdate;
            }
        }
    }
    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            if (GameManager.GameState == GameState.GameOver) yield break;

            Move();

            yield return null;
        }
    }

    //�u�۩T�w��V����
    public void Move()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Viewport.Instance.EnemyExitMap(transform.position.x, transform.position.y, paddingX, paddingY))
        {
            StopAllCoroutines();
            StartMoving();
        }
    }

    //���V���a
    Vector2 targetDirection;
    float angle;

    public void LookAtPlayer()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        moveDirection = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.FromToRotation(Vector2.down, moveDirection),2f * Time.fixedDeltaTime);
    }
}
