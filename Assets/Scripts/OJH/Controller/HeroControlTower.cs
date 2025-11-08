using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControlTower : MonoBehaviour
{
    //소환된 영웅들
    private List<HeroController> _heroControllers = new List<HeroController>();
    private List<Animator> _heroAnimators = new List<Animator>();
    private List<SpriteRenderer> _heroSpriteRenderers = new List<SpriteRenderer>();

    // OverlapSphereNonAlloc 을 위한 캐시 (쓰레기 안 만듦)
    private Collider2D[] _detectResults = new Collider2D[100];

    [SerializeField] private LayerMask _enemyLayerMask;

    private int _unitAttackhash = Animator.StringToHash("Attack");

    private void Update()
    {
        //게임상태가 플레이가 아니거나, 네트워크문제있으면 return.
        if (InGameManager.Instance.GameState != EGameState.Play || NetworkCheckManager.Instance.IsConnected == false)
            return;

        for (int i = 0; i < _heroControllers.Count; i++)
        {
            //만약 살아있다면
            if (_heroControllers[i].gameObject.activeSelf == true)
            {
                _heroControllers[i].CurrentAttackTimer -= Time.deltaTime;
                if (_heroControllers[i].CurrentAttackTimer <= 0)
                {
                    TryAttack(_heroControllers[i], i);
                    _heroControllers[i].CurrentAttackTimer = _heroControllers[i].HeroData.AttackDelay;
                }
            }
        }
    }

    private void TryAttack(HeroController hero, int index)
    {
        int count = Physics2D.OverlapCircleNonAlloc(hero.transform.position, hero.HeroData.AttackRange, _detectResults, _enemyLayerMask);

        if (count == 0)
            return;

        // 가장 가까운 적 찾기
        Transform nearest = null;
        float nearestDist = Mathf.Infinity;
        Vector3 heroPos = hero.transform.position;

        for (int i = 0; i < count; i++)
        {
            float dist = Vector3.SqrMagnitude(_detectResults[i].transform.position - heroPos);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = _detectResults[i].transform;
            }
        }

        //전사나 신이면 따른 코드 추가
        if(hero.HeroData.HeroProjectileIndex == EProjectilePool.Null)
        {
            return;
        }

        if (nearest != null)
            ShootProjectile(hero, nearest, index);
    }

    private void ShootProjectile(HeroController hero, Transform target, int index)
    {
        GameObject proj = ObjectPoolManager.Instance.GetObject(ObjectPoolManager.Instance.ProjectilePools, ObjectPoolManager.Instance.Projectiles, (int)hero.HeroData.HeroProjectileIndex);
        proj.transform.DOKill();
        proj.transform.position = hero.SHootPoint.position;

        //투사체 방향에 따른 각도 계산
        Vector2 direction = (target.position - proj.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 영웅 flip처리     
        if (_heroSpriteRenderers[index] != null)
        {
            _heroSpriteRenderers[index].flipX = direction.x > 0; // 왼쪽이면 flip
        }

        //영웅 애니메이션 처리
        _heroAnimators[index].Play(_unitAttackhash, -1, 0);

        proj.transform
            .DOMove(target.position, 0.15f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                target.GetComponent<IDamagable>().TakeDamage(hero.HeroData.BaseDamage);
                proj.SetActive(false);
                Debug.Log($"{proj.activeSelf}프로젝트 끄기!");
            });
    }

    public void OnHeroActivated(GameObject heroObj)
    {
        HeroController heroController = heroObj.GetComponent<HeroController>();
        Animator heroAnimator = heroObj.GetComponent<Animator>();
        SpriteRenderer heroSR = heroObj.GetComponent<SpriteRenderer>();

        if (_heroControllers.Contains(heroController) == false)
        {
            _heroControllers.Add(heroController);
        }
        if(_heroAnimators.Contains(heroAnimator) == false)
        {
            _heroAnimators.Add(heroAnimator);
        }
        if (_heroSpriteRenderers.Contains(heroSR) == false)
        {
            _heroSpriteRenderers.Add(heroSR);
        }
    }


}
