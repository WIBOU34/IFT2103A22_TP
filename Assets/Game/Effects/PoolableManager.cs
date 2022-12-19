using System.Collections.Generic;
using UnityEngine;

public static class PoolableManager
{
    private static Dictionary<string, Poolable> poolables = new Dictionary<string, Poolable>();

    public static void Init()
    {
        poolables.Clear();
    }

    public static Poolable GetPoolable<T>(string key)
    {
        if (poolables.TryGetValue(key, out Poolable pool))
            return pool;
        return null;
    }

    public static Poolable CreatePoolable(string key, GameObject obj, int qty)
    {
        Poolable poolable = new Poolable();
        poolable.SetCapacity(qty);
        poolable.Init(obj);
        poolables.Add(key, poolable);
        return poolable;
    }

    public static void DestroyCreatedObjects()
    {
        foreach (var item in poolables.Values)
        {
            item.DestroyCreatedObjects();
        }
        poolables.Clear();
    }
}

