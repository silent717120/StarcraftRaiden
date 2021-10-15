using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItem : MonoBehaviour
{
    [Header("�R���S��")]
    [SerializeField] GameObject hitVFX;
    [Header("�R������")]
    [SerializeField] AudioData[] hitSFX;
    [Header("�D������")]
    [SerializeField] int type = 0; //0�O�ɦ�  1�O�ɯ�q  2�O�ɥR�ɼu
    [Header("�D��ƭ�")]
    [SerializeField] int num;


    [Header("���ʳt��")]
    [SerializeField] float minMoveSpeed = 0.5f;
    [SerializeField] float maxMoveSpeed = 1f;
    [Header("����t��")]
    [SerializeField] float moveRotationAngleX = 1f;
    [SerializeField] float moveRotationAngleY = 1f;

    [Header("���ʤ�V")]
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
        //TryGetComponent���լO�_���o�S�wComponent
        if (collider.gameObject.TryGetComponent<Player>(out Player character))
        {
            character.GetItem(type, num);

            PoolManager.Release(hitVFX, transform.position + new Vector3(0,0.2f,0));

            //����
            AudioManager.Instance.PlayRandomSFX(hitSFX);

            gameObject.SetActive(false);
        }
    }

    //�H������
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
