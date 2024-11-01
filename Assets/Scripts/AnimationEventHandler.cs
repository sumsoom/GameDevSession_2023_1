using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UnityEvent를 선언하기 위해 사용
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    // onAnimationEvent라는 이벤트 생성
    // <> 안에는 매개변수가 들어갑니다.
    public UnityEvent<string> onAnimationEvent;

    // Animation 이벤트가 발생하였을 때 실행되는 함수
    public void OnAnimationEvent(string eventName)
    {
        // 이벤트를 발생합니다.
        onAnimationEvent?.Invoke(eventName);
    }
}
