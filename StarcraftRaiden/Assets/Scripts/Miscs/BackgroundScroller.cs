using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("�I�����ʳt��")]
    [SerializeField]Vector2 scrollVelocity;

    Material material;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    //Start�]�i�Ψ�
    IEnumerator Start()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += scrollVelocity * Time.deltaTime;

            yield return null;
        }
    }
}
