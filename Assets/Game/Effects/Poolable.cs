using System.Collections.Generic;
using UnityEngine;

public class Poolable
{
    private Stack<GameObject> pool;
    private int maxQty = 0;
    private readonly Transform cache;
    private int nbrActive = 0;
    private bool isInit = false;

    public int CountInactive => maxQty - nbrActive;

    public bool IsInit()
    {
        return isInit;
    }

    public bool IsType<T>()
    {
        if (pool == null || !pool.TryPeek(out _))
            return false;
        return pool.Peek().TryGetComponent<T>(out _);
    }

    public void SetCapacity(int qty)
    {
        maxQty = qty;
        pool = new Stack<GameObject>(qty);
    }

    public void Init(GameObject obj)
    {
        for (int i = 0; i < maxQty; i++)
        {
            GameObject instance = GameObject.Instantiate(obj, cache);
            instance.SetActive(false);
            pool.Push(instance);
        }
        isInit = true;
    }

    public void Clear()
    {
        nbrActive = 0;
        pool.Clear();
    }

    public GameObject Get(Transform parent)
    {
        GameObject instance = pool.Pop();
        instance.transform.SetParent(parent, false);
        instance.SetActive(true);
        nbrActive++;
        return instance;
    }

    public GameObject Get()
    {
        GameObject instance = pool.Pop();
        instance.SetActive(true);
        nbrActive++;
        return instance;
    }

    public void Release(GameObject instance)
    {
        pool.Push(instance);
        instance.SetActive(false);
        instance.transform.parent = cache;
        nbrActive--;
    }

    public void DestroyCreatedObjects()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            GameObject.Destroy(pool.Pop().gameObject);
        }
    }
}
