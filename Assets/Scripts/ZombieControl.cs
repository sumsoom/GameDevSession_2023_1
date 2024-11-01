using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieControl : MonoBehaviour
{
    //**************************************************

    // ���� ���󰡼� ������ Ÿ��
    public GameObject target;
    // Path finding�� ���� navigation ������Ʈ
    private NavMeshAgent nav;

    //**************************************************

    // ���� ���� �����ΰ�?
    private bool isAttacking = false;
    // ���� ������ �÷��̾���� HealthControl ����Ʈ
    private List<HealthControlBase> healthControls
         = new List<HealthControlBase>();

    //**************************************************

    // �ִϸ��̼��� �����ϱ� ���� ������Ʈ
    private Animator animator;

    //**************************************************

    // ���� ������Ʈ�� ó�� ���۵� �� �ѹ� ����
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // ���� ������Ʈ�� ó�� ���۵� �� �ѹ� ����
    // Start�� ��� �ٸ� ���ӿ�����Ʈ���� Awake�� ����� �Ŀ� ����ȴ�.
    private void Start()
    {
        // PathFindingRoutine�� �����մϴ�.
        StartCoroutine(nameof(PathFindingRoutine));
        // AttackRoutine�� �����մϴ�.
        StartCoroutine(nameof(AttackRoutine));
    }

    // �� �����Ӹ��� ����
    private void Update()
    {
        // ���� Ÿ���� �ִٸ�,
        if (target != null)
        {
            // Ÿ�ٰ��� �Ÿ��� ����մϴ�.
            float dist = Vector3.Distance(this.transform.position,
                target.transform.position);
            
            // ���� �� �Ÿ��� 3 ���϶��,
            if (dist <= 3f)
            {
                // Ÿ���� ���� �ٶ󺸴� ���Ϸ� ���� ����մϴ�.
                Vector3 rotation = Quaternion.LookRotation(
                    target.transform.position - this.transform.position).eulerAngles;

                // Y���� �����ϰ� 0���� �����մϴ�.
                // x, z�࿡ ���� �ִٸ� ĳ���Ͱ� ���̳� �ڷ� �������� ����
                rotation.x = 0f;
                rotation.z = 0f;

                // ĳ������ ȸ�� ���� �����մϴ�.
                this.transform.eulerAngles = rotation;
            }
        }
    }

    // PathFindingRoutine �ڷ�ƾ�� �����մϴ�.
    private IEnumerator PathFindingRoutine()
    {
        // ������ ���鼭
        while (true)
        {
            // Ÿ���� ���ٸ�,
            if (target == null)
            {
                // ���� �����ӱ��� ����մϴ�.
                yield return null;
            }
            // Ÿ���� �ִٸ�,
            else
            {
                // �׺���̼��� �������� Ÿ���� ��ġ�� �����մϴ�.
                nav.SetDestination(target.transform.position);

                // 0.333�� ��ŭ ����մϴ�.
                yield return new WaitForSeconds(0.333f);
            }
        }
    }

    // Ʈ���ſ� Ư�� �ݶ��̴��� ����Ǿ��ٸ� ����
    private void OnTriggerEnter(Collider other)
    {
        // ����� �ݶ��̴��� tag�� Player���,
        if (other.CompareTag("Player"))
        {
            // ����� �ݶ��̴����� HealthControlBase ������Ʈ�� ã���ϴ�.
            HealthControlBase hc =
                other.GetComponentInParent<HealthControlBase>();

            // �̶� hc�� �ִٸ�,
            if (hc != null)
            {
                // HealthControl ����Ʈ�� �߰��մϴ�.
                healthControls.Add(hc);
            }
        }
    }

    // Ʈ���ſ��� Ư�� �ݶ��̴��� ���������ٸ� ����
    private void OnTriggerExit(Collider other)
    {
        // ����� �ݶ��̴��� tag�� Player���,
        if (other.CompareTag("Player"))
        {
            // ����� �ݶ��̴����� HealthControlBase ������Ʈ�� ã���ϴ�.
            HealthControlBase hc =
                other.GetComponentInParent<HealthControlBase>();

            // �̶� hc�� �ִٸ�,
            if (hc != null)
            {
                // HealthControl ����Ʈ���� �����մϴ�.
                healthControls.Remove(hc);
            }
        }
    }

    // AttackRoutine �ڷ�ƾ�� �����մϴ�.
    private IEnumerator AttackRoutine()
    {
        // ������ ���鼭,
        while (true)
        {
            // ���� ����Ʈ�� count�� 0 ���϶��,
            // ��, ���� ������ ��ǥ�� ���ٸ�,
            if (healthControls.Count <= 0)
            {
                // ���� �����ӱ��� ����մϴ�.
                yield return null;
            }
            // ���� ������ ��ǥ�� �ִٸ�,
            else
            {
                // ����Ʈ�� ��ȸ�ϸ鼭,
                for(int i = healthControls.Count - 1; i >= 0; i--)
                {
                    // ���� �ش� ���Ұ� null�̶��
                    if (healthControls[i] == null)
                    {
                        // �ش� ���Ҹ� �����մϴ�.
                        healthControls.RemoveAt(i);
                    }
                }

                // �ִϸ������� Attack Ʈ���Ÿ� �����Ͽ�,
                // ���� �ִϸ��̼��� �����ݴϴ�.
                animator.SetTrigger("Attack");
                
                // ���� �ִϸ��̼ǰ� ��ũ�� ���߱� ���� 0.6�� ����մϴ�.
                yield return new WaitForSeconds(0.6f);

                // ���� ������ ��ǥ���� ��ȸ�ϸ鼭,
                foreach(HealthControlBase hc in healthControls)
                {
                    // �ش� ��ǥ�鿡 �������� ���մϴ�.
                    hc.OnDamage(new HitInfo(), 15f);    
                }

                // ���� ���ݱ��� 3�� ����մϴ�.
                yield return new WaitForSeconds(3f);           
            }
        }
    }

    // ���� ���� �׾��ٸ� ����
    public void OnDead()
    {
        // PathFindingRoutine�� �����մϴ�.
        StopCoroutine(nameof(PathFindingRoutine));
        // AttackRoutine�� �����մϴ�.
        StopCoroutine(nameof(AttackRoutine));

        nav.isStopped = true;

        // �ִϸ����͸� ��Ȱ��ȭ�մϴ�.
        animator.enabled = false;

        // ���� ���� ������Ʈ�� 5�ʵڿ� �����մϴ�.
        Destroy(this.gameObject, 5f);
    }
}
