using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlPlayer : HealthControlBase
{
    // UIControl ������ �����մϴ�.
    private UIControl UIControl;

    // ���� ������Ʈ�� ó�� ���۵� �� �ѹ� ����
    private void Awake()
    {
        // ������ UIControl ������Ʈ�� ã�Ƽ� �����ɴϴ�.
        UIControl = FindObjectOfType<UIControl>();
    }

    // �� �����Ӹ��� ����
    private void Update()
    {
        // ü�� UI�� ������Ʈ �մϴ�.
        UIControl.UpdateHealth(health);
    }

    // ������� ���� �� �����մϴ�.
    public override void OnDamage(HitInfo hitInfo, float damage)
    {
        // UI�� ���� ȿ���� �����մϴ�.
        UIControl.UpdateHurtImage();

        // �θ� Ŭ������ OnDamage�� �����մϴ�.
        base.OnDamage(hitInfo, damage);
    }
}
