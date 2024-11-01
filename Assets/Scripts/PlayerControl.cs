using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerControl : MonoBehaviourPun, IPunObservable
{
    // Move
    //**************************************************

    // �̵� �ӵ�
    public float moveSpeed = 10f;
    // �̵� ����
    private Vector3 moveDirection;

    // ĳ���� ��Ʈ�ѷ�(ĳ������ �������� ����ϴ� ������Ʈ)
    private CharacterController characterController;


    // Look
    //**************************************************

    // ���콺 �ΰ���
    public float mouseSensitivity = 10f;
    // �Ӹ� Transform
    public Transform headTransform;

    // ���콺 X(����) ��
    private float mouseX = 0f;
    // ���콺 Y(����) ��
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

    // ���� ������Ʈ�� ó�� ���۵� �� �ѹ� ����
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // �� ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ���� CharacterController ������Ʈ�� �����ɴϴ�.
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

    // �� �����Ӹ��� ����
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

    // ĳ������ �̵� ����
    private void MoveControl()
    {
        // ���� ĳ���Ͱ� ���� �پ� �ִٸ�
        if (characterController.isGrounded)
        {
            // ���� �� �Է�(A, D)
            float h = Input.GetAxisRaw("Horizontal");
            // ���� �� �Է�(W, S)
            float v = Input.GetAxisRaw("Vertical");

            // ���� ��ǥ�� �������� moveDirection�� ����
            moveDirection = new Vector3(h, moveDirection.y, v);
            // ���� ��ǥ�踦 ���� ��ǥ��� ����
            moveDirection = this.transform.TransformDirection(moveDirection);
            // moveDirection�� normalize�� ��, �̵� �ӵ��� Time.deltaTime�� �����ݴϴ�.
            moveDirection = moveDirection.normalized * moveSpeed;
            // moveDirection�� y���� ������ ���� ������, characterController�� 
            // isGrounded�� ����� üũ���� ������ y���� -1�� ����
            moveDirection.y = -1f;

            // Space�ٸ� ������ ��
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // moveDirection�� y���� 2�� ����
                moveDirection.y = 2f;
            }

            isMoving = h != 0 || v != 0;

            // CharacterController ������Ʈ�� ���� ������ �����Դϴ�.
            characterController.Move(moveDirection * Time.deltaTime);
        }
        // ĳ���Ͱ� ���� �پ� ���� �ʴٸ�(���߿� ��������)
        else
        {
            // moveDirection�� y���� �߷°��ӵ� ��ŭ ���ݴϴ�.
            moveDirection.y -= 10f * Time.deltaTime;
            // moveDirection��ŭ �����Դϴ�.
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    // ĳ������ �ü� ����
    private void LookControl()
    {
        // ���콺 ���� �� �Է�
        float h = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // ���콺 ���� �� �Է�
        float v = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ���� �¿�� �����̹Ƿ� +=
        mouseX += h;
        // �Ӹ��� ���� �ö󰡸� ���� ���� �ǹǷ�, -=
        mouseY -= v;
        // mouseY�� ���� -80 ~ 80���� �����մϴ�.
        mouseY = Mathf.Clamp(mouseY, -80f, 80f);

        // ���� Transform�� ȸ���� ����
        this.transform.localEulerAngles = new Vector3(0f, mouseX, 0f);
        // �Ӹ��� Transform�� ȸ���� ����
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
