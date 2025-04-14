using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerControl : MonoBehaviour
{
    public float currenHue, currentSat, currentVal;

    [SerializeField] private RawImage _hueImage, _satValImage, _outputImage;
    [SerializeField] private Slider _hueSlider;
    [SerializeField] private TMP_InputField _hexInputField;
    private Texture2D _hueTexture, _svTexture, _outputTexture;

    [SerializeField] private SVImageControl svControl;

    [SerializeField] private Image _preview;

    private void Start()
    {
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();

        _hueSlider.onValueChanged.AddListener(delegate { OnHueSliderChanged(); });

        svControl.Initialize(this); // SVImageControl과 연결

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
            // 변경: Value를 1로 해서 선명한 색상을 만듦
            _hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / _hueTexture.height, 1, 1f));
        }

        _hueTexture.Apply();
        currenHue = 0;

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
        // TextureFormat RGBA32, mipChain false로 생성하여 커서 조건에 맞게 만듦
        _outputTexture = new Texture2D(1, 16, TextureFormat.RGBA32, mipChain: false);
        _outputTexture.wrapMode = TextureWrapMode.Clamp;
        _outputTexture.name = GlobalText.OUTPUTTEXTURE;

        _outputImage.texture = _outputTexture;
    }

    public void OnHueSliderChanged()
    {
        currenHue = _hueSlider.value;
        UpdateSVImage();
        UpdateOutputImage();
    }

    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;
        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        for (int y = 0; y < _svTexture.height; y++)
        {
            for (int x = 0; x < _svTexture.width; x++)
            {
                _svTexture.SetPixel(x, y, Color.HSVToRGB(
                    currenHue,
                    (float)x / (_svTexture.width - 1),
                    (float)y / (_svTexture.height - 1)));
            }
        }

        _svTexture.Apply();
    }

    private void UpdateOutputImage()
    {
        Color currentColour = Color.HSVToRGB(currenHue, currentSat, currentVal);
        currentColour.a = 1f;

        for (int i = 0; i < _outputTexture.height; i++)
        {
            _outputTexture.SetPixel(0, i, currentColour);
        }

        _outputTexture.Apply();

        _hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColour);

        // UI 프리뷰 이미지 업데이트
        _outputImage.color = currentColour;
        _preview.color = currentColour;
    }


    public void OnTextInput()
    {
        if (_hexInputField.text.Length < 6) return;

        if (ColorUtility.TryParseHtmlString("#" + _hexInputField.text, out var newCol))
        {
            Color.RGBToHSV(newCol, out currenHue, out currentSat, out currentVal);
            _hueSlider.value = currenHue;
            UpdateSVImage();
            UpdateOutputImage();
        }

        _hexInputField.text = "";
    }

    // 추가: 최종 선택된 색상 반환 함수 (Apply 시 사용)
    public Color GetFinalColor()
    {
        return Color.HSVToRGB(currenHue, currentSat, currentVal);
    }
}


