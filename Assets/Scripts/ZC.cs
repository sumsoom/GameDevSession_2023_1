using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZC : MonoBehaviour
{
    public GameObject target;
    private NavMeshAgent nav;

    private bool isAttacking = false;
    private List<HealthControlBase> healthControls
        = new List<HealthControlBase>();
    private Animator animator;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(nameof(PathFindingRoutine));
        StartCoroutine(nameof(AttackRoutine));
    }

    private void Update()
    {
        if(target!= null)
        {
            float dist = Vector3.Distance(this.transform.position, target.transform.position);
            if(dist <= 3f)
            {
                Vector3 rotation = Quaternion.LookRotation(target.transform.position - this.transform.position).eulerAngles;
                rotation.x = 0f;
                rotation.z = 0f;

                this.transform.eulerAngles = rotation;
            }
        
        }
    }

    private IEnumerator PathFindingRoutine()
    {
        while (true)
        {
            if (target == null)
            {
                yield return null;
            }
            else
            {
                nav.SetDestination(target.transform.position);
                yield return new WaitForSeconds(0.333f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthControlBase hc =
                other.GetComponentInParent<HealthControlBase>();
            if (hc != null)
            {
                healthControls.Add(hc);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthControlBase hc =
                other.GetComponentInParent<HealthControlBase>();
            if (hc != null)
            {
                healthControls.Remove(hc);
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (healthControls.Count <= 0)
            {
                yield return null;
            }
            else
            {
                for(int i=healthControls.Count-1; i>=0; i--)
                {
                    if(healthControls[i] == null)
                    {
                        healthControls.RemoveAt(i);
                    }
                }

                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(0, 6f);

                foreach(HealthControlBase hc in healthControls)
                {
                    hc.OnDamage(new HitInfo(), 15f);
                }
                yield return new WaitForSeconds(3f);

            }
        }
    }

    public void OnDead()
    {
        StopCoroutine(nameof(PathFindingRoutine));
        StopCoroutine(nameof(AttackRoutine));

        animator.enabled = false;

        Destroy(this.gameObject, 5f);
    }
}
