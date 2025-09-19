using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] int _spawnCount;

    private Transform _spawnPoint;

    public Transform[] PathPoints;
     
    private ObjectPool _objectPool => GetComponent<ObjectPool>();

    private void Awake()
    {
        _spawnPoint = PathPoints[0];
    }

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
