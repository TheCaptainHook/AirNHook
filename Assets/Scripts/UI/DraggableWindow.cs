using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform _dragHandle;
    [SerializeField] private Canvas _canvas;

    private RectTransform _rectTransform;
    private Vector2 _originPosition;

    private bool _isDragging = false;
    private bool _pointerOverHandle = false;

    private enum CursorState
    {
        Default,
        DragIdle,
        DragActive
    }

    private CursorState _currentCursor = CursorState.Default;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _originPosition = _rectTransform.anchoredPosition;
    }

    private void OnDisable()
    {
        _rectTransform.anchoredPosition = _originPosition;
        _isDragging = false;
        _pointerOverHandle = false;
        SetCursor(CursorState.Default);
    }

    private void Update()
    {
        // 마우스 위치가 바뀌지 않았으면 검사 안 함
        if (_isDragging) return;

        Vector2 mousePosition = Input.mousePosition;

        // 핸들 영역에 마우스가 있는지 검사
        bool isOver = RectTransformUtility.RectangleContainsScreenPoint(
            _dragHandle, mousePosition, _canvas.worldCamera
        );

        // 상태가 변할 때만 커서 변경
        if (isOver != _pointerOverHandle)
        {
            _pointerOverHandle = isOver;
            SetCursor(_pointerOverHandle ? CursorState.DragIdle : CursorState.Default);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_pointerOverHandle)
        {
            _isDragging = true;
            SetCursor(CursorState.DragActive);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging)
        {
            _isDragging = false;

            // 마우스가 핸들 위에 있으면 idle로 돌아감
            SetCursor(_pointerOverHandle ? CursorState.DragIdle : CursorState.Default);
        }
    }

    private void SetCursor(CursorState state)
    {
        if (_currentCursor == state)
            return;

        _currentCursor = state;

        switch (state)
        {
            case CursorState.Default:
                Managers.CursorManager.SetDefaultCursor();
                break;
            case CursorState.DragIdle:
                Managers.CursorManager.SetDragIdleCursor();
                break;
            case CursorState.DragActive:
                Managers.CursorManager.SetDragActiveCursor();
                break;
        }
    }
}