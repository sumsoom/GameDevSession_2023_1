using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlZombie : HealthControlBase
{
    // 출혈 효과 프리펩
    public GameObject bloodEffectPrefab;
    // Rigidbody 배열
    private Rigidbody[] rigidbodies;

    // 게임 오브젝트가 처음 실행될 때 한번 실행
    private void Awake()
    {
        // 현재 자식 오브젝트들에 있는 Rigidbody 컴포넌트들을 모두 가져와 배열에 담습니다.
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        // rigidbodies 배열을 순회하면서,
        foreach(Rigidbody rb in rigidbodies)
        {
            // 물리의 영향을 받지 않도록 합니다.
            rb.isKinematic = true;
        }
    }

    // 대미지를 가할 때 실행합니다.
    public override void OnDamage(HitInfo hitInfo, float damage)
    {
        // 출혈 효과 오브젝트를 생성합니다.
        GameObject be = Instantiate(bloodEffectPrefab,
            hitInfo.point,
            Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
        // 출혈 효과 오브젝트를 1초 뒤에 제거합니다.
        Destroy(be, 1f);

        // 부모 클래스의 OnDamage를 실행합니다.
        base.OnDamage(hitInfo, damage);
    }

    // 현재 오브젝트가 죽었을 때 실행합니다.
    public override void OnDead(HitInfo hitInfo)
    {
        // rigidbodies 배열을 순회하면서
        foreach(Rigidbody rb in rigidbodies)
        {
            // 물리 효과의 영향을 받도록합니다.
            rb.isKinematic = false;
        }

        // 총알에 맞은 콜라이더의 Rigidbody를 가져와서,
        // 총알에 맞은 지점에 힘을 가합니다.
        hitInfo.collider.attachedRigidbody.AddForceAtPosition(
            hitInfo.direction * 200f, hitInfo.point, ForceMode.Impulse); 

        // 부모 클래스의 OnDead를 실행합니다.
        base.OnDead(hitInfo);
    }

}
