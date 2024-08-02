using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Mirror;
public class AbsencePanel : MonoBehaviour
{

    [SerializeField] GameObject _Air;
    [SerializeField] GameObject _Hook;


    private bool _OnAir, _OnHook;


    public void OnAbsencePanel()
    {
        gameObject.SetActive(true);
    }

    public void SetPanel(GameObject obj)
    {
        if (obj.TryGetComponent(out Hook hook))
        {
            if (_OnHook)
            {
                _OnHook = false;
                _Hook.SetActive(false);
                //Set 
            }
            else
            {
                _OnHook = true;
                _Hook.SetActive(true);
            }
        }
        else if (obj.TryGetComponent(out Air air))
        {
            if (_OnAir)
            {
                _OnAir = false;
                _Air.SetActive(false);
            }
            else
            {
                _OnAir = true;
                _Air.SetActive(true);
            }
        }


    }


    public void NextMoveAnimation()
    {
        Debug.Log("Animation");
    }
}
