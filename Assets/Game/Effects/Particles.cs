using System.Collections.Generic;
using UnityEngine;

public class Particles
{
    private Stack<ParticleSystem> pool;
    private readonly Transform cache;
    private int nbrActive = 0;

    public int CountInactive => pool.Count - nbrActive;

    public void Init(ParticleSystem obj, int qty)
    {
        pool = new Stack<ParticleSystem>(qty);
        for (int i = 0; i < qty; i++)
        {
            ParticleSystem instance = ParticleSystem.Instantiate(obj, cache);
            pool.Push(instance);
        }
    }

    public void Clear()
    {
        nbrActive = 0;
        pool.Clear();
    }

    public ParticleSystem Get(Transform parent)
    {
        ParticleSystem instance = pool.Pop();
        instance.transform.parent = parent;
        nbrActive++;
        return instance;
    }

    public void Release(ParticleSystem element)
    {
        pool.Push(element);
        element.transform.parent = cache;
        nbrActive--;
        throw new System.NotImplementedException();
    }

    void OnDestroy()
    {
        pool.Clear();
    }
}