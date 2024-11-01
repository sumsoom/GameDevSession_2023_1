using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Hit 검출을 위한 사용자 정의 구조체
public struct HitInfo
{
    // 맞은 위치
    public Vector3 point;
    // 공격이 들어온 방향 벡터
    public Vector3 direction;
    // 법선 벡터
    public Vector3 normal;
    // 공격 당한 콜라이더
    public Collider collider;
}

public class HealthControlBase : MonoBehaviour
{
    // 남은 체력
    public float health = 100f;
    // 현재 죽은 상태인가?
    public bool isDead = false;

    public UnityEvent onDead;

    // 대미지를 가할 때 실행합니다.
    public virtual void OnDamage(HitInfo hitInfo, float damage)
    {
        // 만약 죽었다면, 
        if (isDead)
        {
            // 함수 종료
            return;
        }

        // 체력에서 대미지를 빼고,
        health -= damage;
        // 만약 남은 체력이 0 이하면?
        if (health <= 0)
        {
            // 사망 처리
            isDead = true;
            OnDead(hitInfo);
        }
    }

    // 현재 오브젝트가 죽었을 때 실행합니다.
    public virtual void OnDead(HitInfo hitInfo)
    {
        onDead?.Invoke();
    }
}

