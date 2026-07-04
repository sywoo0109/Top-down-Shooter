using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int maxObject = 30; // 풀링도 최대 개수를 정해둘 필요 있음
    List<GameObject> pool;
    public Transform parent; // 생성된 복제 오브젝트를 한번에 담아둘 폴더를 설정
    // 원래 Transform은 위치, 방향, 크기를 나타내는 용도지만 부모-자식 관계를 설정할 때도 사용할 수 있음

    void Start()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < maxObject; i++) 
        {
            GameObject obj = Instantiate(prefab, parent); // Instatiate(원본, 부모) : 원본의 복제 오브젝트를 부모 디렉토리 안에 생성
            obj.SetActive(false); // active 설정을 false로 하면 화면에 렌더링되지도 않고 Update가 불리지도 않음
            pool.Add(obj);
        }
    }

    public GameObject Get() // 오브젝트를 요청할 경우 반환하는 Getter 메소드
    {
        foreach (GameObject obj in pool)
        {
            if(!obj.activeInHierarchy) // activeInHierarchy : Hierarchy에서 active 상태인 오브젝트면 true를 반환
            {
                obj.SetActive(true);
                return obj;
            }
        }

        return null; // 최대 개수를 넘어갔을 경우 알려주기 위해
    }
}
