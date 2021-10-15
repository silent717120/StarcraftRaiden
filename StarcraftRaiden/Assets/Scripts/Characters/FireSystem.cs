using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour
{
    enum FireDirection { Fixed, FixedStartPlayer,FollowPlayer }
    [Header("�o�g��V")]
    [SerializeField] FireDirection fireDirection;
    [Header("�ؼ�")]
    Vector2 oldMoveDirection;
    [SerializeField] Vector2 moveDirection;
    GameObject FireTarget; //�ؼ�
    [Header("����")]
    [SerializeField] Vector2 RotateDirection;
    [Header("���o")]
    [SerializeField] float Radiate;

    [Header("�l�u�w�s��")]
    [SerializeField] GameObject[] projectiles;
    [Header("�o�g����")]
    [SerializeField] AudioData[] projectileLaunchSFX;

    [Header("�ˮ`��")]
    [SerializeField] float damage;
    [Header("�l�u�t��")]
    [SerializeField] protected float moveSpeed = 10;


    [Header("�g���I")]
    [SerializeField] Transform[] muzzle;


    [Header("�N�o�ɶ�")]
    [SerializeField] float DoldDown = 4;
    [Header("�o�g����")]
    [SerializeField] int FireWave = 5;
    int NowFireWave = 0;

    [Header("�O�_�i�o�g")]
    [SerializeField] public bool CanFire = true;
    bool OldCanFire = true; //�����]�w  ���s�}�Үɴ_���
    [Header("�Ĥ@���}���ɶ�")]
    [SerializeField] float StartFireInterval;
    [Header("�̵u�}���ɶ�")]
    [SerializeField] float minFireInterval;
    [Header("�̪��}���ɶ�")]
    [SerializeField] float maxFireInterval;

    [Header("�ʧ@")]
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

    //�}��
    IEnumerator FireCoroutine()
    {
        yield return waitUntiCanFire;
        yield return waitForStartTime;

        while (gameObject.activeSelf)
        {
            //�O�_���H���a  �����u��l�˷� �� �C�i���˷�
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

             //���o���_�l����
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

            //�s��o�g�]�w
            NowFireWave++;
            moveDirection += RotateDirection;

            if (NowFireWave >= FireWave)
            {
                //�Y���˷Ǫ��a�A�h�^���l��V
                moveDirection = oldMoveDirection;

                NowFireWave = 0;
                yield return new WaitForSeconds(DoldDown);
            }
        }
    }
}
