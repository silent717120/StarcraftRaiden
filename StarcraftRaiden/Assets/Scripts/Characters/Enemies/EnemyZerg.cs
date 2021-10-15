using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZerg : Enemy
{
    //固定方向  隨機方向  移動後停止  ,動畫控制
    enum MoveType { Fixed, Random, MoveStop,Animation }
    [Header("生成方式")]
    [SerializeField] bool IsFixed = false;
    [Header("移動方式")]
    [SerializeField] MoveType moveType;

    //面向玩家
    [Header("面向玩家")]
    [SerializeField] bool lookAtPlayer;

    [Header("移動方向")]
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Vector3 targetPosition;

    [Header("移動速度")]
    [SerializeField] float moveSpeed = 2f;

    [Header("發射器")]
    [SerializeField] FireSystem[] firesystems;

    [Header("出場吼叫音效")]
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
        //血量設定
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

        //出場吼叫
        if (howelSFX.Length != 0) StartCoroutine(nameof(StartAudio));

        EnemyManager.Instance.AddToList(gameObject); //加入敵人列表中

        StartMoving();
    }
    void OnDisable()
    {
        EnemyManager.Instance.RemoveFromList(gameObject); //加入敵人列表中
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

    //隨機移動
    IEnumerator RandomlyMovingCoroutine()
    {
        if (GameManager.GameState == GameState.GameOver) yield break;

        //動畫控制
        if (moveType == MoveType.Animation)
        {
            //開啟開火
            for (int i = 0; i < firesystems.Length; i++)
            {
                firesystems[i].CanFire = true;
            }

            //是否面向玩家
            while (gameObject.activeSelf && lookAtPlayer)
            {
                LookAtPlayer();
                yield return waitForFixedUpdate;
            }
            yield break;

        }

        //生成
        if(!IsFixed) transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);

        if (moveType == MoveType.Fixed)//固定方向
        {
            //是否朝向玩家
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

        //設定移動方式
        if (moveType == MoveType.Fixed) //固定方向
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.down, moveDirection);
            StartCoroutine(MoveDirectly());
            yield break;
        }
        else if (moveType == MoveType.Random) //隨機方向
        {
            while (gameObject.activeSelf && GameManager.GameState != GameState.GameOver)
            {
                if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
                {
                    //移動到目標點
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    //到達目標點後更新新的目標點
                    targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
                }

                yield return waitForFixedUpdate;
            }
        }
        else if (moveType == MoveType.MoveStop) //移動後停止 面向玩家
        {
            bool IsArrive = false;
            while (gameObject.activeSelf)
            {
                if (GameManager.GameState == GameState.GameOver) yield break;

                if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime && !IsArrive)
                {
                    //移動到目標點
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

    //沿著固定方向移動
    public void Move()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Viewport.Instance.EnemyExitMap(transform.position.x, transform.position.y, paddingX, paddingY))
        {
            StopAllCoroutines();
            StartMoving();
        }
    }

    //面向玩家
    Vector2 targetDirection;
    float angle;

    public void LookAtPlayer()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        moveDirection = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.FromToRotation(Vector2.down, moveDirection),2f * Time.fixedDeltaTime);
    }
}
