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
    [SerializeField] Collider2D Target => _enemyBuffer.Length > 0 ? _enemyBuffer[0] : null;
    private float _attackTimer = 0;

    // 이동중 공격 불가(수동 제어 중)
    private bool _isManualControl = false;

    // 이동할 목적지
    private Vector3 _targetPos;
    private bool _setTargetPos = false;

    [Header("Debug Helper")]
    // ---------- Debug ----------
    [SerializeField] bool _debugDraw = true;
    [SerializeField, Range(8, 128)] int _circleSegments = 32;
    [SerializeField] Color _idleColor = new Color(0f, 0.7f, 1f, 0.6f);
    [SerializeField] Color _moveColor = new Color(0.2f, 1f, 0.2f, 0.6f);
    [SerializeField] Color _attackColor = new Color(1f, 0.2f, 0.2f, 0.6f);
    [SerializeField] Color _enemyLineColor = Color.red;

    // 마지막 탐지된 적 수(디버그 표시용)
    private int _lastEnemyCount = 0;

    private void Awake()
    {
        _model = GetComponent<UnitModel>();
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
            transform.position,
            _model.AttackRange,
            _enemyBuffer,
            _model.TargetLayer
        );

        _lastEnemyCount = count;
        DebugDrawEnemyLines(count); // 디버그 라인

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
        if (_setTargetPos)
        {
            Vector3 direction = (_targetPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _targetPos);

            if (distance > 0.05f)
            {
                transform.position += direction * _model.MoveSpeed * Time.deltaTime;
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
            transform.position,
            _model.AttackRange *0.5f,
            _enemyBuffer,
            _model.TargetLayer
        );

        // 디버깅용 라인
        _lastEnemyCount = count;
        DebugDrawEnemyLines(count);

        if (count == 0)
        {
            CurrentState = _setTargetPos ? State.Move : State.Idle;
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _model.AttackDelay && Target != null)
        {
            ShootBullet(Target.transform.position);
            _attackTimer = 0;
        }
    }

    private void ShootBullet(Vector2 targetPos)
    {
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity, transform);
        bullet.GetComponent<Bullet>().Init(targetPos);
    }

    // ---------- Debug Methods ----------

    private void DebugDrawEnemyLines(int count)
    {
        if (!_debugDraw) return;
        for (int i = 0; i < count; i++)
        {
            var col = _enemyBuffer[i];
            if (col == null) continue;
            Debug.DrawLine(transform.position, col.transform.position, _enemyLineColor, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_debugDraw) return;

        // _model이 아직 Awake 전일 수 있으므로 null 체크 후 가져오기
        if (_model == null)
            _model = GetComponent<UnitModel>();

        float r = _model != null ? _model.AttackRange * 0.5f : 0f;

        // 상태별 색
        Color c = _idleColor;
        switch (CurrentState)
        {
            case State.Move: c = _moveColor; break;
            case State.Attack: c = _attackColor; break;
        }

        // 반투명 Wire Circle
        Gizmos.color = c;
        DrawWireCircle(transform.position, r, _circleSegments);

        // 현재 Manual 이동 타겟 표시
        if (_setTargetPos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_targetPos, 0.12f);
            Gizmos.DrawLine(transform.position, _targetPos);
        }

        // 탐지된 적 위치에 작은 점
        Gizmos.color = Color.red;
        for (int i = 0; i < _lastEnemyCount && i < _enemyBuffer.Length; i++)
        {
            var col = _enemyBuffer[i];
            if (col == null) continue;
            Gizmos.DrawSphere(col.transform.position, 0.08f);
        }
    }

    private void DrawWireCircle(Vector3 center, float radius, int segments)
    {
        if (radius <= 0f) return;
        float step = 2f * Mathf.PI / segments;
        Vector3 prev = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * radius;
        for (int i = 1; i <= segments; i++)
        {
            float a = step * i;
            Vector3 next = center + new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}