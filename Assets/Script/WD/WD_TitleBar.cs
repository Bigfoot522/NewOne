using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements; // 마우스 이벤트를 받기 위해 필수

[RequireComponent(typeof(BoxCollider2D))]
public class WD_TitleBar : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private BoxCollider2D _bc;
    private BoxCollider2D _boxCollider => _bc ??= GetComponent<BoxCollider2D>();
    private Transform wdPlatform;                // 이동시킬 최상위 부모
    [SerializeField] private GameObject visuals;

    private SpriteRenderer _visualsSR;
    private Vector3 _offset;
    private Vector3 _lastSize;
    private float titleBarHeight;

    private void Awake()
    {
        titleBarHeight = WD_Manager.Instance.titleBarHeight;

        if (visuals != null)
            _visualsSR = visuals.GetComponent<SpriteRenderer>();
        
        UpdateTitleBarSize();
    }
    
    private void Update()
    {
        // 실시간으로 비주얼 크기가 변하면 콜라이더 크기도 갱신 (가변 창문 대응)
        if (_visualsSR != null && _visualsSR.bounds.size != _lastSize)
        {
            UpdateTitleBarSize();
            _lastSize = _visualsSR.bounds.size;
        }
    }

    private void UpdateTitleBarSize()
    {
        if (_visualsSR == null) return;

        // Visuals의 월드 가로 길이를 가져와서 
        // TitleBar의 로컬 스케일에 맞춰 변환 (부모 스케일 영향 고려)
        float worldWidth = _visualsSR.bounds.size.x;
        float localWidth = worldWidth / transform.lossyScale.x;

        // 콜라이더의 가로 길이(size.x)만 갱신, 세로(size.y)는 인스펙터에서 설정한 값 유지
        _boxCollider.size = new Vector2(localWidth, titleBarHeight);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 클릭한 지점과 창 중심의 차이를 저장 (부드러운 드래그)
        _offset = wdPlatform.position - Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0; // 2D니까 Z축 고정
        wdPlatform.position = mousePos + _offset;
    }

    // 버튼 기능들 (인스펙터에서 버튼의 OnClick에 연결)
    public void OnClose() => wdPlatform.gameObject.SetActive(false);
    public void OnMinimize() => wdPlatform.localScale = Vector3.zero; // 혹은 특정 연출
    public void OnMaximize() => wdPlatform.localScale = Vector3.one * 2f; 
}