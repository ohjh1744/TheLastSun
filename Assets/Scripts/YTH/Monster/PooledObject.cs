using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool returnPool;

    private float timer;

    public void ReturnPool()
    {
        if (returnPool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            returnPool.ReturnPool(this);
        }
    }
}
