using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public Transform player;
    public DialogueBox dialogueBox;
    private Rigidbody2D rb;
    public Sprite[] downSprites;
    public Sprite[] upSprites;
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;
    private Sprite[] currentSprites;
    private float frameTimer = 0f;
    private int frameIndex = 0;
    public float frameTime = 0.15f;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // 1. 핵심 수정: 시작할 때 기본 스프라이트 배열을 지정해 줌
        currentSprites = downSprites;

        // 2. 인스펙터에서 놓쳤거나 씬 이동 후 참조가 끊겼을 경우 자동 탐색
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
        if (dialogueBox == null)
        {
            dialogueBox = FindFirstObjectByType<DialogueBox>();
        }
    }

    void Update()
    {
        // 플레이어가 없으면 에러를 내지 않고 대기
        if (player == null) return;

        Vector2 toPlayer = player.position - transform.position;

        // dialogueBox가 연결되어 있을 때만 isOpen 확인
        bool isDialogueOpen = (dialogueBox != null && dialogueBox.isOpen);

        if (isDialogueOpen || toPlayer.magnitude > chaseRange)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = toPlayer.normalized * moveSpeed;

            if (rb.linearVelocity.y > 1f) { currentSprites = upSprites; }
            else if (rb.linearVelocity.y < -1f) { currentSprites = downSprites; }
            else if (rb.linearVelocity.x < 0) { currentSprites = leftSprites; }
            else { currentSprites = rightSprites; }
        }

        // 스프라이트 배열이 인스펙터에 제대로 등록되어 있을 때만 애니메이션 실행
        if (currentSprites != null && currentSprites.Length > 0)
        {
            if (rb.linearVelocity != Vector2.zero)
            {
                frameTimer -= Time.deltaTime;
                if (frameTimer <= 0f)
                {
                    frameTimer = frameTime;
                    frameIndex += 1;
                    if (frameIndex >= currentSprites.Length) { frameIndex = 0; }
                }
            }
            else
            {
                frameIndex = 0;
            }

            sr.sprite = currentSprites[frameIndex];
        }
    }
}
