using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    private Vector2[] _spawnPoint;

    private int _curSpawnPoint;

    private Transform _parent;

    private void Awake()
    {
        _spawnPoint = new Vector2[4]; // 배열 크기 명시적으로 초기화
        for (int i = 0; i < 4; i++)
        {
            _spawnPoint[i] = transform.GetChild(i).position;
        }

        _parent = transform.GetChild(4);
    }

    public void SpawnUnit(GameObject unitPrefab)
    {
        Instantiate(unitPrefab, _spawnPoint[_curSpawnPoint], Quaternion.identity);

        _curSpawnPoint = (_curSpawnPoint >= 3) ? 0 : _curSpawnPoint + 1;
    }
}
