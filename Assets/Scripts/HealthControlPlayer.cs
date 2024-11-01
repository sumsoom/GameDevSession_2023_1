using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlPlayer : HealthControlBase
{
    // UIControl 변수를 선언합니다.
    private UIControl UIControl;

    // 게임 오브젝트가 처음 시작될 때 한번 실행
    private void Awake()
    {
        // 씬에서 UIControl 컴포넌트를 찾아서 가져옵니다.
        UIControl = FindObjectOfType<UIControl>();
    }

    // 매 프레임마다 실행
    private void Update()
    {
        // 체력 UI를 업데이트 합니다.
        UIControl.UpdateHealth(health);
    }

    // 대미지를 가할 때 실행합니다.
    public override void OnDamage(HitInfo hitInfo, float damage)
    {
        // UI의 출혈 효과를 실행합니다.
        UIControl.UpdateHurtImage();

        // 부모 클래스의 OnDamage를 실행합니다.
        base.OnDamage(hitInfo, damage);
    }
}
