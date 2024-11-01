using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Hit ������ ���� ����� ���� ����ü
public struct HitInfo
{
    // ���� ��ġ
    public Vector3 point;
    // ������ ���� ���� ����
    public Vector3 direction;
    // ���� ����
    public Vector3 normal;
    // ���� ���� �ݶ��̴�
    public Collider collider;
}

public class HealthControlBase : MonoBehaviour
{
    // ���� ü��
    public float health = 100f;
    // ���� ���� �����ΰ�?
    public bool isDead = false;

    public UnityEvent onDead;

    // ������� ���� �� �����մϴ�.
    public virtual void OnDamage(HitInfo hitInfo, float damage)
    {
        // ���� �׾��ٸ�, 
        if (isDead)
        {
            // �Լ� ����
            return;
        }

        // ü�¿��� ������� ����,
        health -= damage;
        // ���� ���� ü���� 0 ���ϸ�?
        if (health <= 0)
        {
            // ��� ó��
            isDead = true;
            OnDead(hitInfo);
        }
    }

    // ���� ������Ʈ�� �׾��� �� �����մϴ�.
    public virtual void OnDead(HitInfo hitInfo)
    {
        onDead?.Invoke();
    }
}

