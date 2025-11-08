using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    [SerializeField] private HeroData _heroData;

    public HeroData HeroData { get { return _heroData; } set { _heroData = value; } }

    [SerializeField] private AudioSource _audio;

    [SerializeField] private float _currentAttackTimer;

    [SerializeField] Transform _shootPoint;
    public Transform SHootPoint { get { return _shootPoint; } set { _shootPoint = value; } }
    public float CurrentAttackTimer { get { return _currentAttackTimer; } set { _currentAttackTimer = value; } }

    private void OnEnable()
    {
        SetSpawnSound();
    }

    private void SetSpawnSound()
    {
        if(PlayerController.Instance.PlayerData.IsSound == true)
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
        Vector3 center = transform.position;

        // 색상 지정
        Gizmos.color = Color.red;

        // 원 그리기
        Gizmos.DrawWireSphere(center, _heroData.AttackRange);
    }
}
