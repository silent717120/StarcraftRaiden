using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] float minMoveSpeed = 0.5f;
    [SerializeField] float maxMoveSpeed = 1f;
    [Header("旋轉速度")]
    [SerializeField] float moveRotationAngleX = 1f;
    [SerializeField] float moveRotationAngleY = 1f;
    [Header("大小")]
    [SerializeField] float minScale = 0.15f;
    [SerializeField] float maxScale = 0.25f;
    [Header("移動方向")]
    [SerializeField] Vector2 moveDirection;

    [SerializeField] float paddingX;
    [SerializeField] float paddingY;

    [SerializeField] Vector3 RightBottomPoint;

    float currentRotateX;
    float currentRotateY;

    private void Awake()
    {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    void OnEnable()
    {
        RightBottomPoint = Viewport.Instance.GetRightBottomPoint();
        StartCoroutine(nameof(MovingCoroutine));
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    //隨機移動
    IEnumerator MovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomAsteroidSpawnPosition(paddingX, paddingY,1.5f);
        transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90)));

        float moveSpeed = Random.Range(minMoveSpeed,maxMoveSpeed);
        float Scale = Random.Range(minScale, maxScale);

        currentRotateX = 0;
        currentRotateY = 0;

        while (gameObject.activeSelf)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            currentRotateX += moveRotationAngleX;
            currentRotateY += moveRotationAngleY;

            transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(currentRotateX, currentRotateY, 0));

            if(transform.position.y + paddingY < RightBottomPoint.y)
            {
                gameObject.SetActive(false);
            }

            yield return null;
        }
    }
}
