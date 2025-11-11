using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
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

    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _infoText;

    StringBuilder _sb = new StringBuilder();

    private bool _isMove;
    public bool IsMove { get { return _isMove; } set { _isMove = value; } }

    private int _curAtk;


    private void Awake()
    {
        _cam = Camera.main;
        SetSizeAttackRange();
    }

    private void OnEnable()
    {
        SetSpawnSound();
        
        //cur Atk 강화공격력 더해주기
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

    private void ShowInfo()
    {
        _sb.Clear();
        _sb.Append($"공격력 : {_heroData.BaseDamage} \n공격주기 : {_heroData.AttackDelay}\n공격대상 : {_heroData.MaxAttackCount} ");
        _infoText.SetText(_sb);
        _canvas.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _originalPosition = transform.position;
        _attackPoint.gameObject.SetActive(true);
        _isMove = true;
        ShowInfo();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 worldPos = _cam.ScreenToWorldPoint(eventData.position);

        worldPos.x = Mathf.Clamp(worldPos.x, _minBound.x, _maxBound.x);
        worldPos.y = Mathf.Clamp(worldPos.y, _minBound.y, _maxBound.y);

        transform.position = worldPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _attackPoint.gameObject.SetActive(false);
        _isMove = false;
        _canvas.SetActive(false);
    }

    private void SetSizeAttackRange()
    {
        float diameter = _heroData.AttackRange * 2f;

        // 부모 스케일을 고려해서 자식 스케일 계산
        Vector3 parentScale = transform.lossyScale;

        _attackPoint.transform.localScale = new Vector3(
            diameter / parentScale.x,
            diameter / parentScale.y,
            1f
        );
    }


}
