using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : Singleton<Viewport>
{
    float minX;
    float maxX;
    float minY;
    float maxY;

    float middleX;
    float middleY;
    [Header("�k��")]
    [SerializeField] GameObject[] Asteroids;
    [SerializeField] float minCreatAst;
    [SerializeField] float maxCreatAst;
    [Header("�t�׷P����")]
    [SerializeField] GameObject SpeedLine;

    private void Start()
    {
        //���o��v���|�Ө��b�@�ɮy�Ъ���m
        Camera mainCamera = Camera.main;

        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 topright = mainCamera.ViewportToWorldPoint(new Vector2(1f, 1f));

        //��v�������Ix��m (�ľ��̦h���ʨ줤���Ӥw)
        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;
        middleY = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).y;

        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topright.x;
        maxY = topright.y;

        StartCoroutine(nameof(CreateAsteroidsCoroutine));
        StartCoroutine(nameof(CreateSpeedLineCoroutine));
    }

    public Vector3 PlayerMoveablePosition(Vector3 playerPosition, float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;
        //���޶ǤJ��m�b���A������b�d��
        position.x = Mathf.Clamp(playerPosition.x , minX + paddingX , maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);

        return position;
    }

    //�H���ͦ��ĤH��m
    public Vector3 RandomEnemySpawnPosition(float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;
        
        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = maxY + paddingY;

        return position;
    }

    //�H���ͦ��k�ۦ�m(�u�b����)
    public Vector3 RandomAsteroidSpawnPosition(float paddingX, float paddingY,float sideLength)
    {
        Vector3 position = Vector3.zero;

        if(Random.Range(0,2) == 0)
        {
            position.x = Random.Range(minX + paddingX, minX+sideLength - paddingX);
        }
        else
        {
            position.x = Random.Range(maxX - sideLength + paddingX, maxX - paddingX);
        }
        position.y = maxY + paddingY;

        return position;
    }

    //����ĤH��m (�u��e������)
    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX + 0.5f, maxX - paddingX - 0.5f);
        position.y = Random.Range(middleY +  maxY/2, maxY - paddingY - paddingY);

        return position;
    }

    //����ĤH��m(���Ϥ�)
    public Vector3 RandomEnemyMovePosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    //�P�_�ĤH�O�_���}�a��
    public bool EnemyExitMap(float nowX, float nowY, float paddingX, float paddingY)
    {
        bool IsExit = false;
        if(nowX > maxX + paddingX || nowX < minX - paddingX || nowY < minY - paddingY)
        {
            IsExit = true;
        }

        return IsExit;
    }

    //���o���ϥk�U�I
    public Vector3 GetRightBottomPoint()
    {
        Vector3 position = new Vector3(maxX, minY,0);

        return position;
    }

    //�k�ۥͦ�
    IEnumerator CreateAsteroidsCoroutine()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            var Asteroid = Asteroids[Random.Range(0, Asteroids.Length)];
            PoolManager.Release(Asteroid);

            yield return new WaitForSeconds(Random.Range(minCreatAst, maxCreatAst));
        }
    }

    //�k�ۥͦ�
    IEnumerator CreateSpeedLineCoroutine()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            PoolManager.Release(SpeedLine);

            yield return new WaitForSeconds(Random.Range(0.1f, 0.35f));
        }
    }
}
