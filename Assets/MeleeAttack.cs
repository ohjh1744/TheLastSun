using UnityEngine;

public class MeleeAttack : StateMachineBehaviour
{
    private float _playTiming = 0.9f;

    private UnitController _controller;
    private bool _didAttack = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller = animator.GetComponent<UnitController>();
        _didAttack = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_didAttack || _controller == null)
            return;

        float progress = stateInfo.normalizedTime % 1f;

        if (progress >= _playTiming)
        {
            _controller.MeleeAttack();
            _didAttack = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _didAttack = false;
    }
}
