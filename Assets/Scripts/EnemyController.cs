using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2;
    GameObject target;
    enum State
    {
        Spawing,
        Moving,
        Dying
    }
    State state;
    public Material flashMaterial;
    public Material defaultMaterial;

    public AudioClip hitSound;
    public AudioClip deadSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // target = GameObject.Find("Player"); // Find(태그): 해당 태그를 가진 오브젝트를 찾아서 반환, 실제 게임에서 성능 저하의 요인이라서 별로 사용하지는 않음
        // state = State.Moving;                           
    }

    public void Spawn(GameObject target)
    {
        this.target = target;
        state = State.Spawing;
        GetComponent<Character>().Initialize();
        GetComponent<Animator>().SetTrigger("Spawn");
        Invoke("StartMoving", 1); // 스폰 1초 후부터 활동 시작
        GetComponent<Collider2D>().enabled = false; // 스폰되는 동안은 충돌 판정을 끄기 위해
        // 실제 게임 오브젝트에 포함된건 CircleCollider2D이지만 해당 클래스가 Collider@D 클래스를 상속받았기 때문에 위의 코드로도 호출이 가능
    }

    void StartMoving()
    {
        GetComponent<Collider2D>().enabled = true; // 비활성화 시켜 둔 collider를 다시 활성화
        state = State.Moving;
    }

    private void FixedUpdate()
    {
        if (state == State.Moving)
        {
            Vector2 direction = target.transform.position - transform.position; // 타겟(플레이어)의 위치 좌표 - Enemy 객체의 위치 좌표 = 움직여야하는 벡터
            transform.Translate(direction.normalized * speed * Time.fixedDeltaTime);

            if (direction.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            if (direction.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // collision 매개 변수에는 충돌을 발생시킨 상대 오브젝트가 담김
    {
        if(collision.tag == "Bullet")
        {
            float d = collision.gameObject.GetComponent<Bullet>().damage;

            if(GetComponent<Character>().Hit(d))
            {
                // 살아있는 경우
                GetComponent<AudioSource>().PlayOneShot(hitSound);
                Flash();
            }
            else
            {
                // 죽은 경우
                GetComponent<AudioSource>().PlayOneShot(deadSound);
                Die();
            }
        }
    }

    void Flash()
    {
        GetComponent<SpriteRenderer>().material = flashMaterial;
        Invoke("AfterFlash", 0.3f);
    }

    void AfterFlash()
    {
        GetComponent<SpriteRenderer>().material = defaultMaterial;
    }
   
    void Die()
    {
        state = State.Dying;
        GetComponent<Animator>().SetTrigger("Die");
        Invoke("AfterDying", 1.8f); // Invoke("함수", 시간): 작성한 시간이 지난 후에 작성한 다른 함수를 실행시킴
        // 여기에서는 적이 죽은 다음을 구현하기 위해 사용했지만 그다지 적절한 사용법은 아님
        // 그보다는 Corountine이라는 함수를 사용하는 편이 좋음
    }

    void AfterDying()
    {
        gameObject.SetActive(false);
    }
}
