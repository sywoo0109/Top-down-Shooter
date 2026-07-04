using UnityEngine;
using System;
using System.Collections;

public class CharacterRF : MonoBehaviour
{
    // 체력 관련
    public float MaxHP = 3;

    // 체력바 관련
    public GameObject HPGuage;
    float HP;
    float HPMaxWidth;

    // 피격 처리
    public Material flashMaterial;
    public Material defaultMaterial;

    // 사운드
    public AudioClip hitSound;
    public AudioClip deadSound;

    // 사망 애니메이션 설정
    public float deathDelay = 1.8f;

    // 캐싱
    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audioSource;
    Collider2D col;

    // 사망 관련 처리를 위한 이벤트
    public event Action OnDeathStart; // 사망 시작(움직임, 입력 중지 등)
    public event Action OnDeathComplete; // 사망 종료(씬 전환, 오브젝트 풀 반환 등)

    // 캐싱 초기 설정 및 체력 설정
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();

        // Player만 체력 바를 가짐
        if (HPGuage != null)
        {
            // 남은 체력 비율 계산을 위해 체력바 길이 확인
            HPMaxWidth = HPGuage.GetComponent<RectTransform>().sizeDelta.x;
        }

        Initialize();
    }

    /* 
    // 채력 설정
    void Start()
    {
        HP = MaxHP;

        // Player만 체력 바를 가짐
        if (HPGuage != null) 
        {
            // 남은 체력 비율 계산을 위해 체력바 길이 확인
            HPMaxWidth = HPGuage.GetComponent<RectTransform>().sizeDelta.x; 
        }
    }

    오브젝트 풀링 환경에서 Start의 실행 시점
    Start는 다음 Update 전까지, 이번 프레임 내에 언젠가 한번 수행되는 것을 보장하는 것이기 때문에 인스턴스를 만들자마자 비활성화하면 실행이 보류될 수 있음
    그러다가 오브젝트를 사용하기 위해 활성화하는 순간 Start가 실행되기 때문에 초기화 관련 코드가 변수의 값 할당 이후에 발생해서 오히려 덮어씌울 수 있음
    반면 Awake는 인스턴스화 되자마자 즉시 한번 바로 실행되기 때문에 비활성화 전에도 수행이 보장됨
    따라서 초기화 관련 코드는 Awake 함수에서 하는 편이 타이밍 불확실성도 적고 한 함수에 몰아야 초기화 로직을 통일하기도 좋음
    */

    // 체력 설정(Enemy 오브젝트 용)
    public void Initialize() 
    {
        HP = MaxHP;
    }

    // 피격 상황 처리 
    public bool Hit(float damage)
    {
        // 체력 반영 및 정리
        HP -= damage;
        if (HP < 0) { HP = 0; }

        // Player 오브젝트 체력바 처리
        if (HPGuage != null)
        {
            HPGuage.GetComponent<RectTransform>().sizeDelta = new Vector2(HP / MaxHP * HPMaxWidth, HPGuage.GetComponent<RectTransform>().sizeDelta.y);
        }

        if (HP > 0)
        {
            // 생존 분기
            audioSource.PlayOneShot(hitSound);
            Flash();
        }
        else
        {
            // 사망 분기
            col.enabled = false;
            audioSource.PlayOneShot(deadSound);
            Die();
        }

        // 생존인 경우 true, 사망의 경우 false를 반환하기 위해
        return HP > 0;
    }

    /*
     * 코루틴
     * 여러 프레임에 걸쳐 실행되는 함수
     * 직접 참조라 안정성이 보장되며 매개변수도 사용할 수 있음
     * 순차적으로 작성하는 구조라서 읽기 쉬우며 명시적으로 취소하는 기능(StopCoroutine)도 있음
     * 단, 코루틴을 쓰면서 함수 자체를 직접 넘기는게 아니라 문자열도 사용이 가능한데 그러면 Invoke 함수와 똑같은 문제점을 가지기 때문에 지양
     */
    IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.3f); // 여기서 실행을 멈추고, 0.3초 뒤 유니티가 알아서 다시 여기부터 이어서 실행
        spriteRenderer.material = defaultMaterial;
    }

    // 두 개의 클래스에 존재하던 거의 비슷한 코드를 합쳐서 하나의 스크립트에서 처리
    void Flash() // 피격 처리
    {
        /*
        spriteRenderer.material = flashMaterial;
        Invoke(nameof(AfterFlash), 0.3f);

        Invoke 함수의 문제점
        메소드를 이름, 문자열 그 자체로 검색 후 찾아서 실행 => 직접 호출보다 느림
        컴파일러가 오타 등의 오류 잡아낼 수 없음 => 실행한 다음에야 알 수 있음(런타임 에러)
        */
        StartCoroutine(FlashRoutine());
    }

    IEnumerator DieRoutine()
    {
        animator.SetTrigger("Die");
        OnDeathStart?.Invoke(); // event 타입 안의 Invoke 함수는 이벤트를 발생시켜서 구독된 함수들을 전부 실행하는 기능
        yield return new WaitForSeconds(deathDelay);
        OnDeathComplete?.Invoke();
    }

    void Die() // 사망 처리
    {
        StartCoroutine(DieRoutine());
    }
}
