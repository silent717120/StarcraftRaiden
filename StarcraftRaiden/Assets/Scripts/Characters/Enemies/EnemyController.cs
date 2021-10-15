using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] float moveSpeed = 2f;
    [Header("移動旋轉角度")]
    [SerializeField] float moveRotationAngle = 25f;
    [Header("子彈預製物")]
    [SerializeField] GameObject[] projectiles;
    [Header("發射音效")]
    [SerializeField] AudioData[] projectileLaunchSFX;
    [Header("射擊點")]
    [SerializeField] Transform muzzle;
    [Header("最短開火時間")]
    [SerializeField] float minFireInterval;
    [Header("最長開火時間")]
    [SerializeField] float maxFireInterval;
    [Header("是否發射子彈")]
    [SerializeField] bool CanFire = true;

    [Header("是否固定位置生成")]
    [SerializeField] bool FixedSpawn = false;
    [SerializeField] Vector2 Fixedposition;

    [SerializeField] float paddingX;
    [SerializeField] float paddingY;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    void OnEnable()
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        if(CanFire)StartCoroutine(nameof(RandomlyFireCoroutine));
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }

    //隨機移動
    IEnumerator RandomlyMovingCoroutine()
    {
        //設定生成位置
        if (FixedSpawn)
        {
            transform.position = Fixedposition;
        }
        else
        {
            transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        }
        //設定移動地點
        Vector3 targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            if(Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //移動到目標點
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                //上下時需要旋轉
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.x * moveRotationAngle, Vector3.down);
            }
            else
            {
                //到達目標點後更新新的目標點
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }

            yield return waitForFixedUpdate;
        }
    }

    //隨機開火
    IEnumerator RandomlyFireCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if (GameManager.GameState == GameState.GameOver) yield break;

            foreach(var projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
        }
    }
}
