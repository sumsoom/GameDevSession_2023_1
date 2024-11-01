using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UnityEvent�� �����ϱ� ���� ���
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    // onAnimationEvent��� �̺�Ʈ ����
    // <> �ȿ��� �Ű������� ���ϴ�.
    public UnityEvent<string> onAnimationEvent;

    // Animation �̺�Ʈ�� �߻��Ͽ��� �� ����Ǵ� �Լ�
    public void OnAnimationEvent(string eventName)
    {
        // �̺�Ʈ�� �߻��մϴ�.
        onAnimationEvent?.Invoke(eventName);
    }
}
