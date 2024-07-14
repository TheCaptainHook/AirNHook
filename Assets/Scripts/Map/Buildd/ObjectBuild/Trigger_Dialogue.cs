using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Trigger_Dialogue : MonoBehaviour
{
    public int _DialogueId;
    private bool OnExcuted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player component) && !OnExcuted)
        {
            OnExcuted = true;
            //UI_Dialogue active
        }
    }
}
