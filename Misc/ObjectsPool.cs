using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    List<Pool> poolsList = new List<Pool>();

    public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent)
    {
        var tag = gameObject.tag;
        var pool = GetPool(tag);

        if (pool.Size() > 0)
        {
            var retrivedObject = pool.Get();

            retrivedObject.transform.position = position;
            retrivedObject.transform.SetParent(parent);
            retrivedObject.SetActive(true);
            return retrivedObject;
        }

        //GameObject is used here as a disambiguation to avoid a recursive method call. DO NOT ERASE
        var newObject = GameObject.Instantiate(gameObject, position, rotation, parent);

        return newObject;
    }

    private Pool GetPool(string tag)
    {
        var pool = CheckForExistingPools(tag);

        if (pool == null)
        {
            pool = new Pool(tag);
            poolsList.Add(pool);
        }

        return pool;
    }

    private Pool CheckForExistingPools(string tag)
    {
        foreach (var pool in poolsList)
        {
            if (pool.Tag == tag)
            {
                return pool;
            }
        }

        return null;
    }

    public void GenerateJewelsPools(int numberOfJewels)
    {
        var jewelTemplates = GetComponent<JewelManager>().GetJewelTemplatesListSO();
        var templates = jewelTemplates.GetJewelTemplates();

        for (int i = 1; i <= numberOfJewels; i++)
        {
            var gameObject = templates[i].prefab.tag.ToString();

            poolsList.Add(new Pool(gameObject));
        }
    }

    public void Recycle(GameObject gameObject)
    {     
        var tag = gameObject.tag;
        gameObject.SetActive(false);

        var pool = GetPool(tag);
        pool.Add(gameObject);
    }

    public void ReduceAllPools(int maxSize)
    {
        foreach (var pool in poolsList)
        {
            ReducePool(maxSize, pool);
        }
    }

    public void ReducePool(int maxSize, GameObject gameObject)
    {
        ReducePool(maxSize, GetPool(gameObject));
    }

    private void ReducePool(int maxSize, Pool pool)
    {
        while (pool.Size() > maxSize)
        {
            Recycle(pool.Get());
        }
    }

    private Pool GetPool(GameObject gameObject)
    {
        Pool searchedPool = null;

        foreach (var pool in poolsList)
        {
            if (pool.Tag == gameObject.tag.ToString())
            {
                searchedPool = pool;
                break;
            }
        }

        return searchedPool;
    }

    private class Pool
    {
        public string Tag { get; private set; }
        public Stack<GameObject> objectsStack = new Stack<GameObject>();

        public Pool(string tag)
        {
            Tag = tag;
        }

        public void Add(GameObject gameObject)
        {
            objectsStack.Push(gameObject);
        }

        public GameObject Get()
        {
            return objectsStack.Pop();
        }

        public int Size()
        {
            return objectsStack.Count;
        }
    }
}