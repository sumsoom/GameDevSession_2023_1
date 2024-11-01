using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    //**************************************************

    // 크로스헤어의 Transform 배열
    public Transform[] crosshairs;
    // 남은 탄약 카운트 텍스트
    public Text ammoCountText;

    //**************************************************

    // 출혈 효과 이미지
    public Image hurtImage;
    // 체력 텍스트
    public Text healthText;

    //**************************************************

    public RectTransform canvasTransform;
    public GameObject nicknamePrefab;


    // 크로스헤어의 위치를 업데이트합니다.
    public void UpdateCrosshairPosition(Vector3 worldPosition)
    {
        // 화면의 정 중앙 좌표를 구합니다.
        Vector3 midPoint = new Vector3(Screen.width * 0.5f, 
            Screen.height * 0.5f);
        // 월드 좌표가 실제 화면에는 어느 위치에 찍히는지 변환합니다.
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        // 화면에 찍히는 위치와, 정 중앙 좌표 사이의 거리를 구합니다.
        float distance = screenPosition.y - midPoint.y;

        // 크로스헤어들의 위치를 설정합니다.
        crosshairs[0].position = midPoint + Vector3.up * distance;
        crosshairs[1].position = midPoint - Vector3.up * distance;
        crosshairs[2].position = midPoint + Vector3.right * distance;
        crosshairs[3].position = midPoint - Vector3.right * distance;
    }

    // 남은 탄약의 개수를 업데이트합니다.
    public void UpdateAmmoCount(int ammoLeft, int magazineLeft)
    {
        // 텍스트를 설정합니다.
        ammoCountText.text = $"{ammoLeft} / {magazineLeft}";
    }

    // 체력 텍스트를 업데이트 합니다.
    public void UpdateHealth(float health)
    {
        // 체력 텍스트의 텍스트를 새로 씁니다.
        healthText.text = $"HP {health}";
    }

    // 출혈 효과 이미지를 실행합니다.
    public void UpdateHurtImage()
    {
        // HurtImageRoutine을 정지합니다.
        StopCoroutine(nameof(HurtImageRoutine));
        // HurtImageRoutine을 새로 실행합니다.
        StartCoroutine(nameof(HurtImageRoutine));
    }

    // HurtImageRoutine 코루틴을 선언합니다.
    private IEnumerator HurtImageRoutine()
    {
        // 출혈 효과 이미지의 Color를 가져옵니다.
        Color color = hurtImage.color;
        // 해당 Color의 알파값을 0.5로 설정하고,
        color.a = 0.5f;
        // 다시 출혈 효과 이미지의 Color를 방금 만든 color로 바꿔줍니다.
        hurtImage.color = color;

        // 만약 알파값이 0 초과라면 루프를 돕니다.
        while (color.a > 0f)
        {
            // 알파값을 아주 작은 값만큼 빼고,
            color.a -= Time.deltaTime;
            // 출혈 효과 이미지의 Color를 다시 설정해줍니다.
            hurtImage.color = color;

            // 다음 프레임까지 대기합니다.
            yield return null;
        }
    }

    public GameObject CreateNickname(string nickname)
    {
        GameObject nick = Instantiate(nicknamePrefab, canvasTransform);
        nick.GetComponent<Text>().text = nickname;

        return nick;
    }

    public void UpdateNicknameTransform(GameObject nickname, Vector3 worldPosition)
    {
        Vector2 mid = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);


        if (screenPos.z > 0f)
        {
            screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);
        }
        else
        {
            screenPos = Vector3.one * 99999f;
        }

        nickname.transform.position = screenPos;


    }

}

