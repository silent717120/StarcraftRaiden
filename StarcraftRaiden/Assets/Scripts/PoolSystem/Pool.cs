using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Pool
{
    public GameObject Prefab { get => prefab; }

    //物件池數量
    public int Size { get => size; }
    //物件池運行時的數量
    public int RuntimeSize { get => queue.Count; }
    [Header("要產生的原始物")]
    [SerializeField] GameObject prefab;
    [Header("物件池最大數量")]
    [SerializeField] int size = 1;

    Queue<GameObject> queue;

    Transform parent;

    //初始化對象池
    public void Initialize(Transform parent)
    {
        queue = new Queue<GameObject>();
        this.parent = parent;

        for (var i = 0; i < size; i++)
        {
            queue.Enqueue(Copy()); //加入物件
        }
    }

    //製造物件
    GameObject Copy()
    {
        var copy = GameObject.Instantiate(prefab, parent);

        copy.SetActive(false);

        return copy;
    }

    //取出
    GameObject AvailableObject()
    {
        GameObject availableObject = null;
        //Peek() 返回第一個   並且他未開啟時才調用
        if (queue.Count > 0 && !queue.Peek().activeSelf)
        {
            availableObject =  queue.Dequeue(); //取得開頭第一個
        }
        else
        {
            availableObject = Copy();
        }

        queue.Enqueue(availableObject);  //放回末端

        return availableObject;
    }

    //取出物件並設置
    public GameObject preparedObject()
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);

        return preparedObject;
    }

    public GameObject preparedObject(Vector3 position)
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;

        return preparedObject;
    }

    public GameObject preparedObject(Vector3 position,Quaternion rotation)
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;

        return preparedObject;
    }

    public GameObject preparedObject(Vector3 position, Quaternion rotation,Vector3 localScale)
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;
        preparedObject.transform.localScale = localScale;

        return preparedObject;
    }
}
