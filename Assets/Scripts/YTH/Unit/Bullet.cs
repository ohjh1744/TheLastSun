using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int _moveSpeed = 4;

    private Vector2 _targetPos;
    private Vector2 _moveDir;
    private bool _isInitialized = false;

    public void Init(Vector2 targetPos)
    {
        _targetPos = targetPos;
        _moveDir = ((Vector2)_targetPos - (Vector2)transform.position).normalized;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        transform.position += (Vector3)(_moveDir * _moveSpeed * Time.deltaTime);

       /* // 목표 위치에 도달하면 파괴 (충돌 처리 전에 멈추지 않도록 약간의 거리 허용)
        if (Vector2.Distance(transform.position, _targetPos) < 0.1f)
        {
            Destroy(gameObject);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
           collision.GetComponent<MonsterController>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
