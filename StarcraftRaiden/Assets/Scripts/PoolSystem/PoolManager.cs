using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] Pool[] enemyPools;
    [SerializeField] Pool[] playerProjectilePools;
    [SerializeField] Pool[] enemyProjectilePools;
    [SerializeField] Pool[] vFXPools;
    [SerializeField] Pool[] AsteroidXPools;
    [SerializeField] Pool[] ItemPools;

    static Dictionary<GameObject, Pool> dictionary;

    void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();

        Initialize(enemyPools);
        Initialize(playerProjectilePools);
        Initialize(enemyProjectilePools);
        Initialize(vFXPools);
        Initialize(AsteroidXPools);
        Initialize(ItemPools);
    }

#if UNITY_EDITOR
    //編輯器停止調用
    void OnDestroy()
    {
        CheckPoolSize(enemyPools);
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools);
        CheckPoolSize(AsteroidXPools);
        CheckPoolSize(ItemPools);
    }

    void CheckPoolSize(Pool[] pools)
    {
        foreach(var pool in pools)
        {
            if(pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning(
                                string.Format("對象池:{0} 有{1}個在運行，大過基本數{2}個",
                                pool.Prefab.name,
                                pool.RuntimeSize,
                                pool.Size));
            }
        }
    }
#endif

    //初始化全部對象池
    void Initialize(Pool[] pools)
    {
        foreach(var pool in pools)
        {
            #if UNITY_EDITOR
                //若字典中已存入此鍵就跳過
                if (dictionary.ContainsKey(pool.Prefab))
                {
                    Debug.LogError("Same prefab in multiple pools: Prefab: " + pool.Prefab.name);

                    continue;
                }
            #endif

                dictionary.Add(pool.Prefab, pool);

                Transform poolParent = new GameObject("Pool :  " + pool.Prefab.name).transform;

                poolParent.parent = transform;
                pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// <para>根據傳入的<paramref name="prefab"></paramref>參數,返回對象池中預備好的遊戲對象</para>
    /// </summary>
    /// <paramref name="prefab">
    /// <para>指定的遊戲對象預製體</para>
    /// </param>
    /// <returns>
    /// <para>對象池中預備好的遊戲對象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find Prefab: " + prefab.name);
            return null;
        }
        #endif
        return dictionary[prefab].preparedObject();
    }

    /// <summary>
    /// <para>根據傳入的prefab參數,在position參數位置釋放對象池中預備好的遊戲對象</para>
    /// </summary>
    /// <paramref name="prefab">
    /// <para>指定的遊戲對象預製體</para>
    /// </param>
    /// <returns>
    /// <para>對象池中預備好的遊戲對象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab,Vector3 position)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find Prefab: " + prefab.name);
            return null;
        }
        #endif
        return dictionary[prefab].preparedObject(position);
    }

    /// <summary>
    /// <para>根據傳入的prefab參數,rotation參數,在position參數位置釋放對象池中預備好的遊戲對象</para>
    /// </summary>
    /// <paramref name="prefab">
    /// <para>指定的遊戲對象預製體</para>
    /// </param>
    /// <returns>
    /// <para>對象池中預備好的遊戲對象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find Prefab: " + prefab.name);
            return null;
        }
        #endif
        return dictionary[prefab].preparedObject(position,rotation);
    }

    /// <summary>
    /// <para>根據傳入的prefab參數,rotation參數,localScale參數,在position參數位置釋放對象池中預備好的遊戲對象</para>
    /// </summary>
    /// <paramref name="prefab">
    /// <para>指定的遊戲對象預製體</para>
    /// </param>
    /// <returns>
    /// <para>對象池中預備好的遊戲對象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find Prefab: " + prefab.name);
            return null;
        }
#endif
        return dictionary[prefab].preparedObject(position, rotation, localScale);
        
    }
}
