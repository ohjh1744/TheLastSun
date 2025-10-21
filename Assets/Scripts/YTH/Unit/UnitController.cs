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

    public State CurrentState = State.Idle;

    private UnitModel _model;

    // 공격 관련
    private Collider2D[] _enemyBuffer = new Collider2D[5];
    /*[SerializeField] Collider2D Target => _enemyBuffer.Length > 0 ? _enemyBuffer[0] : null;*/
    private float _attackTimer = 0;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    // 이동중 공격 불가(수동 제어 중)
    private bool _isManualControl = false;

    // 이동할 목적지
    private Vector3 _targetPos;
    private bool _setTargetPos = false;
    private float _moveSpeed = 3;

    // 마지막 탐지된 적 수
    private int _lastEnemyCount = 0;

    // 사분면 규칙 타겟 선택 (원점(0,0) 기준)
    public Collider2D Target
    {
        get
        {
            int count = _lastEnemyCount;
            if (count <= 0) return null;

            Vector3 myPos = transform.position;
            int quadrant = GetQuadrant(myPos); // 1~4

            Collider2D target = null;
            float keyY = 0f, keyX = 0f;

            for (int i = 0; i < count; i++)
            {
                var enemy = _enemyBuffer[i];
                if (enemy == null) continue;

                Vector3 enemyPos = enemy.transform.position;

                switch (quadrant)
                {
                    case 1:
                        // y 최소, x 최대
                        if (target == null || enemyPos.y < keyY || (Mathf.Approximately(enemyPos.y, keyY) && enemyPos.x > keyX))
                        { target = enemy; keyY = enemyPos.y; keyX = enemyPos.x; }
                        break;
                    case 2:
                        // y 최대, x 최대
                        if (target == null || enemyPos.y > keyY || (Mathf.Approximately(enemyPos.y, keyY) && enemyPos.x > keyX))
                        { target = enemy; keyY = enemyPos.y; keyX = enemyPos.x; }
                        break;
                    case 3:
                        // y 최대, x 최소
                        if (target == null || enemyPos.y > keyY || (Mathf.Approximately(enemyPos.y, keyY) && enemyPos.x < keyX))
                        { target = enemy; keyY = enemyPos.y; keyX = enemyPos.x; }
                        break;
                    case 4:
                        // y 최소, x 최소
                        if (target == null || enemyPos.y < keyY || (Mathf.Approximately(enemyPos.y, keyY) && enemyPos.x < keyX))
                        { target = enemy; keyY = enemyPos.y; keyX = enemyPos.x; }
                        break;
                }
            }

            return target;
        }
    }

    private Vector2[] _godAttackZone = new Vector2[]
    {
        new Vector2(-3, 2),
        new Vector2(-3, 0),
        new Vector2(-3, -2),

        new Vector2(3, 2),
        new Vector2(3, 0),
        new Vector2(3, -2),
    };

    private int GetQuadrant(Vector3 standard)
    {
        if (standard.x >= 0f && standard.y >= 0f) return 1;
        if (standard.x < 0f && standard.y >= 0f) return 2;
        if (standard.x < 0f && standard.y < 0f) return 3;
        return 4; // pos.x >= 0 && pos.y < 0
    }

    private void Awake()
    {
        _model = GetComponent<UnitModel>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // 추가: 스프라이트 찾기(자식 포함)
    }

    private void Update()
    {
        switch (CurrentState)
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
    /// 드래그로 유닛을 선택했을 때 호출 : 공격 중지 + 적 탐지 막기
    /// </summary>
    public void BeginManualSelect()
    {
        _isManualControl = true;
        _setTargetPos = false;           // 아직 이동 위치 확정 전
        CurrentState = State.Idle;       // 대기 상태(공격 금지)
    }

    /// <summary>
    /// 드래그 끝난 위치로 이동 시작
    /// </summary>
    public void SetManualMoveTarget(Vector3 worldPos)
    {
        _targetPos = worldPos;
        _setTargetPos = true;
        CurrentState = State.Move;
    }

    /// <summary>
    /// Idle 상태 : 적 탐지
    /// </summary>
    private void HandleIdle()
    {
        // 수동 제어 중 & 아직 이동 타겟을 지정하지 않은 상태 -> 공격/탐지 차단
        if (_isManualControl && !_setTargetPos)
            return;

        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position + new Vector3(0, 0.5f),
            _model.AttackRange * 0.5f * 0.6f,
            _enemyBuffer,
            _model.TargetLayer
        );

        _lastEnemyCount = count;

        if (count > 0 && !_isManualControl)          // 수동 이동 완료 전에는 공격 상태로 못 들어감
        {
            CurrentState = State.Attack;
        }
        else if (_setTargetPos)
        {
            CurrentState = State.Move;
        }
    }

    /// <summary>
    /// Move 상태 : 목표 지점으로 이동
    /// </summary>
    private void HandleMove()
    {
        // 추가: 월드 좌표 기준 x>0이면 플립, 이하면 해제
        bool flip = transform.position.x > 0f;
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = flip;
        }
        else
        {
            // SpriteRenderer가 없을 때 폴백: localScale.x 부호 변경
            var ls = transform.localScale;
            float sign = flip ? -1f : 1f;
            ls.x = Mathf.Abs(ls.x) * sign;
            transform.localScale = ls;
        }

        if (_setTargetPos)
        {
            Vector3 direction = (_targetPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _targetPos);

            if (distance > 0.05f)
            {
                transform.position += direction * _moveSpeed * Time.deltaTime;
            }
            else
            {
                // 도착
                _setTargetPos = false;
                // 수동 제어 종료 → 이후부터 공격 가능
                if (_isManualControl)
                    _isManualControl = false;

                CurrentState = State.Idle;
            }
        }
        else
        {
            // 이동 타겟 상실 시 Idle 복귀
            CurrentState = State.Idle;
        }
    }

    /// <summary>
    /// Attack 상태
    /// </summary>
    private void HandleAttack()
    {
        // 수동 제어 중이라면(이동 완료 전) 즉시 Idle/Move로 되돌림
        if (_isManualControl)
        {
            CurrentState = _setTargetPos ? State.Move : State.Idle;
            return;
        }

        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position + new Vector3(0, 0.5f),
            _model.AttackRange * 0.5f * 0.6f,
            _enemyBuffer,
            _model.TargetLayer
        );

        _lastEnemyCount = count;

        if (count == 0)
        {
            CurrentState = _setTargetPos ? State.Move : State.Idle;
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _model.AttackDelay && Target != null)
        {
            _animator.SetTrigger("Attack");
            _attackTimer = 0;
        }
    }

    public void MeleeAttack()
    {
        for (int i = 0; i < _lastEnemyCount; i++)
        {
            var col = _enemyBuffer[i];
            if (col == null) continue;
            col.GetComponent<MonsterController>()?.TakeDamage(_model.Damage);
        }
    }

    public void RangeAttack(GameObject target)
    {
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity, transform);
        bullet.GetComponent<Bullet>().Init(target);
    }

    bool _initGodAttack = false;
    Coroutine _godAttackCoroutine;
    public void GodAttack()
    {
        if (_initGodAttack == false)
        {
            InitGodAttack();
        }

        if (godRoutine == null)
        {
            godRoutine = StartCoroutine(GodAttackCoroutine());
        }
    }

    List<GameObject> godProjectiles = new List<GameObject>();
    public void InitGodAttack()
    {
        for (int i = 0; i < _godAttackZone.Length; i++)
        {
            Vector2 attackPos = _godAttackZone[i];
            GameObject instance = Instantiate(_bulletPrefab, attackPos, Quaternion.identity);
            godProjectiles.Add(instance);
        }
        _initGodAttack = true;
    }

    Coroutine godRoutine;
    IEnumerator GodAttackCoroutine()
    {
        while (true)
        {
            foreach (var projectile in godProjectiles)
            {
                projectile.SetActive(true);
            }

            yield return new WaitForSeconds(2f);

            foreach (var projectile in godProjectiles)
            {
                projectile.SetActive(false);
            }

            yield return new WaitForSeconds(2f);
        }
    }
}