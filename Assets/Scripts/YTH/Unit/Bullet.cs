using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int _moveSpeed = 4;

    private Vector2 _targetPos;
    private Vector2 _moveDir;
    private bool _isInitialized = false;

    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (!_isInitialized) return;

        transform.position += (Vector3)(_moveDir * _moveSpeed * Time.deltaTime);
    }

    public void Init(Vector2 targetPos)
    {
        _targetPos = targetPos;
        _moveDir = ((Vector2)_targetPos - (Vector2)transform.position).normalized;
        _isInitialized = true;
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
