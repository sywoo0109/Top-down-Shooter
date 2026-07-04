using UnityEngine;

public class Character : MonoBehaviour
{
    public float MaxHP = 3;
    public GameObject HPGuage;
    float HP;
    float HPMaxWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HP = MaxHP; // 그냥 HP = 3 처럼 구현하지 않는 이유는 유니티 인스펙터에서도 수정하기 편하기 위함

        if (HPGuage != null) // Character 클래스는 체력은 있는데 체력바는 없는 적을 구현할 때도 사용중이기 때문에 예외 처리
        {
            HPMaxWidth = HPGuage.GetComponent<RectTransform>().sizeDelta.x; // UI의 경우 RectTransform 컴포넌트가 대신 붙어 있음, 반환값은 Vector2 타입이라 width 대신 x를 사용한다는 점
        }
    }

    public void Initialize() // 새로운 적 스폰 시의 체력 설정을 위해 분리
    {
        HP = MaxHP;
    }

    /*
     * 살아있는 경우 true를 리턴한다
     */
    // 사실 원래 주석의 목적은 이런 식으로 주석만 보고도 메소드 등의 역할을 알 수 있게 하기 위함
   public bool Hit(float damage)
    {
        HP -= damage;

        Debug.Log($"[{gameObject.name}] damage={damage}, 남은 HP={HP}"); // 임시 디버깅용

        if (HP < 0 ) { HP = 0; }

        if (HPGuage != null)
        {
           HPGuage.GetComponent<RectTransform>().sizeDelta = new Vector2(HP / MaxHP * HPMaxWidth, // 아예 새로운 Verctor2 객체를 할당해야함
           HPGuage.GetComponent<RectTransform>().sizeDelta.y);
        }

        return HP > 0;
    }
}
