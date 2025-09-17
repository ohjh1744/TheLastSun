using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMover : MonoBehaviour
{
    private Transform[] _pathPoints => FindObjectOfType<MonsterSpawner>().PathPoints;

    private int _curPointIndex = 0;

    private MonsterModel _model;

    private void Awake()
    {
        _model = GetComponent<MonsterModel>();
    }

    private void Start()
    {
        StartCoroutine(MoveToNextPoint());
    }

    /// <summary>
    /// 정해진 경로를 따라 몬스터가 움직임
    /// </summary>
    IEnumerator MoveToNextPoint()
    {
        while (_curPointIndex < _pathPoints.Length)
        {
            Transform targetPoint = _pathPoints[_curPointIndex + 1];
            while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, _model.MoveSpeed * Time.deltaTime);
                yield return null;
            }
            _curPointIndex++;
        }

        Destroy(gameObject);
    }
}