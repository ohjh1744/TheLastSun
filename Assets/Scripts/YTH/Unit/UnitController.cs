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

    [SerializeField] State _currentState = State.Idle;

    private UnitModel _model;

    private Rigidbody2D _rigid;

    private Vector3 _targetPosition;

    private bool _hasMoveTarget = false;

    private Collider2D[] _enemyBuffer = new Collider2D[5];

    [SerializeField] Collider2D Target => _enemyBuffer.Length > 0 ? _enemyBuffer[0] : null;
    
    private float timter = 0;

    private void Awake()
    {
        _model = GetComponent<UnitModel>();
        _rigid = GetComponent<Rigidbody2D>();
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
    }

    /// <summary>
    /// Idle 상태 : 적 탐지 및 이동 명령 처리
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
    /// Move 상태 : 포인터핸들러를 통한 이동 상태
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
    /// Attack 상태 : 충돌체를 수를 배열로 저장하여 공격
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

        timter += Time.deltaTime;

        if (timter >= _model.AttackDelay && Target != null)
        {
            ShootBullet(Target.transform.position);
            timter = 0;
        }
    }

    private void ShootBullet(Vector2 targetPos)
    {       
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(targetPos);
    }

}