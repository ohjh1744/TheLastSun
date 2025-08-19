using System.Collections;
using System.Collections.Generic;
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

    // 모델에서 값을 가져오므로 별도 변수 제거
    private float _lastAttackTime = 0f;
    private Collider2D[] _enemyBuffer = new Collider2D[5];

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

        // 예시: 마우스 클릭으로 이동 명령
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _targetPosition = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
            _hasMoveTarget = true;
            _currentState = State.Move;
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

        // 이동 중에도 적 탐지
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

        // AttackDelay를 초 단위로 환산하여 쿨타임 적용
        float attackCooldown = _model.AttackDelay * 0.01f;
        if (Time.time - _lastAttackTime > attackCooldown)
        {
            Collider2D target = _enemyBuffer[0];
            if (target != null)
            {
                ShootBullet(target.transform.position);
                _lastAttackTime = Time.time;
            }
        }
    }

    private void ShootBullet(Vector3 targetPos)
    {
        // Bullet 프리팹을 Resources 폴더에 두고 불러오는 예시
        GameObject bulletPrefab = Resources.Load<GameObject>("Bullet");
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Vector2 dir = (targetPos - transform.position).normalized;
            bullet.GetComponent<Rigidbody2D>().velocity = dir * 10f; // 속도 임의 지정
        }
    }
}


