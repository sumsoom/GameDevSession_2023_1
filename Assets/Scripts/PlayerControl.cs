using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerControl : MonoBehaviourPun, IPunObservable
{
    // Move
    //**************************************************

    // 이동 속도
    public float moveSpeed = 10f;
    // 이동 방향
    private Vector3 moveDirection;

    // 캐릭터 컨트롤러(캐릭터의 움직임을 담당하는 컴포넌트)
    private CharacterController characterController;


    // Look
    //**************************************************

    // 마우스 민감도
    public float mouseSensitivity = 10f;
    // 머리 Transform
    public Transform headTransform;

    // 마우스 X(수평) 값
    private float mouseX = 0f;
    // 마우스 Y(수직) 값
    private float mouseY = 0f;
    private float mouseYSync = 0f;

    //**************************************************

    public Camera playerCamera;
    public GameObject tps;
    public GameObject fps;
    public Transform chestTransform;
    private Weapon weapon;

    private Animator animator;
    private bool isMoving = false;
    private bool isReloading = false;

    public GameObject nickname;
    private UIControl UIControl;

    // 게임 오브젝트가 처음 시작될 때 한번 실행
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 이 스크립트가 붙어있는 게임 오브젝트에서 CharacterController 컴포넌트를 가져옵니다.
        characterController = GetComponent<CharacterController>();
        weapon = GetComponentInChildren<Weapon>();
        animator = tps.GetComponentInChildren<Animator>();

        UIControl = FindObjectOfType<UIControl>();
    }

    private void OnDestroy()
    {
        if (nickname != null)
        {
            Destroy(nickname.gameObject);
        }    
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerCamera.gameObject.SetActive(true);
            fps.SetActive(true);
            tps.SetActive(false);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
            fps.SetActive(false);
            tps.SetActive(true);

            nickname = UIControl.CreateNickname(photonView.Owner.NickName);
        }
      
    }

    // 매 프레임마다 실행
    private void Update()
    {
        if (photonView.IsMine)
        {
            MoveControl();
            LookControl();
        }
        else
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsReloading", isReloading);

            mouseYSync = Mathf.Lerp(mouseYSync, mouseY, Time.deltaTime * 5f);

            UIControl.UpdateNicknameTransform(nickname, this.transform.position + Vector3.up * 4.5f);
        }
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            chestTransform.localEulerAngles = new Vector3(0f, mouseYSync, 0f);
        }
    }

    // 캐릭터의 이동 조작
    private void MoveControl()
    {
        // 만약 캐릭터가 땅에 붙어 있다면
        if (characterController.isGrounded)
        {
            // 수평 값 입력(A, D)
            float h = Input.GetAxisRaw("Horizontal");
            // 수직 값 입력(W, S)
            float v = Input.GetAxisRaw("Vertical");

            // 로컬 좌표계 기준으로 moveDirection을 설정
            moveDirection = new Vector3(h, moveDirection.y, v);
            // 로컬 좌표계를 월드 좌표계로 변경
            moveDirection = this.transform.TransformDirection(moveDirection);
            // moveDirection을 normalize한 후, 이동 속도와 Time.deltaTime을 곱해줍니다.
            moveDirection = moveDirection.normalized * moveSpeed;
            // moveDirection의 y값이 음수가 되지 않으면, characterController의 
            // isGrounded가 제대로 체크되지 않으니 y값을 -1로 설정
            moveDirection.y = -1f;

            // Space바를 눌렀을 때
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // moveDirection의 y값을 2로 설정
                moveDirection.y = 2f;
            }

            isMoving = h != 0 || v != 0;

            // CharacterController 컴포넌트롤 통해 실제로 움직입니다.
            characterController.Move(moveDirection * Time.deltaTime);
        }
        // 캐릭터가 땅에 붙어 있지 않다면(공중에 떠있으면)
        else
        {
            // moveDirection의 y값을 중력가속도 만큼 빼줍니다.
            moveDirection.y -= 10f * Time.deltaTime;
            // moveDirection만큼 움직입니다.
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    // 캐릭터의 시선 조작
    private void LookControl()
    {
        // 마우스 수평 값 입력
        float h = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // 마우스 수직 값 입력
        float v = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 몸은 좌우로 움직이므로 +=
        mouseX += h;
        // 머리가 위로 올라가면 음수 값이 되므로, -=
        mouseY -= v;
        // mouseY의 값을 -80 ~ 80으로 제한합니다.
        mouseY = Mathf.Clamp(mouseY, -80f, 80f);

        // 몸의 Transform의 회전값 조정
        this.transform.localEulerAngles = new Vector3(0f, mouseX, 0f);
        // 머리의 Transform의 회전값 조정
        headTransform.localEulerAngles = new Vector3(mouseY, 0f, 0f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isMoving);
            stream.SendNext(mouseY);
            stream.SendNext(weapon.isReloaing);
        }
        else
        {
            isMoving = (bool)stream.ReceiveNext();
            mouseY = (float)stream.ReceiveNext();
            isReloading = (bool)stream.ReceiveNext();
        }
    }
}
