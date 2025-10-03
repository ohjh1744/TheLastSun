using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] List<PooledObject> _pool = new List<PooledObject>();
    [SerializeField] PooledObject _prefab;
    [SerializeField] int _size => _pool.Capacity;

    private void Awake()
    {
        Prewarm();
    }

    private void Prewarm()
    {
        _pool.Clear();
        for (int i = 0; i < _size; i++)
        {
            var instance = Instantiate(_prefab, transform);
            InitInstance(instance);
            instance.gameObject.SetActive(false);
            _pool.Add(instance);
        }
    }

    private void InitInstance(PooledObject instance)
    {
        if (instance == null)
        {
            Debug.LogWarning("[ObjectPool] null 인스턴스 초기화 시도");
            return;
        }
        instance.returnPool = this;
        if (instance.transform.parent != transform)
            instance.transform.SetParent(transform);
    }

    public PooledObject GetPool(Vector3 position, Quaternion rotation)
    {
        PooledObject instance = null;

        if (_pool.Count == 0)
        {
            instance = Instantiate(_prefab, position, rotation);
            InitInstance(instance);
        }
        else
        {
            int last = _pool.Count - 1;
            instance = _pool[last];
            _pool.RemoveAt(last);

            if (instance == null)
            {
                Debug.LogWarning("[ObjectPool] null 슬롯 발견. 제거 후 재시도.");
                return GetPool(position, rotation);
            }
        }

        // 재활성화 세팅
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void ReturnPool(PooledObject instance)
    {
        if (instance == null) return;

        // 이미 파괴된 경우
        if (instance.gameObject == null)
            return;

        // 중복 추가 방지
        if (_pool.Contains(instance))
            return;

        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform);
        _pool.Add(instance);
    }
}
