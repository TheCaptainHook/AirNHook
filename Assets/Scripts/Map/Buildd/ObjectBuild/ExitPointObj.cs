using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExitPointObj : BuildBase
{
    [Header("State")]
    [SerializeField] bool stageClear;

    [Header("Info")]
    [SerializeField] int condition_KeyAmount;
    [SerializeField] int current_KeyAmount;
    
    private void FixedUpdate()
    {
        BuildCheck();
    }




}
