using System.Collections.Generic;
using UnityEngine;

public static class PoolableManager
{
    private static List<Poolable> poolables = new List<Poolable>();

    public static void Init()
    {
        poolables.Clear();
    }

    public static Poolable GetPoolable<T>()
    {
        for (int i = 0; i < poolables.Count; i++)
        {
            if (poolables[i].IsType<T>())
            {
                return poolables[i];
            }
        }
        return null;
    }

    public static Poolable CreatePoolable(GameObject obj, int qty)
    {
        Poolable poolable = new Poolable();
        poolable.SetCapacity(qty);
        poolable.Init(obj);
        poolables.Add(poolable);
        return poolable;
    }

    public static void DestroyCreatedObjects()
    {
        for (int i = 0; i < poolables.Count; i++)
        {
            poolables[i].DestroyCreatedObjects();
        }
    }
}

