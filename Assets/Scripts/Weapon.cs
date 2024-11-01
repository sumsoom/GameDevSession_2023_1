using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Unity.Burst.CompilerServices;

public class Weapon : MonoBehaviourPun
{
    // Fire
    //**************************************************

    // 1�ʿ� �߻��ϴ� �Ѿ��� ��
    public int rpm;
    // �߻� ����
    private float fireInterval;
    // �߻� ���� ������ ���� ����
    private float fireTimer;


    // Raycast
    //**************************************************

    // �⺻ ���߷�
    public float defaultAccuracy;
    // �ݵ�
    public float recoil;
    // ���� ���߷�
    private float currentAccuracy;

    // ���̾� ����ũ, � ���̾ ������ ���ΰ�?
    public LayerMask layerMask;
    // �Ѿ� �ڱ� ������
    public GameObject bulletHolePrefab;


    // Ammo
    //**************************************************
    
    // ���� źâ�� ���� �ִ� �Ѿ��� ��
    public int ammoLeft;
    // źâ�� ���� �ִ� �Ѿ��� ��
    public int maxAmmo;
    // ���� �ָӴϿ� ���� �ִ� �Ѿ��� ��
    public int magazineLeft;
    // �ָӴϿ� ���� �ִ� �Ѿ��� ��
    public int maxMagazine;


    // Reload
    //**************************************************

    // ���� �������� �ϰ� �ִ°�?
    public bool isReloaing = false;
    // �ִϸ��̼��� ����ϴ� ������Ʈ
    private Animator animator;


    // FX
    //**************************************************
   
    // ����Ʈ�� ����ϴ� ������Ʈ
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlashTPS;

    // �߻� ����� Ŭ��
    public AudioClip fireSound;

    // ����� ����� ����ϴ� ������Ʈ
    private AudioSource audioSource;

    // UI
    private UIControl UIControl;

    // ���� ������Ʈ�� ó�� ����� �� �ѹ� ����
    private void Awake()
    {
        // �߻� ���� ���
        fireInterval = 60f / rpm;
        // ���� ���߷��� �⺻ ���߷��� �ʱ�ȭ�մϴ�.
        currentAccuracy = defaultAccuracy;

        // �ڽ� ������Ʈ�� �ִ� Animator ������Ʈ�� �����ɴϴ�.
        animator = GetComponentInChildren<Animator>();
        // �� ��ũ��Ʈ�� �پ� �ִ� ���ӿ�����Ʈ���� AudioSource ������Ʈ�� �����ɴϴ�.
        audioSource = GetComponent<AudioSource>();
        // ���� �ִ� UIControl ������Ʈ�� �����ɴϴ�.
        UIControl = FindObjectOfType<UIControl>();
    }

    // �� �����Ӹ��� ����
    private void Update()
    {
        if (photonView.IsMine)
        {
            ReloadControl();

            FireControl();

            // ���� ���߷���, �⺻ ���߷��� ���������մϴ�.
            currentAccuracy = Mathf.Lerp(currentAccuracy,
                defaultAccuracy, Time.deltaTime * 8f);

            // UI�� ���� ź�� �ؽ�Ʈ�� ������Ʈ�մϴ�.
            UIControl.UpdateAmmoCount(ammoLeft, magazineLeft);

            // ī�޶��� Transform�� ĳ���ϰ�,
            Transform camTransform = Camera.main.transform;
            // �Ѿ��� ������ ���� ���� �������� ����� ��,
            Vector3 worldPosition = camTransform.position +
                camTransform.forward + camTransform.up * currentAccuracy;
            // UI�� ũ�ν��� ������Ʈ�մϴ�.
            UIControl.UpdateCrosshairPosition(worldPosition);
        }
        else
        {
            
        }
    }

    // �߻縦 �����ϴ� �Լ�
    private void FireControl()
    {
        // fireTimer�� �ð��� �����մϴ�.
        fireTimer += Time.deltaTime;

        // ���� fireTimer�� �߻� ������ �Ѿ� ���ٸ�?
        if (fireTimer >= fireInterval)
        {
            // ���� �Ѿ��� ������ ����
            if (ammoLeft <= 0)
            {
                return;
            }

            // ���� ������ ���̶�� ����
            if (isReloaing)
            {
                return;
            }

            // ���콺 ���� Ŭ���� �� ���
            if (Input.GetKey(KeyCode.Mouse0))
            {
                // fireTimer �ʱ�ȭ
                fireTimer = 0f;

                // �߻� ���� ����
                Fire();
            }
        }
    }

    // �Ѿ� �߻� ����
    private void Fire()
    {
        // źâ�� ���� �ִ� �Ѿ��� 1�� �����մϴ�.
        ammoLeft--;

        // �� �߻� ����Ʈ�� �����մϴ�.
        muzzleFlash.Play();

        // �� �߻� ���带 ����մϴ�.
        audioSource.PlayOneShot(fireSound);

        // ���� ���߷��� �ݵ��� ���մϴ�.
        currentAccuracy += recoil;

        Raycast();
    }

    // ����ĳ��Ʈ ����
    private void Raycast()
    {
        // �Ѿ��� ������ ��ġ�� �����ϰ� �� �ȿ��� ���մϴ�.
        Vector2 recoilCircle = Random.insideUnitCircle * currentAccuracy;
        // ī�޶��� Transform�� ĳ���մϴ�.
        Transform camTransform = Camera.main.transform;

        // ī�޶��� ��ġ���� �Ѿ��� ������ ������ ���ϴ� ���͸� ����մϴ�.
        Vector3 dir = camTransform.forward + camTransform.up * recoilCircle.y 
            + camTransform.right * recoilCircle.x;

        // ���̸� ����ϴ�.
        Ray ray = new Ray(camTransform.position, dir);
        // ����ĳ��Ʈ ����� ��� ������ �����մϴ�.
        RaycastHit hit;

        // ����ĳ��Ʈ�� �����մϴ�. ���������� �����ϸ� True�� ��ȯ�˴ϴ�.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask.value))
        {
            // ����� �ݶ��̴��� �θ𿡼� HealthControlBase�� �����ɴϴ�.
            HealthControlBase hc = hit.collider.GetComponentInParent<HealthControlBase>();
            // ���� HealthControlBase�� �ִٸ�?
            if (hc != null)
            {
                // HitInfo ����ü�� �ʱ�ȭ�մϴ�.
                HitInfo hitInfo = new HitInfo
                {
                    // ���̰� ���� ��ġ
                    point = hit.point,
                    // ������ ����
                    direction = ray.direction,
                    // ���̰� ���� �κп� ���ϴ� �鿡 ������ ����(����)
                    normal = hit.normal,
                    // ���̰� ���� �ݶ��̴�
                    collider = hit.collider
                };

                // HealthControlBase���� ����� ������ �ǽ��մϴ�.
                hc.OnDamage(hitInfo, 20f);

                photonView.RPC(nameof(RpcFire), RpcTarget.Others, false, Vector3.zero, Vector3.zero);

                return;
            }

            // ���� HealthControlBase�� ���ٸ� �����̹Ƿ�
            // �Ѿ��� ���� ��ġ�� ���մϴ�.
            Vector3 position = hit.point;
            // �Ѿ� �ڱ��� ȸ������ ���մϴ�.
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // �Ѿ� �ڱ��� �����մϴ�.
            GameObject bh = Instantiate(bulletHolePrefab, position, rotation);
            // Z-Fighting ������ �ذ��ϱ� ����, �Ѿ� �ڱ��� ��¦ ���� �÷��ݴϴ�.
            bh.transform.position += bh.transform.up * 0.001f;
            // 1�� �ڿ� �Ѿ� �ڱ��� �����մϴ�.
            Destroy(bh, 1f);

            photonView.RPC(nameof(RpcFire), RpcTarget.Others, true, hit.point, hit.normal);
        }
        else
        {
            photonView.RPC(nameof(RpcFire), RpcTarget.Others, false, Vector3.zero, Vector3.zero);
        }

    }

    [PunRPC]
    private void RpcFire(bool isHitWorld, Vector3 hitPoint, Vector3 hitNormal)
    {
        muzzleFlashTPS.Play();

        audioSource.PlayOneShot(fireSound);

        if (isHitWorld)
        {
            // ���� HealthControlBase�� ���ٸ� �����̹Ƿ�
            // �Ѿ��� ���� ��ġ�� ���մϴ�.
            Vector3 position = hitPoint;
            // �Ѿ� �ڱ��� ȸ������ ���մϴ�.
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

            // �Ѿ� �ڱ��� �����մϴ�.
            GameObject bh = Instantiate(bulletHolePrefab, position, rotation);
            // Z-Fighting ������ �ذ��ϱ� ����, �Ѿ� �ڱ��� ��¦ ���� �÷��ݴϴ�.
            bh.transform.position += bh.transform.up * 0.001f;
            // 1�� �ڿ� �Ѿ� �ڱ��� �����մϴ�.
            Destroy(bh, 1f);
        }
    }

    // �������� �����ϴ� �Լ�
    private void ReloadControl()
    {
        // �̹� ������ ���̶�� ����
        if (isReloaing)
        {
            return;
        }

        // ���� źâ�� �Ѿ��� ����, �ָӴϿ� �Ѿ��� ���� �ִٸ�?
        if (ammoLeft <= 0 && magazineLeft > 0)
        {
            // ������ ��
            isReloaing = true;

            // ������ �ִϸ��̼� ����
            animator.SetBool("IsReloading", true);
        }
    }

    // �������� �Ϸ�Ǿ��� ���
    private void Reload()
    {
        // źâ�� �����ϴ� �Ѿ��� ���� ���մϴ�.
        int ammoNeeded = maxAmmo - ammoLeft;

        // ���� źâ�� �����ϴ� �Ѿ��� ������ �ָӴϿ� ���� �ִ�
        // �Ѿ��� ���� �� ���ٸ�?
        if (ammoNeeded <= magazineLeft)
        {
            // źâ�� ���� �ִ� �Ѿ� �� = źâ�� ���� �Ѿ��� ��
            ammoLeft = maxAmmo;
            // �ָӴϿ� ���� �ִ� �Ѿ��� �� -= źâ�� ���� �Ѿ� �� ��
            magazineLeft -= ammoNeeded;
        }
        // �׷��� �ʴٸ�
        else
        {
            // źâ�� ���� �ִ� �Ѿ˿� �ָӴϿ� ���� �ִ� �Ѿ��� ���� ��� ���մϴ�.
            ammoLeft += magazineLeft;
            // �ָӴϿ� ���� �ִ� �Ѿ��� �� = 0
            magazineLeft = 0;
        }

        // �������� �Ϸ� �Ǿ����Ƿ�
        isReloaing = false;
        animator.SetBool("IsReloading", false);
    }

    // Animation���� �̺�Ʈ�� �߻��Ͽ��� ��
    public void OnAnimationEvent(string eventName)
    {
        if (eventName == "OnAnimation_Weapon_Reload_Complete")
        {
            Reload();
        }
    }
}
