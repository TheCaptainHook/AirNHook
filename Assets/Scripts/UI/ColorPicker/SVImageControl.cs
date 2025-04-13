using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] private Image _pickerImage;
    private RawImage _svimage;
    [SerializeField] private ColorPickerControl CC;

    private RectTransform _rectTransform, _pickerTransform;

    private void Awake()
    {
        _svimage = GetComponent<RawImage>();
        _rectTransform = GetComponent<RectTransform>();
        _pickerTransform = _pickerImage.GetComponent<RectTransform>();

        _pickerTransform.SetParent(_rectTransform);
        _pickerTransform.localPosition = Vector2.zero;
    }

    public void Initialize(ColorPickerControl picker)
    {
        CC = picker;
    }

    private void UpdateColour(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            return;
        }

        // RectTransform 좌표 기준 보정
        Vector2 rectSize = _rectTransform.rect.size;
        Vector2 pivot = _rectTransform.pivot;
        Vector2 zeroBasedPoint = new Vector2(
            localPoint.x + rectSize.x * pivot.x,
            localPoint.y + rectSize.y * pivot.y);

        // Clamp to bounds
        zeroBasedPoint.x = Mathf.Clamp(zeroBasedPoint.x, 0, rectSize.x);
        zeroBasedPoint.y = Mathf.Clamp(zeroBasedPoint.y, 0, rectSize.y);

        // 정규화 좌표 계산
        Vector2 normalized = new Vector2(
            zeroBasedPoint.x / rectSize.x,
            zeroBasedPoint.y / rectSize.y);

        // Picker 위치 설정 (pivot 고려하여 다시 -center 보정)
        _pickerTransform.localPosition = new Vector2(
            zeroBasedPoint.x - rectSize.x * pivot.x,
            zeroBasedPoint.y - rectSize.y * pivot.y);

        // 단순 시각화용 (현재 Hue 미사용)
        _pickerImage.color = Color.HSVToRGB(0, 0, 1 - normalized.y);

        // SV 업데이트
        CC.SetSV(normalized.x, normalized.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateColour(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColour(eventData);
    }
}


