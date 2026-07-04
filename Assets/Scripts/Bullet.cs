using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 오브젝트 기본 설정(속도, 대미지 등)
    public float speed = 15;
    public float damage = 1;
    Vector2 direction; // 이동 방향을 프로퍼티로 설정
    public Vector2 Direction
    {
        set
        {
            direction = value.normalized; // 방향 정규화를 통해 어떤 값이 들어와도 벡터 유지
        }
    }

    // Bullet 오브젝트 움직임 처리
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime); // 오브젝트 이동은 update 메소드에서
    }

    // 충돌 처리
    private void OnTriggerEnter2D(Collider2D collision) // onTrigger가 켜진 collider를 가진 오브젝트와 충돌하는 경우
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy") ) // 충돌이 일어난 오브젝트의 tag를 확인
        {
            // Destroy(gameObject); // 오브젝트 제거
            gameObject.SetActive(false); // 오브젝트 풀링을 하는 경우 아예 제거하면 안되고 비활성화만 시킴
        }
            
    }
}
