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

    private readonly Dictionary<CursorType, Texture2D> _cursorTextures = new();
    private Texture2D _inGameCursorOriginal;  // 원본 텍스처 (immutable, read-only)
    private Texture2D _editableInGameCursor;    // 이 사본을 실제로 수정하여 사용

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
            // 원본 텍스처를 새로 생성해서 복사 (완전히 writable한 상태)
            _inGameCursorOriginal = new Texture2D(inGameCursor.width, inGameCursor.height, TextureFormat.RGBA32, mipChain: false);
            _inGameCursorOriginal.filterMode = inGameCursor.filterMode;
            _inGameCursorOriginal.wrapMode = inGameCursor.wrapMode;
            _inGameCursorOriginal.SetPixels(inGameCursor.GetPixels());
            _inGameCursorOriginal.Apply();
            Debug.Log("[CursorManager] _inGameCursorOriginal created successfully.");

            // 수정 가능한 사본 생성 (초기 상태는 원본과 동일)
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
        if (_editableInGameCursor != null)
            Cursor.SetCursor(_editableInGameCursor, _centerHotspot, CursorMode.Auto);
        else
            Debug.LogWarning("Editable InGame cursor not initialized.");
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
        if (!_cursorTextures.TryGetValue(type, out var texture) || texture == null)
        {
            Debug.LogWarning($"[CursorManager] Cursor of type '{type}' not loaded.");
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }
        Vector2 hotspot = hotspotOverride ?? Vector2.zero;
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }

    /// <summary>
    /// Tint the in-game cursor texture while preserving its original brightness.
    /// For each pixel in the original (immutable) in-game cursor texture, it applies the tint's Hue and Saturation
    /// while keeping the original pixel’s Value.
    /// </summary>
    /// <param name="tintColor">Tint color: hue and saturation are applied; the original brightness (value) is preserved.</param>
    public void UpdateInGameCursorColor(Color tintColor)
    {
        if (_inGameCursorOriginal == null || _editableInGameCursor == null)
        {
            Debug.LogWarning("Editable InGame cursor not initialized.");
            return;
        }

        Debug.Log($"[CursorManager] UpdateInGameCursorColor called with tintColor: {tintColor}");

        // Extract tint's hue and saturation (value ignored)
        float tintHue, tintSat, dummy;
        Color.RGBToHSV(tintColor, out tintHue, out tintSat, out dummy);

        Color[] originalPixels = _inGameCursorOriginal.GetPixels();
        Color[] newPixels = new Color[originalPixels.Length];

        for (int i = 0; i < originalPixels.Length; i++)
        {
            Color orig = originalPixels[i];
            float origHue, origSat, origVal;
            Color.RGBToHSV(orig, out origHue, out origSat, out origVal);

            // Create new color with tint's hue and saturation, preserving original brightness (value)
            Color newPixel = Color.HSVToRGB(tintHue, tintSat, origVal);
            newPixel.a = orig.a; // 원래 알파 유지
            newPixels[i] = newPixel;
        }

        // For debugging, log sample pixel values (첫 번째 픽셀)
        if (newPixels.Length > 0)
        {
            Debug.Log($"[CursorManager] Sample pixel before tint: {_inGameCursorOriginal.GetPixel(0, 0)}, after tint: {newPixels[0]}");
        }

        _editableInGameCursor.SetPixels(newPixels);
        _editableInGameCursor.Apply();

        Debug.Log("[CursorManager] Updated in-game cursor texture applied.");

        Cursor.SetCursor(_editableInGameCursor, _centerHotspot, CursorMode.Auto);
    }
}
