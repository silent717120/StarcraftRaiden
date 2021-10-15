using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour
{
    enum FireDirection { Fixed, FixedStartPlayer,FollowPlayer }
    [Header("發射方向")]
    [SerializeField] FireDirection fireDirection;
    [Header("目標")]
    Vector2 oldMoveDirection;
    [SerializeField] Vector2 moveDirection;
    GameObject FireTarget; //目標
    [Header("旋轉")]
    [SerializeField] Vector2 RotateDirection;
    [Header("散發")]
    [SerializeField] float Radiate;

    [Header("子彈預製物")]
    [SerializeField] GameObject[] projectiles;
    [Header("發射音效")]
    [SerializeField] AudioData[] projectileLaunchSFX;

    [Header("傷害值")]
    [SerializeField] float damage;
    [Header("子彈速度")]
    [SerializeField] protected float moveSpeed = 10;


    [Header("射擊點")]
    [SerializeField] Transform[] muzzle;


    [Header("冷卻時間")]
    [SerializeField] float DoldDown = 4;
    [Header("發射次數")]
    [SerializeField] int FireWave = 5;
    int NowFireWave = 0;

    [Header("是否可發射")]
    [SerializeField] public bool CanFire = true;
    bool OldCanFire = true; //紀錄設定  重新開啟時復原用
    [Header("第一次開火時間")]
    [SerializeField] float StartFireInterval;
    [Header("最短開火時間")]
    [SerializeField] float minFireInterval;
    [Header("最長開火時間")]
    [SerializeField] float maxFireInterval;

    [Header("動作")]
    [SerializeField] Animator animator;

    WaitForSeconds waitForStartTime;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    WaitUntil waitUntiCanFire;

    private void Awake()
    {
        OldCanFire = CanFire;
        oldMoveDirection = moveDirection;
        waitForStartTime = new WaitForSeconds(StartFireInterval);
        FireTarget = GameObject.FindGameObjectWithTag("Player");
        waitUntiCanFire = new WaitUntil(() => CanFire);
    }

    void OnEnable()
    {
        CanFire = OldCanFire;
        NowFireWave = 0;
        Fire();
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }

    void Fire()
    {
        StartCoroutine(nameof(FireCoroutine));
    }

    //開火
    IEnumerator FireCoroutine()
    {
        yield return waitUntiCanFire;
        yield return waitForStartTime;

        while (gameObject.activeSelf)
        {
            //是否跟隨玩家  分成只初始瞄準 跟 每波都瞄準
            if (NowFireWave == 0 && fireDirection == FireDirection.FixedStartPlayer)
            {
                moveDirection = (FireTarget.transform.position - transform.position).normalized;
            }else if(fireDirection == FireDirection.FollowPlayer)
            {
                moveDirection = (FireTarget.transform.position - transform.position).normalized;
            }

            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if (GameManager.GameState == GameState.GameOver) yield break;

            if (animator != null) animator.SetTrigger("Attack");

             //散發的起始角度
             Vector2 RadiateDirection = moveDirection - new Vector2((float)projectiles.Length / 2f * Radiate,0);
            for (int i = 0; i < projectiles.Length; i++)
            {
                GameObject newprojectile = PoolManager.Release(projectiles[i], muzzle[i].position);
                if(newprojectile.GetComponent<Projectile>() != null)
                {
                    newprojectile.GetComponent<Projectile>().moveDirection = RadiateDirection;
                    newprojectile.GetComponent<Projectile>().damage = damage;
                    newprojectile.GetComponent<Projectile>().moveSpeed = moveSpeed;
                    newprojectile.GetComponent<EnemyProjectile>().updateRoate();
                    RadiateDirection += new Vector2(Radiate, 0);
                }
            }

            if(projectileLaunchSFX.Length != 0)
            {
                AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            }

            //連續發射設定
            NowFireWave++;
            moveDirection += RotateDirection;

            if (NowFireWave >= FireWave)
            {
                //若不瞄準玩家，則回到初始方向
                moveDirection = oldMoveDirection;

                NowFireWave = 0;
                yield return new WaitForSeconds(DoldDown);
            }
        }
    }
}
