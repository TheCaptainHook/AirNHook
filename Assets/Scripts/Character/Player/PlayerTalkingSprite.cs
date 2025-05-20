using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalkingSprite : MonoBehaviour
{
    public float timer = 0f;
    public float timeToDisable = 0.7f;


    private void Start()
    {
        StartVoice();
    }

    private void Update()
    {
        if (timer < timeToDisable)
        {
            timer += Time.deltaTime;
        }
        else
        {
            DisableObject();
        }
    }

    public void StartVoice()
    {
        timer = 0f;
    }

    public void DisableObject()
    {
        timer = 0f;
        gameObject.SetActive(false);
    }
}
