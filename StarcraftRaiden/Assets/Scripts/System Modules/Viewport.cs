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
    [Header("隕石")]
    [SerializeField] GameObject[] Asteroids;
    [SerializeField] float minCreatAst;
    [SerializeField] float maxCreatAst;
    [Header("速度感物件")]
    [SerializeField] GameObject SpeedLine;

    private void Start()
    {
        //取得攝影機四個角在世界座標的位置
        Camera mainCamera = Camera.main;

        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 topright = mainCamera.ViewportToWorldPoint(new Vector2(1f, 1f));

        //攝影機中間點x位置 (敵機最多移動到中間而已)
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
        //不管傳入位置在哪，都限制在範圍內
        position.x = Mathf.Clamp(playerPosition.x , minX + paddingX , maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);

        return position;
    }

    //隨機生成敵人位置
    public Vector3 RandomEnemySpawnPosition(float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;
        
        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = maxY + paddingY;

        return position;
    }

    //隨機生成隕石位置(只在邊邊)
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

    //限制敵人位置 (只到畫面中間)
    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX + 0.5f, maxX - paddingX - 0.5f);
        position.y = Random.Range(middleY +  maxY/2, maxY - paddingY - paddingY);

        return position;
    }

    //限制敵人位置(全圖內)
    public Vector3 RandomEnemyMovePosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    //判斷敵人是否離開地圖
    public bool EnemyExitMap(float nowX, float nowY, float paddingX, float paddingY)
    {
        bool IsExit = false;
        if(nowX > maxX + paddingX || nowX < minX - paddingX || nowY < minY - paddingY)
        {
            IsExit = true;
        }

        return IsExit;
    }

    //取得全圖右下點
    public Vector3 GetRightBottomPoint()
    {
        Vector3 position = new Vector3(maxX, minY,0);

        return position;
    }

    //隕石生成
    IEnumerator CreateAsteroidsCoroutine()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            var Asteroid = Asteroids[Random.Range(0, Asteroids.Length)];
            PoolManager.Release(Asteroid);

            yield return new WaitForSeconds(Random.Range(minCreatAst, maxCreatAst));
        }
    }

    //隕石生成
    IEnumerator CreateSpeedLineCoroutine()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            PoolManager.Release(SpeedLine);

            yield return new WaitForSeconds(Random.Range(0.1f, 0.35f));
        }
    }
}
