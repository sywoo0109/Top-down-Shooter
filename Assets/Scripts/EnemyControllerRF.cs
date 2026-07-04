using UnityEngine;

public class EnemyControllerRF : MonoBehaviour
{
    // 이동 속도
    public float speed = 2;

    // Player 오브젝트 위치 저장
    GameObject target;
    
    // Enemy 오브젝트의 State 분기
    enum State { Spawning, Moving, Dying }
    State state;

    // 캐싱용 필드
    SpriteRenderer spriteRenderer;
    CharacterRF character;
    Animator animator;
    CircleCollider2D circleCollider;

    // 캐싱용 초기 설정 
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        character = GetComponent<CharacterRF>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // 이벤트 구독
    void OnEnable()
    {
        character.OnDeathStart += HandleDeathStart;
        character.OnDeathComplete += HandleDeathComplete;
    }

    // 이벤트 구독 취소
    void OnDisable()
    {
        character.OnDeathStart -= HandleDeathStart;
        character.OnDeathComplete -= HandleDeathComplete;
    }

    // Enemy 오브젝트 소환 및 초기 설정
    public void Spawn(GameObject target)
    {
        // 이벤트성 호출에서는 성능상의 이점은 크게 없지만 필드를 어차피 선언했으니 재사용성을 높이기 위한 목적으로도 사용 가능
        this.target = target;
        state = State.Spawning;
        character.Initialize();
        animator.SetTrigger("Spawn");
        Invoke("StartMoving", 1);
        circleCollider.enabled = false; 
    }

    // 소환 1초 후부터 동작 시작
    void StartMoving()
    {
        circleCollider.enabled = true; 
        state = State.Moving;
    }

    private void FixedUpdate()
    {
        // 동작 중일때 Player 위치와 비교해서 벡터 값을 따서 오브젝트를 계속 이동시킴
        // 벡터 값을 보고 음수면 왼쪽을 보게 좌우 반전
        if (state == State.Moving)
        {
            Vector2 direction = target.transform.position - transform.position; 
            transform.Translate(direction.normalized * speed * Time.fixedDeltaTime);

            if (direction.x < 0) spriteRenderer.flipX = true;
            if (direction.x > 0) spriteRenderer.flipX = false;
        }
    }

    // Bullet 오브젝트와 충돌 처리
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Bullet"))
        {
            // 대미지 계산 및 충돌 계산
            float d = collision.gameObject.GetComponent<Bullet>().damage;
            character.Hit(d);
        }
    }

    // 사망 즉시 처리 필요한 부분
    void HandleDeathStart()
    {
        state = State.Dying;
    }

    // 사망 이후 처리할 부분
    void HandleDeathComplete()
    {
        gameObject.SetActive(false);
    }
}
