using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroControlTower : MonoBehaviour
{
    //소환된 영웅들
    private List<(HeroController, EHeroPool)> _heroControllers = new List<(HeroController, EHeroPool)>();
    private List<Animator> _heroAnimators = new List<Animator>();
    private List<SpriteRenderer> _heroSpriteRenderers = new List<SpriteRenderer>();
    private List<AudioSource> _heroAudios = new List<AudioSource>();
    [SerializeField] private int[] _heroAttackNumForType;

    // OverlapSphereNonAlloc 을 위한 캐시 (쓰레기 안 만듦)
    private Collider2D[] _detectResults = new Collider2D[100];

    [SerializeField] private LayerMask _enemyLayerMask;
    private int _unitAttackhash = Animator.StringToHash("Attack");

    [Header("유닛별 공격 사운드 개수")]
    [SerializeField] private int _maxAttackSfxCount;

    private void Update()
    {
        //게임상태가 플레이가 아니거나, 네트워크문제있으면 return.
        if (InGameManager.Instance.GameState != EGameState.Play || NetworkCheckManager.Instance.IsConnected == false)
            return;

        for (int i = 0; i < _heroControllers.Count; i++)
        {
            //만약 살아있고, 이동중이 아니라면
            if (_heroControllers[i].Item1.gameObject.activeSelf == true && _heroControllers[i].Item1.IsMove == false)
            {
                _heroControllers[i].Item1.CurrentAttackTimer -= Time.deltaTime;
                if (_heroControllers[i].Item1.CurrentAttackTimer <= 0)
                {
                    TryAttack(_heroControllers[i].Item1, i, _heroControllers[i].Item2);
                }
            }
        }
    }

    private void TryAttack(HeroController hero, int index, EHeroPool heroType)
    {
        int count = Physics2D.OverlapCircleNonAlloc(hero.AttackPoint.position, hero.HeroData.AttackRange, _detectResults, _enemyLayerMask);

        if (count == 0)
            return;
        //공격하는 유닛 수 증가
        _heroAttackNumForType[(int)heroType]++;

        // 거리 계산용 리스트 (struct로 할당 최소화)
        List<(Transform target, float dist)> detectedList = new List<(Transform, float)>(count);

        //거리계산해서 리스트에 저장
        for (int i = 0; i < count; i++)
        {
            float dist = Vector3.SqrMagnitude(_detectResults[i].transform.position - hero.AttackPoint.position);
            detectedList.Add((_detectResults[i].transform, dist));
        }

        // 거리순 정렬
        detectedList.Sort((a, b) => a.dist.CompareTo(b.dist));

        // 앞에서부터 maxAttackCount 만큼 가져오기
        int attackCount = Mathf.Min(hero.HeroData.MaxAttackCount, count);

        //사운드
        if (_heroAttackNumForType[(int)heroType] <= _maxAttackSfxCount && PlayerController.Instance.PlayerData.IsSound == true)
        {
            _heroAudios[index].PlayOneShot(hero.HeroData.AttackClip);
        }

        for (int i = 0; i < attackCount; i++)
        {
            Transform target = detectedList[i].target;

            if (hero.HeroData.HeroProjectileIndex == EProjectilePool.Null)
                HitWithOutProjectile(hero, target, index, heroType);
            else if (hero.HeroData.HeroProjectileIndex == EProjectilePool.God_1 || hero.HeroData.HeroProjectileIndex == EProjectilePool.God_2 || hero.HeroData.HeroProjectileIndex == EProjectilePool.God_3)
                HitWithProjectile(hero, target, index, heroType);
            else
                ShootProjectile(hero, target, index, heroType);
        }

        //1초후에 heroAttackNumForType[(int)heroType] 감소
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _heroAttackNumForType[(int)heroType]--;
            if (_heroAttackNumForType[(int)heroType] < 0)
                _heroAttackNumForType[(int)heroType] = 0; // 음수 방지
        });


        // 공격 후 타이머 초기화
        hero.CurrentAttackTimer = hero.HeroData.AttackDelay;
    }

    //투사체 없이 공격
    private void HitWithOutProjectile(HeroController hero, Transform target, int index, EHeroPool heroType)
    {
        //투사체 방향에 따른 각도 계산
        Vector2 direction = (target.position - hero.AttackPoint.position).normalized;

        // 영웅 flip처리     
        _heroSpriteRenderers[index].flipX = direction.x > 0; // 왼쪽이면 flip


        //영웅 애니메이션 처리
        _heroAnimators[index].Play(_unitAttackhash, -1, 0);

        if (target.gameObject.activeSelf == true)
            target.GetComponent<IDamagable>().TakeDamage(hero.CurAtk);

    }

    //투사체 즉시 공격
    private void HitWithProjectile(HeroController hero, Transform target, int index, EHeroPool heroType)
    {
        //영웅 애니메이션 처리
        _heroAnimators[index].Play(_unitAttackhash, -1, 0);

        GameObject proj = ObjectPoolManager.Instance.GetObject(ObjectPoolManager.Instance.ProjectilePools, ObjectPoolManager.Instance.Projectiles, (int)hero.HeroData.HeroProjectileIndex);

        proj.transform.position = target.position;

        if (target.gameObject.activeSelf == true)
            target.GetComponent<IDamagable>().TakeDamage(hero.CurAtk);

        DOVirtual.DelayedCall(1f, () =>
        {
            proj.SetActive(false);
        });

    }

    //투사체 날리는 공격
    private void ShootProjectile(HeroController hero, Transform target, int index, EHeroPool heroType)
    {
        GameObject proj = ObjectPoolManager.Instance.GetObject(ObjectPoolManager.Instance.ProjectilePools, ObjectPoolManager.Instance.Projectiles, (int)hero.HeroData.HeroProjectileIndex);
        proj.transform.DOKill();
        proj.transform.position = hero.AttackPoint.position;

        //투사체 방향에 따른 각도 계산
        Vector2 direction = (target.position - proj.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 영웅 flip처리     
        _heroSpriteRenderers[index].flipX = direction.x > 0; // 왼쪽이면 flip

        //영웅 애니메이션 처리
        _heroAnimators[index].Play(_unitAttackhash, -1, 0);


        if (target.gameObject.activeSelf == true)
        {
            proj.transform
               .DOMove(target.position, 0.15f)
               .SetEase(Ease.Linear)
               .OnComplete(() =>
               {
                   if(target.gameObject.activeSelf == true)
                   {
                       target.GetComponent<IDamagable>().TakeDamage(hero.CurAtk);
                   }

                   proj.SetActive(false);
               });
        }

    }

    public void OnHeroActivated(GameObject heroObj, EHeroPool heroType)
    {
        HeroController heroController = heroObj.GetComponent<HeroController>();
        Animator heroAnimator = heroObj.GetComponent<Animator>();
        SpriteRenderer heroSR = heroObj.GetComponent<SpriteRenderer>();
        AudioSource heroAudio = heroObj.GetComponent<AudioSource>();

        if (!_heroControllers.Exists(x => x.Item1 == heroController))
        {
            _heroControllers.Add((heroController, heroType));
        }
        if (_heroAnimators.Contains(heroAnimator) == false)
        {
            _heroAnimators.Add(heroAnimator);
        }
        if (_heroSpriteRenderers.Contains(heroSR) == false)
        {
            _heroSpriteRenderers.Add(heroSR);
        }
        if (_heroAudios.Contains(heroAudio) == false)
        {
            _heroAudios.Add(heroAudio);
        }
    }


}
