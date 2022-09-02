// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
    private List<GameObject> pool;

    public void Init(GameObject objectPrefab, int poolSize)
    {
        pool = new List<GameObject>();
        GameObject temp;
        for (int i = 0; i < poolSize; i++)
        {
            temp = Instantiate(objectPrefab);
            temp.SetActive(false);
            pool.Add(temp);
        }
    }

    public GameObject GetObjectFromPool()
    {
        for(int i = 0; i < pool.Count; i++)
        {
            if(!pool[i].activeInHierarchy)
                return pool[i];
        }
        return null;
    }

    public void DeactivateAll()
    {
        for (int i = 0; i < pool.Count; i++)
            pool[i].SetActive(false);
    }

    public List<GameObject> GetActiveObjects()
    {
        List<GameObject> activeObjects = new List<GameObject>();

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].activeInHierarchy)
                activeObjects.Add(pool[i]);
        }

        return activeObjects;
    }

}
