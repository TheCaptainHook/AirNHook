using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_1_RightTrigger : MonoBehaviour
{
    [SerializeField] Puzzle_1 puzzle_1;
    public AirSM air;
    public Transform airWeaponPivot;


    private void Update()
    {
        if (Input.GetMouseButton(1) && air)
        {
            if (GetReadyToCharge(GetAirDir()) && !air.airGun._inhaling)
            {
                //Charging;
                puzzle_1.Charging();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out AirSM component))
            {
                air = component;
                //dir
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


    private Vector3 GetAirDir()
    {
        if (air == null) return Vector3.zero;

        if (airWeaponPivot == null)
        {
            foreach (Transform tr in air.transform)
            {
                if (tr.name == "WeaponPivot")
                {
                    airWeaponPivot = tr;
                }
            }
        }

        return airWeaponPivot.rotation.eulerAngles;

    }

    private bool GetReadyToCharge(Vector3 rot)
    {
        Debug.Log(rot);
        float z = rot.z - 360;
        if (rot.y == 180 && (z >= -10 && z <= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
