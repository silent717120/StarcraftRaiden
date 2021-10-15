using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLine : MonoBehaviour
{
    [Header("���ʳt��")]
    [SerializeField] float minMoveSpeed = 0.5f;
    [SerializeField] float maxMoveSpeed = 1f;
    [Header("���ʤ�V")]
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

    //�H������
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
