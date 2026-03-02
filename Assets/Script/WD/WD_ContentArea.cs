using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D), typeof(SpriteRenderer))]
public class WDContentArea : MonoBehaviour
{
    [SerializeField] private GameObject visuals;
    private SpriteRenderer _sr;
    private EdgeCollider2D _ec;
    private Vector3 _lastSize;

    // visuals에서 SpriteRenderer를 안전하게 가져옴
    private SpriteRenderer _spriteRenderer => _sr ??= visuals.GetComponent<SpriteRenderer>();
    private EdgeCollider2D _edgeCollider => _ec ??= GetComponent<EdgeCollider2D>();

    [Header("Window Layout")]
    public float toppadding;
    public float sidepadding;
    public float bottompadding;
    public float innerpadding = 0.2f;

    [Header("Constraint Settings")]
    public Transform playerTransform;

    void Awake()
    {   
        toppadding = WD_Manager.Instance.titleBarHeight;
        sidepadding = WD_Manager.Instance.sideEdge;
        bottompadding = WD_Manager.Instance.bottomEdge;
        UpdateContentAreaSize();
        _lastSize = _spriteRenderer.bounds.size;
    }

    void Update()
    {
        // 1. 크기 변화 감지 및 콜라이더 갱신
        if (_spriteRenderer.bounds.size != _lastSize)
        {
            UpdateContentAreaSize();
            _lastSize = _spriteRenderer.bounds.size;
        }
    }

    void LateUpdate()
    {
        // 2. 캐릭터가 창 밖으로 나가지 못하게 실시간 보정
        // 캐릭터 이동(Update/FixedUpdate)이 모두 끝난 뒤인 LateUpdate가 최적의 타이밍이야.
        if (playerTransform != null)
        {
            KeepPlayerInside();
        }
    }

    public void UpdateContentAreaSize()
    {
        if (_spriteRenderer == null || _spriteRenderer.sprite == null) return;

        // 월드 크기를 현재 오브젝트의 '전역 스케일'로 나누어 순수 로컬 좌표를 구함
        Vector3 worldSize = _spriteRenderer.bounds.size;
        float halfW = worldSize.x / (2f * transform.lossyScale.x);
        float halfH = worldSize.y / (2f * transform.lossyScale.y);
        
        // 상단바 높이도 스케일에 맞춰 보정
        float scaledTitleHeight = toppadding / transform.lossyScale.y;
        float scaledTitleLeft = sidepadding / transform.lossyScale.x;
        float scaledTitleRight = sidepadding / transform.lossyScale.x;
        float scaledTitleBottom = bottompadding / transform.lossyScale.y;
        
        float top = halfH - scaledTitleHeight;
        float left = -halfW + scaledTitleLeft;
        float right = halfW - scaledTitleRight;
        float bottom = -halfH + scaledTitleBottom;

        Vector2[] points = new Vector2[5]
        {
            new Vector2(left,  top),
            new Vector2(right, top),
            new Vector2(right, bottom),
            new Vector2(left,  bottom),
            new Vector2(left,  top)
        };

        _edgeCollider.points = points;
    }

    private void KeepPlayerInside()
    {
        // 창문의 중심점과 크기를 기준으로 캐릭터가 가야 할 최대/최소 범위 계산
        Vector3 center = new Vector3(0, 0, 0);
        float halfW = (_spriteRenderer.bounds.size.x / 2f) - innerpadding;
        float halfH = (_spriteRenderer.bounds.size.y / 2f) - innerpadding;

        Vector3 playerPos = playerTransform.position;

        // Mathf.Clamp를 이용해 플레이어의 위치를 강제로 창문 안으로 제한(Clamp 매개변수 1이 매개변수 2~3사이가 아니면 2~3 중 가까운 값으로 반환)
        float clampedX = Mathf.Clamp(playerPos.x, center.x - halfW, center.x + halfW);
        float clampedY = Mathf.Clamp(playerPos.y, center.y - halfH, center.y + halfH);

        // 위치가 변경되어야 한다면(즉, 창 밖으로 나갔다면) 위치 재설정
        if (playerPos.x != clampedX || playerPos.y != clampedY)
        {
            playerTransform.position = new Vector3(clampedX, clampedY, playerPos.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (visuals == null) return;
        var sr = visuals.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        Bounds b = sr.bounds;
        Vector3 center = b.center;

        // --- 1. 최종 플레이 가능 영역 (하늘색) ---
        Gizmos.color = Color.cyan;
        float rectW = b.size.x - (sidepadding * 2);
        float rectH = b.size.y - toppadding - bottompadding;
        Vector3 rectCenter = new Vector3(
            center.x, 
            center.y + (bottompadding / 2f) - (toppadding / 2f), 
            center.z
        );
        Gizmos.DrawWireCube(rectCenter, new Vector3(rectW, rectH, 0.1f));

        // --- 2. 캐릭터가 실제로 멈추는 경계 (내부 패딩 포함 - 노란색) ---
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(rectCenter, new Vector3(rectW - (innerpadding * 2), rectH - (innerpadding * 2), 0.1f));

        // --- 3. 패딩 가이드라인 (빨간색 선) ---
        Gizmos.color = Color.cyan;
        // 상단바 라인
        float topY = center.y + (b.size.y / 2f) - toppadding;
        Gizmos.DrawLine(new Vector3(center.x - b.size.x / 2f, topY, center.z), new Vector3(center.x + b.size.x / 2f, topY, center.z));
        // 하단 라인
        float botY = center.y - (b.size.y / 2f) + bottompadding;
        Gizmos.DrawLine(new Vector3(center.x - b.size.x / 2f, botY, center.z), new Vector3(center.x + b.size.x / 2f, botY, center.z));
    }
}