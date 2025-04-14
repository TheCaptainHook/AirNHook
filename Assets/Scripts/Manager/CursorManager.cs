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
    private Texture2D _inGameCursorOriginal;
    private Texture2D _editableInGameCursor;

    private readonly string _resourcePath = "Arts/Cursors/";
    private Vector2 _centerHotspot = Vector2.zero;

    private const string CursorColorKey = "CursorColor_HSV";

    public void SetUp()
    {
        LoadCursor(CursorType.Default, "Default");
        LoadCursor(CursorType.DragIdle, "DragIdle");
        LoadCursor(CursorType.DragActive, "DragActive");
        LoadCursor(CursorType.InGame, "InGame");

        if (_cursorTextures.TryGetValue(CursorType.InGame, out var inGameCursor) && inGameCursor != null)
        {
            _inGameCursorOriginal = new Texture2D(inGameCursor.width, inGameCursor.height, TextureFormat.RGBA32, false);
            _inGameCursorOriginal.filterMode = inGameCursor.filterMode;
            _inGameCursorOriginal.wrapMode = inGameCursor.wrapMode;
            _inGameCursorOriginal.SetPixels(inGameCursor.GetPixels());
            _inGameCursorOriginal.Apply();

            _editableInGameCursor = new Texture2D(inGameCursor.width, inGameCursor.height, TextureFormat.RGBA32, false);
            _editableInGameCursor.filterMode = inGameCursor.filterMode;
            _editableInGameCursor.wrapMode = inGameCursor.wrapMode;
            _editableInGameCursor.SetPixels(_inGameCursorOriginal.GetPixels());
            _editableInGameCursor.Apply();

            _centerHotspot = new Vector2(_editableInGameCursor.width / 2, _editableInGameCursor.height / 2);
        }
        LoadSavedColor();
    }

    private void LoadCursor(CursorType type, string fileName)
    {
        var texture = Resources.Load<Texture2D>(_resourcePath + fileName);
        if (texture != null)
            _cursorTextures[type] = texture;
    }

    public void SetDefaultCursor() => SetCursor(CursorType.Default);
    public void SetDragIdleCursor() => SetCursor(CursorType.DragIdle);
    public void SetDragActiveCursor() => SetCursor(CursorType.DragActive);
    public void SetInGameCursor()
    {
        if (_editableInGameCursor != null)
        {
            Cursor.SetCursor(_editableInGameCursor, _centerHotspot, CursorMode.Auto);
            _currentCursorType = CursorType.InGame;
        }
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
            Cursor.SetCursor(texture, hotspotOverride ?? Vector2.zero, CursorMode.Auto);
            _currentCursorType = type;
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            _currentCursorType = CursorType.Default;
        }
    }

    public void UpdateInGameCursorColor(Color tintColor)
    {
        if (_inGameCursorOriginal == null) return;

        Color[] originalPixels = _inGameCursorOriginal.GetPixels();
        Color[] newPixels = new Color[originalPixels.Length];

        for (int i = 0; i < originalPixels.Length; i++)
        {
            Color orig = originalPixels[i];
            float brightness = orig.grayscale;
            newPixels[i] = new Color(tintColor.r * brightness, tintColor.g * brightness, tintColor.b * brightness, orig.a);
        }

        if (_editableInGameCursor != null)
        {
            UnityEngine.Object.Destroy(_editableInGameCursor);
            _editableInGameCursor = null;
        }

        _editableInGameCursor = new Texture2D(_inGameCursorOriginal.width, _inGameCursorOriginal.height, TextureFormat.RGBA32, false);
        _editableInGameCursor.SetPixels(newPixels);
        _editableInGameCursor.Apply();
    }

    public void SaveColor(float h, float s, float v)
    {
        PlayerPrefs.SetString(CursorColorKey, $"{h},{s},{v}");
        PlayerPrefs.Save();
    }

    private void LoadSavedColor()
    {
        if (PlayerPrefs.HasKey(CursorColorKey))
        {
            string[] hsv = PlayerPrefs.GetString(CursorColorKey).Split(',');
            if (hsv.Length == 3 && float.TryParse(hsv[0], out float h) && float.TryParse(hsv[1], out float s) && float.TryParse(hsv[2], out float v))
            {
                Color restored = Color.HSVToRGB(h, s, v);
                UpdateInGameCursorColor(restored);
            }
        }
    }

    public (float h, float s, float v)? GetSavedHSV()
    {
        if (PlayerPrefs.HasKey(CursorColorKey))
        {
            string[] hsv = PlayerPrefs.GetString(CursorColorKey).Split(',');
            if (hsv.Length == 3 && float.TryParse(hsv[0], out float h) && float.TryParse(hsv[1], out float s) && float.TryParse(hsv[2], out float v))
            {
                return (h, s, v);
            }
        }
        return null;
    }
}
