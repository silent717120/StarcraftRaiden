using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItem : MonoBehaviour
{
    [Header("命中特效")]
    [SerializeField] GameObject hitVFX;
    [Header("命中音效")]
    [SerializeField] AudioData[] hitSFX;
    [Header("道具類型")]
    [SerializeField] int type = 0; //0是補血  1是補能量  2是補充導彈
    [Header("道具數值")]
    [SerializeField] int num;


    [Header("移動速度")]
    [SerializeField] float minMoveSpeed = 0.5f;
    [SerializeField] float maxMoveSpeed = 1f;
    [Header("旋轉速度")]
    [SerializeField] float moveRotationAngleX = 1f;
    [SerializeField] float moveRotationAngleY = 1f;

    [Header("移動方向")]
    [SerializeField] Vector2 moveDirection;

    float currentRotateX;
    float currentRotateY;

    [SerializeField] float paddingX;
    [SerializeField] float paddingY;


    void OnEnable()
    {
        StartCoroutine(nameof(MovingCoroutine));
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        //TryGetComponent測試是否取得特定Component
        if (collider.gameObject.TryGetComponent<Player>(out Player character))
        {
            character.GetItem(type, num);

            PoolManager.Release(hitVFX, transform.position + new Vector3(0,0.2f,0));

            //音效
            AudioManager.Instance.PlayRandomSFX(hitSFX);

            gameObject.SetActive(false);
        }
    }

    //隨機移動
    IEnumerator MovingCoroutine()
    {
        transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90)));

        moveDirection = new Vector3(Random.Range(0f, 2f)-1f, Random.Range(0f, 2f)-1f);

        float moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        currentRotateX = 0;
        currentRotateY = 0;

        while (gameObject.activeSelf)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            currentRotateX += moveRotationAngleX;
            currentRotateY += moveRotationAngleY;

            transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(currentRotateX, currentRotateY, 0));

            yield return null;
        }
    }
}
