using System;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    public float oldPosition;

    private void Awake()
    {
        Init();
    }

    public void Init()
    { 
        onCameraTranslate = null;
        Debug.Log("CameraInit");
        oldPosition = transform.position.x;
    }

    private void Update()
    {
        if (transform.position.x != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                float delta = oldPosition - transform.position.x;
                onCameraTranslate(delta);
            }

            oldPosition = transform.position.x;
        }
    }
}