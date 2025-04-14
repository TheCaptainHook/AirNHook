using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager
{
    private enum CursorType
    {
        Default,
        DragIdle,
        DragActive,
        InGame
    }

    private CursorType _currentCursorType = CursorType.Default;

    private readonly Dictionary<CursorType, Texture2D> _cursorTextures = new();
    private Texture2D _inGameCursorOriginal;  // 원본 텍스처 (immutable, read-only)
    private Texture2D _editableInGameCursor;    // 수정 가능한 사본

    private readonly string _resourcePath = "Arts/Cursors/";
    private Vector2 _centerHotspot = Vector2.zero;

    public void SetUp()
    {
        LoadCursor(CursorType.Default, "Default");
        LoadCursor(CursorType.DragIdle, "DragIdle");
        LoadCursor(CursorType.DragActive, "DragActive");
        LoadCursor(CursorType.InGame, "InGame");

        if (_cursorTextures.TryGetValue(CursorType.InGame, out var inGameCursor) && inGameCursor != null)
        {
            // 원본 텍스처를 새로 생성하여 복사 (writable)
            _inGameCursorOriginal = new Texture2D(inGameCursor.width, inGameCursor.height, TextureFormat.RGBA32, mipChain: false);
            _inGameCursorOriginal.filterMode = inGameCursor.filterMode;
            _inGameCursorOriginal.wrapMode = inGameCursor.wrapMode;
            _inGameCursorOriginal.SetPixels(inGameCursor.GetPixels());
            _inGameCursorOriginal.Apply();
            Debug.Log("[CursorManager] _inGameCursorOriginal created successfully.");

            // 수정 가능한 사본 생성 (초기 상태 동일)
            _editableInGameCursor = new Texture2D(inGameCursor.width, inGameCursor.height, TextureFormat.RGBA32, mipChain: false);
            _editableInGameCursor.filterMode = inGameCursor.filterMode;
            _editableInGameCursor.wrapMode = inGameCursor.wrapMode;
            _editableInGameCursor.SetPixels(_inGameCursorOriginal.GetPixels());
            _editableInGameCursor.Apply();
            Debug.Log("[CursorManager] _editableInGameCursor created successfully.");

            _centerHotspot = new Vector2(_editableInGameCursor.width / 2, _editableInGameCursor.height / 2);
            Debug.Log($"[CursorManager] Center Hotspot set to {_centerHotspot}");
        }
        else
        {
            Debug.LogWarning("InGame cursor is missing or failed to load.");
        }
    }

    private void LoadCursor(CursorType type, string fileName)
    {
        var texture = Resources.Load<Texture2D>(_resourcePath + fileName);
        if (texture == null)
        {
            Debug.LogWarning($"[CursorManager] Failed to load cursor: {_resourcePath}{fileName}");
            return;
        }
        _cursorTextures[type] = texture;
        Debug.Log($"[CursorManager] Loaded cursor {fileName} for type {type}");
    }

    public void SetDefaultCursor() => SetCursor(CursorType.Default);
    public void SetDragIdleCursor() => SetCursor(CursorType.DragIdle);
    public void SetDragActiveCursor() => SetCursor(CursorType.DragActive);
    public void SetInGameCursor()
    {
        if (_editableInGameCursor == null)
        {
            Debug.LogWarning("EditableInGameCursor not set.");
            return;
        }

        Cursor.SetCursor(_editableInGameCursor, Vector2.zero, CursorMode.Auto);
        _currentCursorType = CursorType.InGame;

        Debug.Log("[CursorManager] InGame 커서로 전환 완료");
    }


    public void ClearCursor()
    {
        if (Managers.Game.CurrentState is GameState.Game or GameState.Lobby)
            SetInGameCursor();
        else
            SetDefaultCursor();
    }

    private void SetCursor(CursorType type, Vector2? hotspotOverride = null)
    {
        if (_cursorTextures.TryGetValue(type, out var texture) && texture != null)
        {
            Vector2 hotspot = hotspotOverride ?? Vector2.zero;
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
            _currentCursorType = type;
            Debug.Log($"[CursorManager] Cursor changed to {type}");
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            _currentCursorType = CursorType.Default;
            Debug.LogWarning($"[CursorManager] Failed to set cursor: {type}");
        }
    }

    /// <summary>
    /// Updates the in-game cursor texture by applying a tint.
    /// This method multiplies the tintColor with the grayscale (brightness) of each pixel in the original texture,
    /// so that the original brightness contrast is preserved.
    /// </summary>
    /// <param name="tintColor">The tint color to apply. Only its RGB values are used; the resulting brightness comes from the original texture.</param>
    public void UpdateInGameCursorColor(Color tintColor)
    {
        if (_inGameCursorOriginal == null)
        {
            Debug.LogWarning("InGameCursorOriginal not initialized.");
            return;
        }

        // 원본 텍스처에서 픽셀 읽기
        Color[] originalPixels = _inGameCursorOriginal.GetPixels();
        Color[] newPixels = new Color[originalPixels.Length];

        for (int i = 0; i < originalPixels.Length; i++)
        {
            Color orig = originalPixels[i];
            float brightness = orig.grayscale;

            // 픽셀 색상 계산 (명도 유지, 색상 덮기)
            newPixels[i] = new Color(
                tintColor.r * brightness,
                tintColor.g * brightness,
                tintColor.b * brightness,
                orig.a);
        }

        // 기존 텍스처 메모리 해제 (메모리 누수 방지)
        if (_editableInGameCursor != null)
        {
            UnityEngine.Object.Destroy(_editableInGameCursor);
            _editableInGameCursor = null;
        }

        // 새로운 텍스처 생성
        Texture2D newTexture = new Texture2D(
            _inGameCursorOriginal.width,
            _inGameCursorOriginal.height,
            TextureFormat.RGBA32,
            false);

        newTexture.SetPixels(newPixels);
        newTexture.Apply();

        // 새 텍스처 적용
        _editableInGameCursor = newTexture;

        Debug.Log("[CursorManager] InGame 커서 색상 업데이트 및 새 텍스처 적용 완료");
    }

}

