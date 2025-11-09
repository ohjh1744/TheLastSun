using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{

    [SerializeField] private HeroData _heroData;

    public HeroData HeroData { get { return _heroData; } set { _heroData = value; } }

    [SerializeField] private AudioSource _audio;

    private float _currentAttackTimer;
    public float CurrentAttackTimer { get { return _currentAttackTimer; } set { _currentAttackTimer = value; } }

    [SerializeField] Transform _attackPoint;
    public Transform AttackPoint { get { return _attackPoint; } set { _attackPoint = value; } }

    private void OnEnable()
    {
        SetSpawnSound();
    }


    private void OnDisable()
    {
        _currentAttackTimer = 0f;
    }

    private void SetSpawnSound()
    {
        if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            if(_heroData.SpawnClip != null)
            {
                _audio.PlayOneShot(_heroData.SpawnClip);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        // 중심: hero 위치
        Vector3 center = _attackPoint.position;

        // 색상 지정
        Gizmos.color = Color.red;

        // 원 그리기
        Gizmos.DrawWireSphere(center, _heroData.AttackRange);
    }
}
