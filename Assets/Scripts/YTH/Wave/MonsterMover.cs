using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterMover : MonoBehaviour
{
    public List<Transform> pathPoints;
    public float moveSpeed = 3f;
    private int currentPointIndex = 0;

    void Start()
    {
        if (pathPoints.Count > 0)
        {
            StartCoroutine(MoveToNextPoint());
        }
    }

    IEnumerator MoveToNextPoint()
    {
        while (currentPointIndex < pathPoints.Count)
        {
            Transform targetPoint = pathPoints[currentPointIndex];
            while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
                yield return null;
            }
            currentPointIndex++;
        }

        // 경로 끝에 도달했을 때 처리 (예: 적 파괴)
        Destroy(gameObject);
    }
}