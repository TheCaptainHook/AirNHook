using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerControl : MonoBehaviour
{
    public float currentHue, currentSat, currentVal;

    [SerializeField] private RawImage _hueImage, _satValImage, _outputImage;
    [SerializeField] private Slider _hueSlider;
    [SerializeField] private TMP_InputField _hexInputField;
    private Texture2D _hueTexture, _svTexture, _outputTexture;

    [SerializeField] private SVImageControl svControl;

    [SerializeField] private Image _preview;


    public void Init()
    {
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();

        _hueSlider.onValueChanged.AddListener(delegate { OnHueSliderChanged(); });

        svControl.Initialize(this);

        // 강제 레이아웃 갱신
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)svControl.transform);

        UpdateSVImage();
        UpdateOutputImage();
    }

    private void CreateHueImage()
    {
        _hueTexture = new Texture2D(1, 16);
        _hueTexture.wrapMode = TextureWrapMode.Clamp;
        _hueTexture.name = GlobalText.HUETEXTURE;

        for (int i = 0; i < _hueTexture.height; i++)
        {
            _hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / _hueTexture.height, 1, 1f));
        }

        _hueTexture.Apply();
        currentHue = 0;

        _hueImage.texture = _hueTexture;
    }

    private void CreateSVImage()
    {
        _svTexture = new Texture2D(16, 16);
        _svTexture.wrapMode = TextureWrapMode.Clamp;
        _svTexture.name = GlobalText.SATVALTEXTURE;

        _satValImage.texture = _svTexture;
    }

    private void CreateOutputImage()
    {
        _outputTexture = new Texture2D(1, 16, TextureFormat.RGBA32, mipChain: false);
        _outputTexture.wrapMode = TextureWrapMode.Clamp;
        _outputTexture.name = GlobalText.OUTPUTTEXTURE;

        _outputImage.texture = _outputTexture;
    }

    public void OnHueSliderChanged()
    {
        currentHue = _hueSlider.value;
        UpdateSVImage();
        UpdateOutputImage();
    }

    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;
        UpdateOutputImage();
    }

    public void SetHSV(float h, float s, float v)
    {
        currentHue = h;
        currentSat = s;
        currentVal = v;

        _hueSlider.value = h;
        svControl.SetHandlePositionFromSV(s, v);
        UpdateSVImage();
        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        if (_svTexture == null)
        {
            Debug.LogError("UpdateSVImage called before _svTexture was initialized.");
            return;
        }

        for (int y = 0; y < _svTexture.height; y++)
        {
            for (int x = 0; x < _svTexture.width; x++)
            {
                _svTexture.SetPixel(x, y, Color.HSVToRGB(
                    currentHue,
                    (float)x / (_svTexture.width - 1),
                    (float)y / (_svTexture.height - 1)));
            }
        }

        _svTexture.Apply();
    }

    private void UpdateOutputImage()
    {
        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);
        currentColour.a = 1f;

        for (int i = 0; i < _outputTexture.height; i++)
        {
            _outputTexture.SetPixel(0, i, currentColour);
        }

        _outputTexture.Apply();

        _hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColour);
        _outputImage.color = currentColour;
        _preview.color = currentColour;
    }

    public void OnTextInput()
    {
        if (_hexInputField.text.Length < 6) return;

        if (ColorUtility.TryParseHtmlString("#" + _hexInputField.text, out var newCol))
        {
            Color.RGBToHSV(newCol, out currentHue, out currentSat, out currentVal);
            _hueSlider.value = currentHue;
            UpdateSVImage();
            UpdateOutputImage();
        }

        _hexInputField.text = "";
    }

    public Color GetFinalColor()
    {
        return Color.HSVToRGB(currentHue, currentSat, currentVal);
    }
}


