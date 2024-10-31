using System.Collections.Generic;
using UnityEngine;

/*
Constructor: Takes a prefab of type T, an initial size for the pool, and an optional parent transform.
CreateInstance: Instantiates an object of type T, disables it, and returns it.
Get: Retrieves an object from the pool. If the pool is empty, it creates a new instance.
ReturnToPool: Disables the object and returns it to the pool.
*/
public class ObjectPool<T> where T : Component
{
    private readonly Queue<T> poolQueue = new Queue<T>();
    private readonly T prefab;
    private readonly Transform parent;

    public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T instance = CreateInstance();
            poolQueue.Enqueue(instance);
        }
    }

    private T CreateInstance()
    {
        T instance = Object.Instantiate(prefab, parent);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public T Get()
    {
        if (poolQueue.Count > 0)
        {
            T instance = poolQueue.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }
        return CreateInstance();
    }

    public void ReturnToPool(T instance)
    {
        instance.gameObject.SetActive(false);
        poolQueue.Enqueue(instance);
    }
}
