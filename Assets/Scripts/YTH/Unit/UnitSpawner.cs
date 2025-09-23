using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    private Vector2[] _spawnPoint;

    private int _curSpawnPoint;

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            _spawnPoint[i] = transform.GetChild(i).position;
        }
    }

    public void SpawnUnit(GameObject unitPrefab)
    {
        Instantiate(unitPrefab, _spawnPoint[_curSpawnPoint], Quaternion.identity);

        _curSpawnPoint = (_curSpawnPoint >= 3) ? 0 : _curSpawnPoint + 1;
    }
}
