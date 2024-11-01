using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieControl : MonoBehaviour
{
    //**************************************************

    // 좀비가 따라가서 공격할 타겟
    public GameObject target;
    // Path finding을 위한 navigation 컴포넌트
    private NavMeshAgent nav;

    //**************************************************

    // 현재 공격 상태인가?
    private bool isAttacking = false;
    // 공격 가능한 플레이어들의 HealthControl 리스트
    private List<HealthControlBase> healthControls
         = new List<HealthControlBase>();

    //**************************************************

    // 애니메이션을 제어하기 위한 컴포넌트
    private Animator animator;

    //**************************************************

    // 게임 오브젝트가 처음 시작될 때 한번 실행
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // 게임 오브젝트가 처음 시작될 때 한번 실행
    // Start는 모든 다른 게임오브젝트들의 Awake가 실행된 후에 실행된다.
    private void Start()
    {
        // PathFindingRoutine을 실행합니다.
        StartCoroutine(nameof(PathFindingRoutine));
        // AttackRoutine을 실행합니다.
        StartCoroutine(nameof(AttackRoutine));
    }

    // 매 프레임마다 실행
    private void Update()
    {
        // 만약 타겟이 있다면,
        if (target != null)
        {
            // 타겟과의 거리를 계산합니다.
            float dist = Vector3.Distance(this.transform.position,
                target.transform.position);
            
            // 만약 그 거리가 3 이하라면,
            if (dist <= 3f)
            {
                // 타겟을 향해 바라보는 오일러 각을 계산합니다.
                Vector3 rotation = Quaternion.LookRotation(
                    target.transform.position - this.transform.position).eulerAngles;

                // Y축을 제외하고 0으로 설정합니다.
                // x, z축에 값이 있다면 캐릭터가 옆이나 뒤로 기울어지기 때문
                rotation.x = 0f;
                rotation.z = 0f;

                // 캐릭터의 회전 값을 설정합니다.
                this.transform.eulerAngles = rotation;
            }
        }
    }

    // PathFindingRoutine 코루틴을 선언합니다.
    private IEnumerator PathFindingRoutine()
    {
        // 루프를 돌면서
        while (true)
        {
            // 타겟이 없다면,
            if (target == null)
            {
                // 다음 프레임까지 대기합니다.
                yield return null;
            }
            // 타겟이 있다면,
            else
            {
                // 네비게이션의 목적지를 타겟의 위치로 설정합니다.
                nav.SetDestination(target.transform.position);

                // 0.333초 만큼 대기합니다.
                yield return new WaitForSeconds(0.333f);
            }
        }
    }

    // 트리거에 특정 콜라이더가 검출되었다면 실행
    private void OnTriggerEnter(Collider other)
    {
        // 검출된 콜라이더의 tag가 Player라면,
        if (other.CompareTag("Player"))
        {
            // 검출된 콜라이더에서 HealthControlBase 컴포넌트를 찾습니다.
            HealthControlBase hc =
                other.GetComponentInParent<HealthControlBase>();

            // 이때 hc가 있다면,
            if (hc != null)
            {
                // HealthControl 리스트에 추가합니다.
                healthControls.Add(hc);
            }
        }
    }

    // 트리거에서 특정 콜라이더가 빠져나갔다면 실행
    private void OnTriggerExit(Collider other)
    {
        // 검출된 콜라이더의 tag가 Player라면,
        if (other.CompareTag("Player"))
        {
            // 검출된 콜라이더에서 HealthControlBase 컴포넌트를 찾습니다.
            HealthControlBase hc =
                other.GetComponentInParent<HealthControlBase>();

            // 이때 hc가 있다면,
            if (hc != null)
            {
                // HealthControl 리스트에서 제거합니다.
                healthControls.Remove(hc);
            }
        }
    }

    // AttackRoutine 코루틴을 선언합니다.
    private IEnumerator AttackRoutine()
    {
        // 루프를 돌면서,
        while (true)
        {
            // 만약 리스트의 count가 0 이하라면,
            // 즉, 공격 가능한 목표가 없다면,
            if (healthControls.Count <= 0)
            {
                // 다음 프레임까지 대기합니다.
                yield return null;
            }
            // 공격 가능한 목표가 있다면,
            else
            {
                // 리스트를 순회하면서,
                for(int i = healthControls.Count - 1; i >= 0; i--)
                {
                    // 만약 해당 원소가 null이라면
                    if (healthControls[i] == null)
                    {
                        // 해당 원소를 제거합니다.
                        healthControls.RemoveAt(i);
                    }
                }

                // 애니메이터의 Attack 트리거를 실행하여,
                // 공격 애니메이션을 보여줍니다.
                animator.SetTrigger("Attack");
                
                // 공격 애니메이션과 싱크를 맞추기 위해 0.6초 대기합니다.
                yield return new WaitForSeconds(0.6f);

                // 공격 가능한 목표들을 순회하면서,
                foreach(HealthControlBase hc in healthControls)
                {
                    // 해당 목표들에 데미지를 가합니다.
                    hc.OnDamage(new HitInfo(), 15f);    
                }

                // 다음 공격까지 3초 대기합니다.
                yield return new WaitForSeconds(3f);           
            }
        }
    }

    // 만약 좀비가 죽었다면 실행
    public void OnDead()
    {
        // PathFindingRoutine을 정지합니다.
        StopCoroutine(nameof(PathFindingRoutine));
        // AttackRoutine을 정지합니다.
        StopCoroutine(nameof(AttackRoutine));

        nav.isStopped = true;

        // 애니메이터를 비활성화합니다.
        animator.enabled = false;

        // 현재 좀비 오브젝트를 5초뒤에 제거합니다.
        Destroy(this.gameObject, 5f);
    }
}
