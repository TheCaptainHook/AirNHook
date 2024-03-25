using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewControl : MonoBehaviour
{
    public RectTransform content;
    float minY = 0f;
    [SerializeField] float maxY;


    private void LateUpdate()
    {
        float clamp = Mathf.Clamp(content.anchoredPosition.y, minY, maxY);
        content.anchoredPosition = new Vector3(0, clamp, 0);
    }

}
