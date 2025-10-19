using UnityEngine;

public class RangeAttack : StateMachineBehaviour
{
    private float _playTiming = 0.9f;

    private UnitController _controller;

    private bool _didAttack = false;
    private GameObject _cachedTarget;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller = animator.GetComponent<UnitController>();
        _didAttack = false;

        // 현재 타겟을 캐싱(도중에 사라질 수 있음)
        var col = _controller != null ? _controller.Target : null;
        _cachedTarget = col != null ? col.gameObject : null;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_didAttack || _controller == null)
            return;

        float progress = stateInfo.normalizedTime % 1f;

        if (progress >= _playTiming)
        {
            if (_cachedTarget != null)
            {
                _controller.RangeAttack(_cachedTarget);
            }
            else
            {
                Debug.LogWarning("이미 죽음.", animator);
            }
            _didAttack = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _didAttack = false;
        _cachedTarget = null;
    }
}
