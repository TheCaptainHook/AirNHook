using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public static CameraHolder Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }




    [SerializeField] Camera StageSelectViewCamera;
    public Camera StageSelectCamera()
    {
        StageSelectViewCamera.gameObject.SetActive(true);
        return StageSelectViewCamera;
    }
    public void ShutDownStageSelectCamera()
    {
        StageSelectViewCamera.gameObject.SetActive(false);
    }

}
