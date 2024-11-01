using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlZombie : HealthControlBase
{
    // ���� ȿ�� ������
    public GameObject bloodEffectPrefab;
    // Rigidbody �迭
    private Rigidbody[] rigidbodies;

    // ���� ������Ʈ�� ó�� ����� �� �ѹ� ����
    private void Awake()
    {
        // ���� �ڽ� ������Ʈ�鿡 �ִ� Rigidbody ������Ʈ���� ��� ������ �迭�� ����ϴ�.
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        // rigidbodies �迭�� ��ȸ�ϸ鼭,
        foreach(Rigidbody rb in rigidbodies)
        {
            // ������ ������ ���� �ʵ��� �մϴ�.
            rb.isKinematic = true;
        }
    }

    // ������� ���� �� �����մϴ�.
    public override void OnDamage(HitInfo hitInfo, float damage)
    {
        // ���� ȿ�� ������Ʈ�� �����մϴ�.
        GameObject be = Instantiate(bloodEffectPrefab,
            hitInfo.point,
            Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
        // ���� ȿ�� ������Ʈ�� 1�� �ڿ� �����մϴ�.
        Destroy(be, 1f);

        // �θ� Ŭ������ OnDamage�� �����մϴ�.
        base.OnDamage(hitInfo, damage);
    }

    // ���� ������Ʈ�� �׾��� �� �����մϴ�.
    public override void OnDead(HitInfo hitInfo)
    {
        // rigidbodies �迭�� ��ȸ�ϸ鼭
        foreach(Rigidbody rb in rigidbodies)
        {
            // ���� ȿ���� ������ �޵����մϴ�.
            rb.isKinematic = false;
        }

        // �Ѿ˿� ���� �ݶ��̴��� Rigidbody�� �����ͼ�,
        // �Ѿ˿� ���� ������ ���� ���մϴ�.
        hitInfo.collider.attachedRigidbody.AddForceAtPosition(
            hitInfo.direction * 200f, hitInfo.point, ForceMode.Impulse); 

        // �θ� Ŭ������ OnDead�� �����մϴ�.
        base.OnDead(hitInfo);
    }

}
