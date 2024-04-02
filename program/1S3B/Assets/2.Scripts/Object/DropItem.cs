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

/*// Unity ������ �⺻ Ŭ������ ����ϱ� ���� ���ӽ����̽�
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// DropItem Ŭ���� ����, MonoBehaviour�� ��ӹ޾� Unity�� ���� ������Ʈ�� ���� �� �ִ� ������Ʈ�� �˴ϴ�.
public class DropItem : MonoBehaviour
{
    // GameManager Ÿ���� ���� ����, ������ �������� ������ ����մϴ�.
    private GameManager gameManager;
    // Player Ÿ���� ���� ����, �÷��̾� ������Ʈ�� �����մϴ�.
    private Player player;
    // Inspector â���� ������ �� �ִ� ��� ���� �������� �׷����մϴ�.
    [Header("Drop")]
    public Transform sprite; // �������� ��������Ʈ(�̹���) ��ġ�� ��Ÿ���� ����
    public Transform floor; // �ٴ��� ��ġ�� ��Ÿ���� ����
    // Inspector â���� ������ �� �ִ� ����(looting) ���� �������� �׷����մϴ�.
    [Header("Looting")]
    public CircleCollider2D circleCollider; // �������� ���� ������ ��Ÿ���� �ݶ��̴�
    // ��� ���� �����̺�(�����) ���� ����
    private int maxBounce = 3; // �ִ� ƨ�� Ƚ��
    private float xForce = 5; // x������ ���ϴ� ���� ��
    private float yForce = 10; // y������ ���ϴ� ���� ��
    private float gravity = 25; // �߷��� ũ��
    private Vector2 direction; // �������� ������ ����
    private int currentBounce = 0; // ������� ƨ�� Ƚ��
    private bool isGrounded = true; // �������� �ٴڿ� ��Ҵ����� ��Ÿ���� �÷���
    private float maxHeight; // �ִ� ����
    private float currentHeight; // ���� ����
    // ���� ���� �����̺� ���� ����
    private float range = 2; // ���� ����
    private Vector3 startPos; // ���� ���� ��ġ
    private float currentTime; // ���� �ð�(�����ϴ� �� �ɸ��� �ð��� ���)
    private float distanceSpeed = 5f; // ���� �� �̵� �ӵ�
    private bool isLooting = false; // ���� ���� �������� ��Ÿ���� �÷���
    // Start �޼ҵ�� ������Ʈ�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    private void Start()
    {
        // ���� �Ŵ��� �ν��Ͻ��� ������ �Ҵ��մϴ�.
        gameManager = GameManager.Instance;
        // ���� �Ŵ����κ��� �÷��̾� �ν��Ͻ��� ������ �Ҵ��մϴ�.
        player = gameManager.Player;
        // ���� ���� ����
        circleCollider.radius = range;
        // ���� ���̸� yForce ���� ������ �����ϰ� �����մϴ�.
        currentHeight = Random.Range(yForce - 1, yForce);
        // �ִ� ���̸� ���� ���̷� �����մϴ�.
        maxHeight = currentHeight;
        // ������ �����ϰ� �����ϸ� Init �޼ҵ� ȣ��� �ʱ�ȭ�մϴ�.
        Init(new Vector2(Random.Range(-xForce, xForce), Random.Range(-xForce, xForce)));
    }
    // ���� ������ �����ϴ� �޼ҵ�
    public void RadiusSetting(float val)
    {
        range = val; // ���� ���� �Ű������� �����մϴ�.
        circleCollider.radius = range; // �ݶ��̴��� �ݰ��� ���ο� ������ �����մϴ�.
    }
    // �� �����Ӹ��� ȣ��Ǵ� Update �޼ҵ��Դϴ�.
    private void Update()
    {
        // �������� ���� ���̰�, �ִ� ƨ�� Ƚ������ ���ų�, �������� �ִ� ���¶��
        if ((isLooting == true && currentBounce<=2)||(isLooting == false&& isGrounded == false))
        {
            // �߷��� ������ ���� ���̸� ������Ʈ�մϴ�.
            currentHeight += -gravity * Time.deltaTime;
            // ��������Ʈ�� ��ġ�� ���� ���̿� ���� ������Ʈ�մϴ�.
            sprite.position += new Vector3(0, currentHeight, 0) * Time.deltaTime;
            // ��ü ������Ʈ�� ��ġ�� ���⿡ ���� ������Ʈ�մϴ�.
            transform.position += (Vector3)direction * Time.deltaTime;
            // �� �ӵ��� ����մϴ�.
            float totalVelocity = Mathf.Abs(currentHeight) + Mathf.Abs(maxHeight);
            // ������ ����� ����մϴ�.
            float scaleXY = Mathf.Abs(currentHeight) / totalVelocity;
            // �ٴ��� �������� ������Ʈ�մϴ�.
            floor.localScale = Vector2.one * Mathf.Clamp(scaleXY, 0.5f, 1.0f);
            // �ٴڿ� ��Ҵ��� Ȯ���մϴ�.
            CheckFloorHit();
        }
        // ���� ���̰�, �ִ� ƨ�� Ƚ������ ���ٸ�, ���� ������ �޼ҵ带 ȣ���մϴ�.
        if (isLooting == true && currentBounce>2  )
            LootingItem();
    }
    // �������� �ʱ� ���¸� �����ϴ� �޼ҵ��Դϴ�.
    private void Init(Vector2 direction)
    {
        // �ٴڿ� ���� ���� ���·� �����մϴ�.
        isGrounded = false;
        // �ִ� ���̸� �����մϴ�.
        maxHeight /= 1.5f;
        // ������ �����մϴ�.
        this.direction = direction;
        // ���� ���̸� �ִ� ���̷� �����մϴ�.
        currentHeight = maxHeight;
        // ƨ�� Ƚ���� ������ŵ�ϴ�.
        currentBounce++;
        // ƨ�� Ƚ���� 2���� ũ�� ���� ��ġ�� ���� ��ġ�� �����մϴ�.
        if (currentBounce > 2)
            startPos = transform.position;
    }
    // �ٴڿ� ��Ҵ��� Ȯ���ϴ� �޼ҵ��Դϴ�.
    private void CheckFloorHit()
    {
        // ��������Ʈ�� y ��ġ�� �ٴں��� ���ٸ�, �� �ٴڿ� ��Ҵٸ�
        if (sprite.position.y < floor.position.y)
        {
            // ��������Ʈ�� ��ġ�� �ٴ��� ��ġ�� �����մϴ�.
            sprite.position = floor.position;
            // �ٴ��� �������� ������� �����մϴ�.
            floor.localScale = Vector3.one;
            // �ִ� ƨ�� Ƚ������ �۴ٸ� �ٽ� ƨ�ܿø��ϴ�.
            if (currentBounce < maxBounce)
                Init(direction / 1.5f);
            else
                // �ִ� ƨ�� Ƚ���� �����ߴٸ� �ٴڿ� ���� ���·� �����մϴ�.
                isGrounded = true;
        }
    }
    // �������� �����ϴ� �޼ҵ��Դϴ�.
    private void LootingItem()
    {
        // BoxCollider2D ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        GetComponent<BoxCollider2D>().enabled = false;
        // �÷��̾��� ��ġ�� �����ɴϴ�.
        Vector3 playerPos = player.transform.position;
        // �����۰� �÷��̾� ������ �Ÿ��� ����մϴ�.
        float distance = Vector3.Distance(transform.position, playerPos);
        // �ð��� ������Ʈ�մϴ�.
        currentTime += distanceSpeed * Time.deltaTime;
        // �������� ��ġ�� �÷��̾� ������ �̵���ŵ�ϴ�.
        this.transform.position = Vector3.Lerp(startPos, playerPos, currentTime);
        // �������� �÷��̾�� ����� ��������ٸ� �������� �ı��ϰ� ���� ���¸� �����մϴ�.
        if (distance < 0.5f)
        {
            Destroy(gameObject);
            isLooting = false;
        }
    }
    // �ٸ� ������Ʈ�� �� ������Ʈ�� Ʈ���� �ݶ��̴� ���� ������ �� ȣ��˴ϴ�.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���� ���°� �ƴϰ�, �ٸ� ������Ʈ�� �÷��̾��� ���� ���·� �����ϰ� �ð��� �ʱ�ȭ�մϴ�.
        if (isLooting == false && other.CompareTag("Player"))
        {
            isLooting = true;
            currentTime = 0;
        }
    }
    // �ٸ� ������Ʈ�� �� ������Ʈ�� Ʈ���� �ݶ��̴��� ������ �� ȣ��˴ϴ�.
    private void OnTriggerExit2D(Collider2D other)
    {
        // ���� ��ġ�� �������� �ʾҰ�, �ٸ� ������Ʈ�� �÷��̾��� ���� ���¸� �����ϰ� �ð��� �ʱ�ȭ�մϴ�.
        if (startPos == Vector3.zero && other.CompareTag("Player"))
        {
            isLooting = false;
            currentTime = 0;
        }
    }
}*/
