using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 8; // 멤버 변수의 경우 유니티의 인스펙터에서도 확인 및 수정이 가능한데 그 쪽이 우선 적용됨
    Vector3 move;
    public GameObject bulletPrefab;

    public Material flashMaterial;
    public Material defaultMaterial;

    public AudioClip shotsound;
    public AudioClip hitsound;
    public AudioClip deadsound;

    void Update() // Update: 매 '프레임'마다 실행될 함수(매 초 아님 주의)
    {
        move = Vector3.zero;
        // 아래의 방식은 현업과 차이가 있음. 키 세팅을 변경하는 기능을 구현하기 힘들기 때문
        // 현재 아래의 방식 자체가 구형임. 현재는 Keyboard.current.aKey.isPressed 와 같은 방식으로 사용하며 아래의 방식을 쓰기 위해선 기존 입력 시스템을 사용해야함.
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) // Input.GetKey: 특정 키가 눌리면 true를 반환하는 메소드, Keycode: 누를 수 있는 키 값들을 가진 enum
        {
            move += new Vector3(-1, 0, 0); // transform: 게임 오브젝트의 transform 컴포넌트에 접근, Translate: transform 컴포넌트를 변경하는 메소드
            // transform 컴포넌트: Position, Rotation, Scale 3가지 값을 통해 위치, 방향, 크기를 표현
            // Vector: 좌표 데이터를 저장하고 있는 구조체. Vector2의 경우는 x값과 y값만을 통한 2차원 좌표, Vector3의 경우는 x값, y값, z값을 통해 3차원 좌표를 저장하고 있음.
            // 게임 오브젝트의 transform 컴포넌트의 속성들을 보면 x, y, z 값을 가지고 있으므로 Vector3 구조체를 통해 다룰 수 있음
            // deltaTime: Update 함수의 호출 간격 (예: 60프레임 환경이라면 0.16666...초)
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            move += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            move += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            move += new Vector3(0, -1, 0);
        }

        move = move.normalized; // normalize: 대각선 이동이 더 빨라지는 것을 보간하기 위해 벡터의 방향은 유지하고 길이만 1로 만드는 것
        
        if (move.x < 0) // x 값이 음수일 때 = 왼쪽으로 움직이고 있음
        {
            GetComponent<SpriteRenderer>().flipX = true; // GetComponent<컴포넌트명> 같은 오브젝트에 붙어있는 컴포넌트를 다른 컴포넌트에서 호출하는 방법
        }
        if (move.x > 0) // x 값이 양수일 때 = 오른쪽으로 움직이고 있음.
        {
            GetComponent<SpriteRenderer>().flipX = false;
        } // x 값이 0이면 좌우 움직임은 없으므로 상태 유지를 위해 따로 코드 작성 X

        if (move.magnitude > 0) // magnitude: 백터의 길이를 반환, 여기에서는 움직임의 유무를 판별하기 위해서 사용
        {
            GetComponent<Animator>().SetTrigger("Move"); // SetTrigger: Animation끼리의 관계를 설정하면서 지정했던 Conditions를 사용
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Stop");
        }

        if(Input.GetMouseButtonDown(0)) // GetMouseButtonDown(): 마우스 버튼이 클릭될때 true 반환, 0이면 좌클릭
        {
            Shoot();
        }
    }

    private void FixedUpdate() // 이동과 같은 물리적 작용과 관련된 내용은 Update 함수가 아닌 FixedUpdate 함수에서 처리해야 함
    {
        transform.Translate(move * speed * Time.fixedDeltaTime); // FixedUpdate에서는 fixedDeltaTime을 사용해야 함
    }

    void Shoot()
    {
        GetComponent<AudioSource>().PlayOneShot(shotsound);

        Vector3 worldPosiotion = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 카메라를 기준으로 마우스가 클릭한 지점이 어딘지 알 수 있음
        worldPosiotion.z = 0; // 카메라가 캐릭터보다 약간 뒤에 있기 때문에 보간을 위해 0으로
        worldPosiotion -= (transform.position + new Vector3(0, -0.5f, 0));

        /// GameObject newBullet = Instantiate<GameObject>(bulletPrefab); // Instantiate<타입>(원본): 실행 중 원본 오브젝트의 복사본을 생성
        // Instantiate는 많이 실행되면 성능에 영향을 미칠 수 있음. 그래서 오브젝트를 미리 생성해서 필요한 경우 사용할 수 있음 = 오브젝트 풀링
        GameObject newBullet = GetComponent<ObjectPool>().Get();
        if (newBullet != null) // 최대 개수 초과인지 확인
        {
            newBullet.transform.position = transform.position + new Vector3(0, -0.5f); // 생성될 bullet 오브젝트 복사본의 위치를 Player 위치에서 살짝 낮춘 위치로 조정
            newBullet.GetComponent<Bullet>().Direction = worldPosiotion; // bullet 오브젝트가 날아갈 위치를 조정   
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) // isTrigger 옵션이 설정되어 있지 않은 두 오브젝트가 충돌할 때
    // 매개변수로는 충돌한 상대방의 collider를 가지고 옴
    {
        if(collision.gameObject.tag=="Enemy")
        {
            if(GetComponent<Character>().Hit(1))
            {
                // 살아있을 때
                GetComponent<AudioSource>().PlayOneShot(hitsound);
                Flash();
            }
            else
            {
                // 죽었을 때
                GetComponent<AudioSource>().PlayOneShot(deadsound);
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
        GetComponent<Animator>().SetTrigger("Die");
        Invoke("AfterDying", 1.8f); 
    }

    void AfterDying()
    {
        // gameObject.SetActive(false);
        SceneManager.LoadScene("GameOVerScene");
    }
}
