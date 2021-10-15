using System.Collections;
using UnityEngine;

public class AutoDeactivate : MonoBehaviour
{
    [Header("是否刪除物件")]
    [SerializeField] bool destroyGameObject;
    [Header("存活時間")]
    [SerializeField] float lifetime = 3f;

    WaitForSeconds waitLifetime;

    void Awake()
    {
        waitLifetime = new WaitForSeconds(lifetime);
    }

    void OnEnable()
    {
        StartCoroutine(DeactivateCoroutine());
    }

    IEnumerator DeactivateCoroutine()
    {
        yield return waitLifetime;

        if (destroyGameObject)
        {
            Destroy(gameObject);
        }
        else 
        {
            gameObject.SetActive(false);
        }
    }
}