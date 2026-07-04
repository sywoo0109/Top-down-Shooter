using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControllerRF : MonoBehaviour
{
    // 이동 속도
    public float speed = 8;

    // Bullet 프리팹
    public GameObject bulletPrefab;

    // 사운드 (Player만 있는)
    public AudioClip shotsound; 
    public AudioClip dodgesound;

    // 이동 처리용
    PlayerControls controls; // .inputactions 에셋으로 생성한 이동 처리를 위한 클래스
    Vector3 move;

    // 사격 처리용
    bool isShooting;

    // 캐싱
    SpriteRenderer spriteRenderer; // 불러와야할 컴포넌트를 미리 필드로 선언
    Animator animator;
    AudioSource audioSource;
    CharacterRF character;

    /*
     * Awake 메소드: 오브젝트가 생성되거나 씬이 로드될 떄 딱 한 번 호출
     * 모든 생명주기 함수보다 가장 먼저 실행
     * 스크립트의 초기 설정(참조 캐싱, 객체 생성 등)중에서 다른 스크립트가 해당 스크립트를 참조하기 전에 미리 준비해야하는 것들을 처리
     */
    // 이동 처리용 스크립트 연결 및 캐싱 설정
    private void Awake()
    {
        controls = new PlayerControls();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Awake 메소드에서 미리 객체 생성하며 여기에서 GetComponent를 1번만 사용하도록 = 캐싱
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<CharacterRF>();
    }

    // OnEnable 메소드: 컴포넌트 활성화될 때마다 호출
    // Player Action Map 활성화 및 Fire.performed 이벤트에 실행시킬 메소드 구독
    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Fire.performed += OnFirePerformed;
        /*
         * 각 액션에는 몇 개의 이벤트가 자동으로 생성됨
         * 그 중 performed 이벤트는 지정한 키가 눌렸다고 인식될 때 실행되서 메소드를 구독시킴
         */
    }

    // OnDisable 메소드: 컴포넌트 비활성화될 때마다 호출
    // Player Action Map 비활성화 및 Fire.performed 이벤트에 구독되있는 메소드 취소
    private void OnDisable()
    {
        controls.Player.Fire.performed -= OnFirePerformed; // 이벤트에 구독된 메소드도 취소해야지 GC가 메모리를 정리할 수 있음
        controls.Player.Disable(); // 오브젝트가 꺼지면 입력을 받지 않아야 함
    }

    // Fire.performed 이벤트에 등록할 메소드
    void OnFirePerformed(InputAction.CallbackContext context)
    // UnityEngine.InputSystem.InputAction.CallbackContext : performed 이벤트가 넘겨주는 관련 정보를 담은 값. 이벤트를 발생시킨 입력 및 그 값 등의 정보를 담고 있음
    {
        if (isShooting) return; // 이미 사격 중이면 새 입력 무시
        Shoot();
    }

    void Update()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        /*.
         * inputactions 에셋으로 생성한 이동 처리를 위한 클래스를 이용하는 방식
         * 코드 상에서 키를 하드코딩할 필요 없이 Action Map에 등록한 키가 입력되면 자동으로 처리
         */
        Vector3 inputMove = new Vector3(input.x, input.y, 0).normalized;

        move = isShooting ? Vector3.zero : inputMove; // 사격 중이면 이동 입력을 무시

       /* 
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) 
        {
            move += new Vector3(-1, 0, 0); 
        }
        ...

        구버전 Input Manager를 사용하는 방식
        매 프레임 하드코딩된 키가 눌려 있는지를 폴링하는 방식
        키 재설정 기능 구현 및 여러 입력 디바이스 대응 어려움
       */

        move = move.normalized; // 방향 벡터 정규화

        if (!isShooting) // 반전도 사격 중엔 건너뜀
        {
            if (move.x < 0) spriteRenderer.flipX = true;
            if (move.x > 0) spriteRenderer.flipX = false;
        }


        // 현재 움직임 여부 파악해서 애니메이션 설정
        animator.SetBool("IsMoving", move.magnitude > 0);
        /*
        if (move.magnitude > 0) animator.SetTrigger("Move");
        else animator.SetTrigger("Stop");
        
        자주 반복되는 지속적인 상태와 관련된 애니메이션은 Trigger보다는 Bool로 관리하는 것이 좋음
        */

        /*
        if (move.x < 0) 
        {
            GetComponent<SpriteRenderer>().flipX = true; 
        }
        if (move.x > 0) 
        {
            GetComponent<SpriteRenderer>().flipX = false;
        } 

        if (move.magnitude > 0) 
        {
            GetComponent<Animator>().SetTrigger("Move"); 
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Stop");
        }
        Update 메소드 안에서 GetComponent를 사용하는 방식
        타입이 일치하는 컴포넌트를 찾을 때까지 하나씩 비교하기 때문에 비용이 크기 때문에 Update/FixedUpdate류의 메소드에서는 사용을 지양
        자주 발생하지 않는 이벤트성 호출, 런타임에 컴포넌트 구성이 바뀜, 초기화 이럴 때만 사용
        */
    }

    // 이동 처리
    private void FixedUpdate() 
    {
        transform.Translate(move * speed * Time.fixedDeltaTime);
    }

    // 사격 처리
    void Shoot()
    {
        isShooting = true;

        audioSource.PlayOneShot(shotsound);
        animator.SetTrigger("Shoot");

        // 커서 현재 위치랑 메인 카메라 위치 비교해서 방향 벡터 설정 후 위치 보정
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPosiotion = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        worldPosiotion.z = 0; 
        worldPosiotion -= transform.position + new Vector3(0, -0.5f, 0);

        // 조준 방향에 따라 캐릭터 반전
        if (worldPosiotion.x < 0) spriteRenderer.flipX = true;
        if (worldPosiotion.x > 0) spriteRenderer.flipX = false;

        // Bullet 오브젝트는 오브젝트 풀에서 매번 꺼내오는 다른 인스턴스라 필드로 고정해두는 방식이 유효하지 않음
        GameObject newBullet = GetComponent<ObjectPool>().Get();
        if (newBullet != null) 
        {
            newBullet.transform.position = transform.position + new Vector3(0, -0.5f); // 시작점은 Player 위치에서 약간 보정한 위치
            newBullet.GetComponent<Bullet>().Direction = worldPosiotion; // 방향 벡터만 전달하고 위치 이동은 Bullet 스크립트에서 
        }
    }

    // 충돌 처리
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        // Enemy 객체와 충돌
        if (collision.gameObject.CompareTag("Enemy"))
        {
            character.Hit(1); // 생존 및 사망 분기와 그에 따른 처리는 공통된 스크립트로 이동
        }
    }

    // 사격 모션 끝
    public void OnShootAnimationEnd()
    {
        isShooting = false;
    }

    /*
    // 사망 종료될 경우 수행
    void HandleDeathComplete()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    Player 오브젝트의 행동 이외의 게임의 흐름까지 해당 스크립트에 넣는 것은 책임 범위를 넘어감
    게임 흐름과 관련된 다른 로직도 이 스크립트에 쌓일 우려가 있고 다른 씬에 사용할 수 없어 재사용성도 떨어짐
    */
}
