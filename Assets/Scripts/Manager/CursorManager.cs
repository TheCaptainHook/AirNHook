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
    private readonly string _resourcePath = "Arts/Cursors/";
    private Vector2 _centerHotspot = Vector2.zero;

    public void SetUp()
    {
        LoadCursor(CursorType.Default, "Default");
        LoadCursor(CursorType.DragIdle, "DragIdle");
        LoadCursor(CursorType.DragActive, "DragActive");
        LoadCursor(CursorType.InGame, "InGame");

        if (_cursorTextures.TryGetValue(CursorType.InGame, out var inGameCursor) && inGameCursor != null)
            _centerHotspot = new Vector2(inGameCursor.width / 2, inGameCursor.height / 2);
        else
            Debug.LogWarning("InGame cursor is missing or failed to load.");
        //if (_cursorTextures.TryGetValue(CursorType.DragIdle, out var dragIdleCursor) && dragIdleCursor != null)
        //    _centerHotspot = new Vector2(dragIdleCursor.width / 2, dragIdleCursor.height / 2);
        //else
        //    Debug.LogWarning("DragIdle cursor is missing or failed to load.");
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
    }

    public void SetDefaultCursor() => SetCursor(CursorType.Default);
    public void SetDragIdleCursor() => SetCursor(CursorType.DragIdle);
    public void SetDragActiveCursor() => SetCursor(CursorType.DragActive);
    public void SetInGameCursor() => SetCursor(CursorType.InGame, _centerHotspot);

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
}
