
using TMPro;
using UnityEngine;

public class TEST_FPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _fps_text;



    float _deltaTime = 0.0f;
    
    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;

        _fps_text.text = string.Format("{0:0.}", fps);
    }
}
