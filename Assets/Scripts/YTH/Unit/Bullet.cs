using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int _moveSpeed = 4;

    private GameObject _target;

    private bool _isInitialized = false;

    private UnitModel _unitModel;

    private void Awake()
    {
        _unitModel = GetComponentInParent<UnitModel>();

    }
    private void Start()
    {
        if (_unitModel.Rank != Rank.God)
        {
            Destroy(gameObject, 2f);
        }
    }

    private void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 toTarget = _target.transform.position - transform.position;
        if (toTarget.sqrMagnitude < 0.0001f)
            return;

        Vector3 dir = toTarget.normalized;

        transform.right = dir;

        transform.position += dir * _moveSpeed * Time.deltaTime;
    }

    public void Init(GameObject target)
    {
        _target = target;
        _isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            collision.GetComponent<MonsterController>()?.TakeDamage(_unitModel.Damage);
            Destroy(gameObject);
        }
    }
}
