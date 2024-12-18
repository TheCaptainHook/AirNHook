using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle_1_Button : MonoBehaviour
{
    [SerializeField] Puzzle_1_ChargingBar chargingBar;

    private AirSM air;

    private void Update()
    {
        //Test
        if (Input.GetMouseButton(0) && air)
        {
            Charging();
        }
        if (Input.GetMouseButtonUp(0) || !air)

        {
            chargingBar.onCharging = false;
        }
        //Test
    }

    public void Charging()
    {
        chargingBar.Charging();
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if(collision.TryGetComponent(out AirSM component))
            {
                air = component;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out AirSM component))
            {
                air = null;
            }
        }
    }

}
