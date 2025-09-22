using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] int _spawnCount;

     [SerializeField] Transform _spawnPoint;

    private ObjectPool _objectPool => GetComponent<ObjectPool>();

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void Spawn()
    {
        PooledObject pooledObject = _objectPool.GetPool(_spawnPoint.position, Quaternion.identity);
    }

    WaitForSeconds delay = new(2f);
    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Spawn();
            yield return delay;
        }
    }

}
