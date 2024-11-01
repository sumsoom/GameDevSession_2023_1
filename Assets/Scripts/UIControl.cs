using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    //**************************************************

    // ũ�ν������ Transform �迭
    public Transform[] crosshairs;
    // ���� ź�� ī��Ʈ �ؽ�Ʈ
    public Text ammoCountText;

    //**************************************************

    // ���� ȿ�� �̹���
    public Image hurtImage;
    // ü�� �ؽ�Ʈ
    public Text healthText;

    //**************************************************

    public RectTransform canvasTransform;
    public GameObject nicknamePrefab;


    // ũ�ν������ ��ġ�� ������Ʈ�մϴ�.
    public void UpdateCrosshairPosition(Vector3 worldPosition)
    {
        // ȭ���� �� �߾� ��ǥ�� ���մϴ�.
        Vector3 midPoint = new Vector3(Screen.width * 0.5f, 
            Screen.height * 0.5f);
        // ���� ��ǥ�� ���� ȭ�鿡�� ��� ��ġ�� �������� ��ȯ�մϴ�.
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        // ȭ�鿡 ������ ��ġ��, �� �߾� ��ǥ ������ �Ÿ��� ���մϴ�.
        float distance = screenPosition.y - midPoint.y;

        // ũ�ν������� ��ġ�� �����մϴ�.
        crosshairs[0].position = midPoint + Vector3.up * distance;
        crosshairs[1].position = midPoint - Vector3.up * distance;
        crosshairs[2].position = midPoint + Vector3.right * distance;
        crosshairs[3].position = midPoint - Vector3.right * distance;
    }

    // ���� ź���� ������ ������Ʈ�մϴ�.
    public void UpdateAmmoCount(int ammoLeft, int magazineLeft)
    {
        // �ؽ�Ʈ�� �����մϴ�.
        ammoCountText.text = $"{ammoLeft} / {magazineLeft}";
    }

    // ü�� �ؽ�Ʈ�� ������Ʈ �մϴ�.
    public void UpdateHealth(float health)
    {
        // ü�� �ؽ�Ʈ�� �ؽ�Ʈ�� ���� ���ϴ�.
        healthText.text = $"HP {health}";
    }

    // ���� ȿ�� �̹����� �����մϴ�.
    public void UpdateHurtImage()
    {
        // HurtImageRoutine�� �����մϴ�.
        StopCoroutine(nameof(HurtImageRoutine));
        // HurtImageRoutine�� ���� �����մϴ�.
        StartCoroutine(nameof(HurtImageRoutine));
    }

    // HurtImageRoutine �ڷ�ƾ�� �����մϴ�.
    private IEnumerator HurtImageRoutine()
    {
        // ���� ȿ�� �̹����� Color�� �����ɴϴ�.
        Color color = hurtImage.color;
        // �ش� Color�� ���İ��� 0.5�� �����ϰ�,
        color.a = 0.5f;
        // �ٽ� ���� ȿ�� �̹����� Color�� ��� ���� color�� �ٲ��ݴϴ�.
        hurtImage.color = color;

        // ���� ���İ��� 0 �ʰ���� ������ ���ϴ�.
        while (color.a > 0f)
        {
            // ���İ��� ���� ���� ����ŭ ����,
            color.a -= Time.deltaTime;
            // ���� ȿ�� �̹����� Color�� �ٽ� �������ݴϴ�.
            hurtImage.color = color;

            // ���� �����ӱ��� ����մϴ�.
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

