using UnityEngine;
public enum State
{
    Idle,
    Move,
    Attack
}

public class UnitController : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;

    private State _currentState = State.Idle;

    private UnitModel _model;

    private Rigidbody2D _rigidbody2D;

    private Vector3 _targetPosition;

    private bool _hasMoveTarget = false;

    private float _lastAttackTime = 0f;

    private Collider2D[] _enemyBuffer = new Collider2D[5];

    private Collider2D _target;
    public Collider2D Target => _target;

    private void Awake()
    {
        _model = GetComponent<UnitModel>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Move:
                HandleMove();
                break;
            case State.Attack:
                HandleAttack();
                break;
        }

        // ����: ���콺 Ŭ������ �̵� ���
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _targetPosition = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
            _hasMoveTarget = true;
            _currentState = State.Move;
        }
    }

    /// <summary>
    /// Idle ���� : �� Ž�� �� �̵� ��� ó��
    /// </summary>
    private void HandleIdle()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            _model.AttackRange,
            _enemyBuffer,
            _model.TargetLayer
        );
        if (count > 0)
        {
            _currentState = State.Attack;
        }
        else if (_hasMoveTarget)
        {
            _currentState = State.Move;
        }
    }

    /// <summary>
    /// Move ���� : �������ڵ鷯�� ���� �̵� ����
    /// </summary>
    private void HandleMove()
    {
        if (_hasMoveTarget)
        {
            Vector3 direction = (_targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _targetPosition);

            if (distance > 0.1f)
            {
                transform.position += direction * _model.MoveSpeed * Time.deltaTime;
            }
            else
            {
                _hasMoveTarget = false;
                _currentState = State.Idle;
            }
        }
    }

    /// <summary>
    /// Attack ���� : �浹ü�� ���� �迭�� �����Ͽ� ����
    /// </summary>
    private void HandleAttack()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            _model.AttackRange,
            _enemyBuffer,
            _model.TargetLayer
        );
        if (count == 0)
        {
            _currentState = _hasMoveTarget ? State.Move : State.Idle;
            return;
        }

        // AttackDelay�� �� ������ ȯ���Ͽ� ��Ÿ�� ����
        float attackCooldown = _model.AttackDelay * 0.01f;
        if (Time.time - _lastAttackTime > attackCooldown)
        {
            _target = _enemyBuffer[0];
            if (_target != null)
            {
                ShootBullet(_target.transform.position);
                _lastAttackTime = Time.time;
            }
        }
    }

    private void ShootBullet(Vector2 targetPos)
    {        // Bullet �������� Resources ������ �ΰ� �ҷ����� ����
        GameObject bullet = Instantiate(_bulletPrefab, new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z), Quaternion.identity, transform);
        Vector2 dir = targetPos;
    }
      
}


