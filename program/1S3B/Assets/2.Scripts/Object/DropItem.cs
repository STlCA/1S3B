using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;

    [Header("Drop")]
    public Transform sprite;
    public Transform floor;

    [Header("Looting")]
    public CircleCollider2D circleCollider;

    //Drop
    private int maxBounce = 3;
    private float xForce = 5;
    private float yForce = 7;
    private float gravity = 25;

    private Vector2 direction;
    private int currentBounce = 0;
    private bool isGrounded = true;

    private float maxHeight;
    private float currentHeight;

    //Loot
    private float range = 2;
    private Vector3 startPos;
    private float currentTime;
    private float distanceSpeed = 5f;
    private bool isLooting = false;


    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;

        circleCollider.radius = range;

        currentHeight = Random.Range(yForce - 1, yForce);
        maxHeight = currentHeight;
        Init(new Vector2(Random.Range(-xForce, xForce),0));
    }

    public void RadiusSetting(float val)
    {
        range = val;
        circleCollider.radius = range;
    }

    private void Update()
    {
        if ((isLooting == true && currentBounce <= 2) || (isLooting == false && isGrounded == false))
        {
            currentHeight += -gravity * Time.deltaTime;
            sprite.position += new Vector3(0, currentHeight, 0) * Time.deltaTime;
            transform.position += (Vector3)direction * Time.deltaTime;

            float totalVelocity = Mathf.Abs(currentHeight) + Mathf.Abs(maxHeight);
            float scaleXY = Mathf.Abs(currentHeight) / totalVelocity;
            floor.localScale = Vector2.one * Mathf.Clamp(scaleXY, 0.5f, 1.0f);

            CheckFloorHit();
        }

        if (isLooting == true && currentBounce > 2)
            LootingItem();
    }

    private void Init(Vector2 direction)
    {
        isGrounded = false;
        maxHeight /= 1.5f;
        this.direction = direction;
        currentHeight = maxHeight;
        currentBounce++;

        if (currentBounce > 2)
            startPos = transform.position;
    }

    private void CheckFloorHit()
    {
        if (sprite.position.y < floor.position.y)
        {
            sprite.position = floor.position;
            floor.localScale = Vector3.one;

            if (currentBounce < maxBounce)
                Init(direction / 1.5f);
            else
                isGrounded = true;
        }
    }

    private void LootingItem()
    {
        Vector3 playerPos = player.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);
        currentTime += distanceSpeed * Time.deltaTime;
        this.transform.position = Vector3.Lerp(startPos, playerPos, currentTime);

        if (distance < 0.5f)
        {
            Destroy(gameObject);
            isLooting = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLooting == false && other.CompareTag("Player"))
        {
            isLooting = true;
            currentTime = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (startPos == Vector3.zero && other.CompareTag("Player"))
        {
            isLooting = false;
            currentTime = 0;
        }
    }

}

/*// Unity 엔진의 기본 클래스를 사용하기 위한 네임스페이스
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// DropItem 클래스 정의, MonoBehaviour를 상속받아 Unity의 게임 오브젝트에 붙일 수 있는 컴포넌트가 됩니다.
public class DropItem : MonoBehaviour
{
    // GameManager 타입의 변수 선언, 게임의 전반적인 관리를 담당합니다.
    private GameManager gameManager;
    // Player 타입의 변수 선언, 플레이어 오브젝트를 참조합니다.
    private Player player;
    // Inspector 창에서 설정할 수 있는 드롭 관련 변수들을 그룹핑합니다.
    [Header("Drop")]
    public Transform sprite; // 아이템의 스프라이트(이미지) 위치를 나타내는 변수
    public Transform floor; // 바닥의 위치를 나타내는 변수
    // Inspector 창에서 설정할 수 있는 수집(looting) 관련 변수들을 그룹핑합니다.
    [Header("Looting")]
    public CircleCollider2D circleCollider; // 아이템의 수집 범위를 나타내는 콜라이더
    // 드롭 관련 프라이빗(비공개) 변수 선언
    private int maxBounce = 3; // 최대 튕김 횟수
    private float xForce = 5; // x축으로 가하는 힘의 양
    private float yForce = 10; // y축으로 가하는 힘의 양
    private float gravity = 25; // 중력의 크기
    private Vector2 direction; // 아이템이 움직일 방향
    private int currentBounce = 0; // 현재까지 튕긴 횟수
    private bool isGrounded = true; // 아이템이 바닥에 닿았는지를 나타내는 플래그
    private float maxHeight; // 최대 높이
    private float currentHeight; // 현재 높이
    // 수집 관련 프라이빗 변수 선언
    private float range = 2; // 수집 범위
    private Vector3 startPos; // 수집 시작 위치
    private float currentTime; // 현재 시간(수집하는 데 걸리는 시간을 계산)
    private float distanceSpeed = 5f; // 수집 시 이동 속도
    private bool isLooting = false; // 현재 수집 중인지를 나타내는 플래그
    // Start 메소드는 오브젝트가 활성화될 때 한 번 호출됩니다.
    private void Start()
    {
        // 게임 매니저 인스턴스를 가져와 할당합니다.
        gameManager = GameManager.Instance;
        // 게임 매니저로부터 플레이어 인스턴스를 가져와 할당합니다.
        player = gameManager.Player;
        // 수집 범위 설정
        circleCollider.radius = range;
        // 현재 높이를 yForce 범위 내에서 랜덤하게 설정합니다.
        currentHeight = Random.Range(yForce - 1, yForce);
        // 최대 높이를 현재 높이로 설정합니다.
        maxHeight = currentHeight;
        // 방향을 랜덤하게 설정하며 Init 메소드 호출로 초기화합니다.
        Init(new Vector2(Random.Range(-xForce, xForce), Random.Range(-xForce, xForce)));
    }
    // 수집 범위를 설정하는 메소드
    public void RadiusSetting(float val)
    {
        range = val; // 범위 값을 매개변수로 설정합니다.
        circleCollider.radius = range; // 콜라이더의 반경을 새로운 범위로 설정합니다.
    }
    // 매 프레임마다 호출되는 Update 메소드입니다.
    private void Update()
    {
        // 아이템이 수집 중이고, 최대 튕김 횟수보다 적거나, 떨어지고 있는 상태라면
        if ((isLooting == true && currentBounce<=2)||(isLooting == false&& isGrounded == false))
        {
            // 중력을 적용해 현재 높이를 업데이트합니다.
            currentHeight += -gravity * Time.deltaTime;
            // 스프라이트의 위치를 현재 높이에 따라 업데이트합니다.
            sprite.position += new Vector3(0, currentHeight, 0) * Time.deltaTime;
            // 전체 오브젝트의 위치를 방향에 따라 업데이트합니다.
            transform.position += (Vector3)direction * Time.deltaTime;
            // 총 속도를 계산합니다.
            float totalVelocity = Mathf.Abs(currentHeight) + Mathf.Abs(maxHeight);
            // 스케일 계수를 계산합니다.
            float scaleXY = Mathf.Abs(currentHeight) / totalVelocity;
            // 바닥의 스케일을 업데이트합니다.
            floor.localScale = Vector2.one * Mathf.Clamp(scaleXY, 0.5f, 1.0f);
            // 바닥에 닿았는지 확인합니다.
            CheckFloorHit();
        }
        // 수집 중이고, 최대 튕김 횟수보다 많다면, 수집 아이템 메소드를 호출합니다.
        if (isLooting == true && currentBounce>2  )
            LootingItem();
    }
    // 아이템의 초기 상태를 설정하는 메소드입니다.
    private void Init(Vector2 direction)
    {
        // 바닥에 닿지 않은 상태로 설정합니다.
        isGrounded = false;
        // 최대 높이를 조정합니다.
        maxHeight /= 1.5f;
        // 방향을 설정합니다.
        this.direction = direction;
        // 현재 높이를 최대 높이로 설정합니다.
        currentHeight = maxHeight;
        // 튕김 횟수를 증가시킵니다.
        currentBounce++;
        // 튕김 횟수가 2보다 크면 시작 위치를 현재 위치로 설정합니다.
        if (currentBounce > 2)
            startPos = transform.position;
    }
    // 바닥에 닿았는지 확인하는 메소드입니다.
    private void CheckFloorHit()
    {
        // 스프라이트의 y 위치가 바닥보다 낮다면, 즉 바닥에 닿았다면
        if (sprite.position.y < floor.position.y)
        {
            // 스프라이트의 위치를 바닥의 위치로 설정합니다.
            sprite.position = floor.position;
            // 바닥의 스케일을 원래대로 설정합니다.
            floor.localScale = Vector3.one;
            // 최대 튕김 횟수보다 작다면 다시 튕겨올립니다.
            if (currentBounce < maxBounce)
                Init(direction / 1.5f);
            else
                // 최대 튕김 횟수에 도달했다면 바닥에 닿은 상태로 설정합니다.
                isGrounded = true;
        }
    }
    // 아이템을 수집하는 메소드입니다.
    private void LootingItem()
    {
        // BoxCollider2D 컴포넌트를 비활성화합니다.
        GetComponent<BoxCollider2D>().enabled = false;
        // 플레이어의 위치를 가져옵니다.
        Vector3 playerPos = player.transform.position;
        // 아이템과 플레이어 사이의 거리를 계산합니다.
        float distance = Vector3.Distance(transform.position, playerPos);
        // 시간을 업데이트합니다.
        currentTime += distanceSpeed * Time.deltaTime;
        // 아이템의 위치를 플레이어 쪽으로 이동시킵니다.
        this.transform.position = Vector3.Lerp(startPos, playerPos, currentTime);
        // 아이템이 플레이어와 충분히 가까워졌다면 아이템을 파괴하고 수집 상태를 해제합니다.
        if (distance < 0.5f)
        {
            Destroy(gameObject);
            isLooting = false;
        }
    }
    // 다른 오브젝트가 이 오브젝트의 트리거 콜라이더 내에 들어왔을 때 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 수집 상태가 아니고, 다른 오브젝트가 플레이어라면 수집 상태로 변경하고 시간을 초기화합니다.
        if (isLooting == false && other.CompareTag("Player"))
        {
            isLooting = true;
            currentTime = 0;
        }
    }
    // 다른 오브젝트가 이 오브젝트의 트리거 콜라이더를 떠났을 때 호출됩니다.
    private void OnTriggerExit2D(Collider2D other)
    {
        // 시작 위치가 설정되지 않았고, 다른 오브젝트가 플레이어라면 수집 상태를 해제하고 시간을 초기화합니다.
        if (startPos == Vector3.zero && other.CompareTag("Player"))
        {
            isLooting = false;
            currentTime = 0;
        }
    }
}*/
