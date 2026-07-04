using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Player 관련
    public GameObject player;
    CharacterRF playerCharacter;

    // Enemy 관련
    public float spawnTerm = 5;
    public float fasterEverySpawn = 0.05f;
    public float minSpawnTerm = 1;
    float timeAfterLastSpawn; // 마지막 적 스폰 이후로 지난 시간을 저장

    // UI 관련
    public TextMeshProUGUI scoreText;
    float score;

    // 캐싱 데이터 설정
    void Awake()
    {
        playerCharacter = player.GetComponent<CharacterRF>();
    }

    // 적 소환 관련 정보 및 점수 초기화
    void Start()
    {
        timeAfterLastSpawn = 0;
        score = 0;
    }

    // 적 소환 및 점수 처리
    void Update()
    {
        timeAfterLastSpawn += Time.deltaTime;
        score += Time.deltaTime;

        if (timeAfterLastSpawn > spawnTerm) // 지정한 스폰 시간이 되면
        {
            timeAfterLastSpawn -= spawnTerm; // 마지막 스폰 시간을 초기화하고

            SpawnEnemy(); // 적을 스폰

            spawnTerm -= fasterEverySpawn;
            if(spawnTerm < minSpawnTerm)
            {
                spawnTerm = minSpawnTerm;
            }
        }

        scoreText.text = ((int)score).ToString();
    }

    // 게임 오버 처리 메소드 구독
    void OnEnable()
    {
        playerCharacter.OnDeathComplete += HandleGameOver;
    }

    // 게임 오버 처리 메소드 구독 취소
    void OnDisable()
    {
        playerCharacter.OnDeathComplete -= HandleGameOver;
    }

    // 소환 메소드(위치 설정 및 Player 정보 제공)
    void SpawnEnemy ()
    {
        float x = Random.Range(-9f, 9f); // Random.Range(수,수) : 앞의 수와 뒤의 수 사이에서 랜덤한 숫자 출력. 정수를 넣으면 결과도 정수, 실수를 넣으면 결과도 실수
        float y = Random.Range(-4.5f, 4.5f);

        GameObject obj = GetComponent<ObjectPool>().Get();
        obj.transform.position = new Vector3(x, y, 0);
        obj.GetComponent<EnemyControllerRF>().Spawn(player);
    }

    // 게임 오버(Player 사망) 처리
    void HandleGameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}
