using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("���ʳt��")]
    [SerializeField] float moveSpeed = 2f;
    [Header("���ʱ��ਤ��")]
    [SerializeField] float moveRotationAngle = 25f;
    [Header("�l�u�w�s��")]
    [SerializeField] GameObject[] projectiles;
    [Header("�o�g����")]
    [SerializeField] AudioData[] projectileLaunchSFX;
    [Header("�g���I")]
    [SerializeField] Transform muzzle;
    [Header("�̵u�}���ɶ�")]
    [SerializeField] float minFireInterval;
    [Header("�̪��}���ɶ�")]
    [SerializeField] float maxFireInterval;
    [Header("�O�_�o�g�l�u")]
    [SerializeField] bool CanFire = true;

    [Header("�O�_�T�w��m�ͦ�")]
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

    //�H������
    IEnumerator RandomlyMovingCoroutine()
    {
        //�]�w�ͦ���m
        if (FixedSpawn)
        {
            transform.position = Fixedposition;
        }
        else
        {
            transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        }
        //�]�w���ʦa�I
        Vector3 targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            if(Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                //���ʨ�ؼ��I
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                //�W�U�ɻݭn����
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.x * moveRotationAngle, Vector3.down);
            }
            else
            {
                //��F�ؼ��I���s�s���ؼ��I
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }

            yield return waitForFixedUpdate;
        }
    }

    //�H���}��
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
