using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLine : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] float minMoveSpeed = 0.5f;
    [SerializeField] float maxMoveSpeed = 1f;
    [Header("移動方向")]
    [SerializeField] Vector2 moveDirection;

    float paddingX = 0.01f;
    float paddingY = 1f;

    private void Awake()
    {
    }

    void OnEnable()
    {
        StartCoroutine(nameof(MovingCoroutine));
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    //隨機移動
    IEnumerator MovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);

        float moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        while (gameObject.activeSelf)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
