using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroController : MonoBehaviour,IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    [SerializeField] private HeroData _heroData;

    public HeroData HeroData { get { return _heroData; } set { _heroData = value; } }

    [SerializeField] private AudioSource _audio;

    private float _currentAttackTimer;
    public float CurrentAttackTimer { get { return _currentAttackTimer; } set { _currentAttackTimer = value; } }

    [SerializeField] Transform _attackPoint;
    public Transform AttackPoint { get { return _attackPoint; } set { _attackPoint = value; } }

    private Vector2 _originalPosition;
    private Camera _cam;
    [SerializeField] private Vector2 _minBound;
    [SerializeField] private Vector2 _maxBound;

    private void Awake()
    {
        _cam = Camera.main;
    }

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

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("hhhi");
        _originalPosition = transform.position;  // 누른 순간 위치 저장
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("hhhia");
        // 마우스(터치)를 월드좌표로 변환
        Vector2 worldPos = _cam.ScreenToWorldPoint(eventData.position);

        // 사각형 영역 안으로 Clamp
        worldPos.x = Mathf.Clamp(worldPos.x, _minBound.x, _maxBound.x);
        worldPos.y = Mathf.Clamp(worldPos.y, _minBound.y, _maxBound.y);

        transform.position = worldPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
