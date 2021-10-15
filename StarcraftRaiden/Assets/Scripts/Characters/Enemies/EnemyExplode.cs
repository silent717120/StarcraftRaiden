using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplode : MonoBehaviour
{
    [Header("�l�u")]
    [SerializeField] GameObject projectile;
    [Header("�ˮ`")]
    [SerializeField] int damage = 2;
    [Header("�t��")]
    [SerializeField] float moveSpeed = 3;
    [Header("����Ĳ�o")]
    [SerializeField] bool IsKeep = false;
    [SerializeField] int FourOrEight = 1;
    [SerializeField] float FireInterval = 3;
    WaitForSeconds waitForFireInterval; //���q�g���ɶ�

    GameObject newprojectile;
    private void OnEnable()
    {
        if (IsKeep) StartCoroutine(nameof(StartExplode));
    }

    IEnumerator StartExplode()
    {
        waitForFireInterval = new WaitForSeconds(FireInterval);
        while (gameObject.activeSelf)
        {
            if (GameManager.GameState == GameState.GameOver) yield break;
            yield return waitForFireInterval;
            if (FourOrEight == 1)
            {
                FourDirExplode();
            }
            else
            {
                EightDirExplode();
            }
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    //�|��V
    public void FourDirExplode()
    {
        float Rotate = Random.Range(0f,1f);

        for(int i = 0; i < 4; i++)
        {
            newprojectile = PoolManager.Release(projectile, transform.position);
            newprojectile.GetComponent<Projectile>().damage = damage;
            newprojectile.GetComponent<Projectile>().moveSpeed = moveSpeed;
            if(i == 0)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(Rotate, 1);
            }else if (i == 1)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(1, -Rotate);
            }else if (i == 2)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(-Rotate, -1);
            }else if (i == 3)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(-1, Rotate);
            }
        }
    }

    //�K��V
    public void EightDirExplode()
    {
        //float Rotate = Random.Range(0f, 1f);
        float Rotate = 0;

        for (int i = 0; i < 8; i++)
        {
            newprojectile = PoolManager.Release(projectile, transform.position);
            newprojectile.GetComponent<Projectile>().damage = damage;
            newprojectile.GetComponent<Projectile>().moveSpeed = moveSpeed;
            if (i == 0)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(Rotate, 1);
            }
            else if (i == 1)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(0.5f, 0.5f);
            }
            else if (i == 2)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(1, -Rotate);
            }
            else if (i == 3)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(0.5f, -0.5f);
            }
            else if (i == 4)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(-Rotate, -1);
            }
            else if (i == 5)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(-0.5f, -0.5f);
            }
            else if (i == 6)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(-1, Rotate);
            }
            else if (i == 7)
            {
                newprojectile.GetComponent<Projectile>().moveDirection = new Vector2(-0.5f, 0.5f);
            }
        }
    }
}
