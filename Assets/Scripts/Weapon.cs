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

    // 1초에 발사하는 총알의 수
    public int rpm;
    // 발사 간격
    private float fireInterval;
    // 발사 간격 측정을 위한 변수
    private float fireTimer;


    // Raycast
    //**************************************************

    // 기본 명중률
    public float defaultAccuracy;
    // 반동
    public float recoil;
    // 현재 명중률
    private float currentAccuracy;

    // 레이어 마스크, 어떤 레이어를 검출할 것인가?
    public LayerMask layerMask;
    // 총알 자국 프리펩
    public GameObject bulletHolePrefab;


    // Ammo
    //**************************************************
    
    // 현재 탄창에 남아 있는 총알의 수
    public int ammoLeft;
    // 탄창에 들어가는 최대 총알의 수
    public int maxAmmo;
    // 현재 주머니에 남아 있는 총알의 수
    public int magazineLeft;
    // 주머니에 들어가는 최대 총알의 수
    public int maxMagazine;


    // Reload
    //**************************************************

    // 현재 재장전을 하고 있는가?
    public bool isReloaing = false;
    // 애니메이션을 담당하는 컴포넌트
    private Animator animator;


    // FX
    //**************************************************
   
    // 이펙트를 담당하는 컴포넌트
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlashTPS;

    // 발사 오디오 클립
    public AudioClip fireSound;

    // 오디오 재생을 담당하는 컴포넌트
    private AudioSource audioSource;

    // UI
    private UIControl UIControl;

    // 게임 오브젝트가 처음 실행될 때 한번 실행
    private void Awake()
    {
        // 발사 간격 계산
        fireInterval = 60f / rpm;
        // 현재 명중률을 기본 명중률로 초기화합니다.
        currentAccuracy = defaultAccuracy;

        // 자식 오브젝트에 있는 Animator 컴포넌트를 가져옵니다.
        animator = GetComponentInChildren<Animator>();
        // 이 스크립트가 붙어 있는 게임오브젝트에서 AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();
        // 씬에 있는 UIControl 컴포넌트를 가져옵니다.
        UIControl = FindObjectOfType<UIControl>();
    }

    // 매 프레임마다 실행
    private void Update()
    {
        if (photonView.IsMine)
        {
            ReloadControl();

            FireControl();

            // 현재 명중률을, 기본 명중률로 선형보간합니다.
            currentAccuracy = Mathf.Lerp(currentAccuracy,
                defaultAccuracy, Time.deltaTime * 8f);

            // UI의 남은 탄약 텍스트를 업데이트합니다.
            UIControl.UpdateAmmoCount(ammoLeft, magazineLeft);

            // 카메라의 Transform을 캐싱하고,
            Transform camTransform = Camera.main.transform;
            // 총알이 박히는 원의 월드 포지션을 계산한 뒤,
            Vector3 worldPosition = camTransform.position +
                camTransform.forward + camTransform.up * currentAccuracy;
            // UI의 크로스헤어를 업데이트합니다.
            UIControl.UpdateCrosshairPosition(worldPosition);
        }
        else
        {
            
        }
    }

    // 발사를 조작하는 함수
    private void FireControl()
    {
        // fireTimer에 시간을 누적합니다.
        fireTimer += Time.deltaTime;

        // 만약 fireTimer가 발사 간격을 넘어 섰다면?
        if (fireTimer >= fireInterval)
        {
            // 만약 총알이 없으면 리턴
            if (ammoLeft <= 0)
            {
                return;
            }

            // 만약 재장전 중이라면 리턴
            if (isReloaing)
            {
                return;
            }

            // 마우스 왼쪽 클릭을 할 경우
            if (Input.GetKey(KeyCode.Mouse0))
            {
                // fireTimer 초기화
                fireTimer = 0f;

                // 발사 로직 수행
                Fire();
            }
        }
    }

    // 총알 발사 로직
    private void Fire()
    {
        // 탄창에 남아 있는 총알을 1발 제거합니다.
        ammoLeft--;

        // 총 발사 이펙트를 실행합니다.
        muzzleFlash.Play();

        // 총 발사 사운드를 재생합니다.
        audioSource.PlayOneShot(fireSound);

        // 현재 명중률에 반동을 더합니다.
        currentAccuracy += recoil;

        Raycast();
    }

    // 레이캐스트 로직
    private void Raycast()
    {
        // 총알이 박히는 위치를 랜덤하게 원 안에서 구합니다.
        Vector2 recoilCircle = Random.insideUnitCircle * currentAccuracy;
        // 카메라의 Transform을 캐싱합니다.
        Transform camTransform = Camera.main.transform;

        // 카메라의 위치에서 총알이 박히는 곳으로 향하는 벡터를 계산합니다.
        Vector3 dir = camTransform.forward + camTransform.up * recoilCircle.y 
            + camTransform.right * recoilCircle.x;

        // 레이를 만듭니다.
        Ray ray = new Ray(camTransform.position, dir);
        // 레이캐스트 결과를 담는 변수를 생성합니다.
        RaycastHit hit;

        // 레이캐스트를 수행합니다. 성공적으로 검출하면 True가 반환됩니다.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask.value))
        {
            // 검출된 콜라이더의 부모에서 HealthControlBase를 가져옵니다.
            HealthControlBase hc = hit.collider.GetComponentInParent<HealthControlBase>();
            // 만약 HealthControlBase가 있다면?
            if (hc != null)
            {
                // HitInfo 구조체를 초기화합니다.
                HitInfo hitInfo = new HitInfo
                {
                    // 레이가 맞은 위치
                    point = hit.point,
                    // 레이의 방향
                    direction = ray.direction,
                    // 레이가 맞은 부분에 접하는 면에 수직인 벡터(법선)
                    normal = hit.normal,
                    // 레이가 맞은 콜라이더
                    collider = hit.collider
                };

                // HealthControlBase에서 대미지 누적을 실시합니다.
                hc.OnDamage(hitInfo, 20f);

                photonView.RPC(nameof(RpcFire), RpcTarget.Others, false, Vector3.zero, Vector3.zero);

                return;
            }

            // 만약 HealthControlBase가 없다면 월드이므로
            // 총알이 맞은 위치를 구합니다.
            Vector3 position = hit.point;
            // 총알 자국의 회전값을 구합니다.
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // 총알 자국을 생성합니다.
            GameObject bh = Instantiate(bulletHolePrefab, position, rotation);
            // Z-Fighting 문제를 해결하기 위해, 총알 자국을 살짝 위로 올려줍니다.
            bh.transform.position += bh.transform.up * 0.001f;
            // 1초 뒤에 총알 자국을 제거합니다.
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
            // 만약 HealthControlBase가 없다면 월드이므로
            // 총알이 맞은 위치를 구합니다.
            Vector3 position = hitPoint;
            // 총알 자국의 회전값을 구합니다.
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

            // 총알 자국을 생성합니다.
            GameObject bh = Instantiate(bulletHolePrefab, position, rotation);
            // Z-Fighting 문제를 해결하기 위해, 총알 자국을 살짝 위로 올려줍니다.
            bh.transform.position += bh.transform.up * 0.001f;
            // 1초 뒤에 총알 자국을 제거합니다.
            Destroy(bh, 1f);
        }
    }

    // 재장전을 조작하는 함수
    private void ReloadControl()
    {
        // 이미 재장전 중이라면 리턴
        if (isReloaing)
        {
            return;
        }

        // 만약 탄창에 총알이 없고, 주머니에 총알이 남아 있다면?
        if (ammoLeft <= 0 && magazineLeft > 0)
        {
            // 재장전 중
            isReloaing = true;

            // 재장전 애니메이션 실행
            animator.SetBool("IsReloading", true);
        }
    }

    // 재장전이 완료되었을 경우
    private void Reload()
    {
        // 탄창에 들어가야하는 총알의 수를 구합니다.
        int ammoNeeded = maxAmmo - ammoLeft;

        // 만약 탄창에 들어가야하는 총알의 수보다 주머니에 남아 있는
        // 총알의 수가 더 많다면?
        if (ammoNeeded <= magazineLeft)
        {
            // 탄창에 남아 있는 총알 수 = 탄창에 들어가는 총알의 수
            ammoLeft = maxAmmo;
            // 주머니에 남아 있는 총알의 수 -= 탄창에 들어가는 총알 의 수
            magazineLeft -= ammoNeeded;
        }
        // 그렇지 않다면
        else
        {
            // 탄창에 남아 있는 총알에 주머니에 남아 있는 총알의 수를 모두 더합니다.
            ammoLeft += magazineLeft;
            // 주머니에 남아 있는 총알의 수 = 0
            magazineLeft = 0;
        }

        // 재장전이 완료 되었으므로
        isReloaing = false;
        animator.SetBool("IsReloading", false);
    }

    // Animation에서 이벤트가 발생하였을 때
    public void OnAnimationEvent(string eventName)
    {
        if (eventName == "OnAnimation_Weapon_Reload_Complete")
        {
            Reload();
        }
    }
}
