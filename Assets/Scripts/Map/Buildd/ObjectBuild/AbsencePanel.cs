using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AbsencePanel : MonoBehaviour
{
    //[SerializeField] SpriteRenderer _Empty;
    //[SerializeField] SpriteRenderer _Air;
    //[SerializeField] SpriteRenderer _Hook;
    //[SerializeField] SpriteRenderer _Full;

    private bool _OnAir, _OnHook;


    [SerializeField] Animator animator;

    //private void Reset()
    //{
    //    _Air.enabled = false;
    //    _Hook.enabled = false;
    //    _Full.enabled = false;

    //}


    public void SetPanel(Player player)
    {
        CharacterType type = player.characterType;

        switch (type)
        {
            case CharacterType.Hook:
                if (_OnHook)
                {
                    _OnHook = false;
                  //Set 
                }
                else
                {
                    _OnHook = true;
                }

                
                break;
            case CharacterType.Air:
                if (_OnAir)
                {
                    _OnAir = false;
                }
                else
                {
                    _OnAir = true;
                }
                break;
        }

    }

    public async Task NextMoveAnimation()
    {
        
    }

   
}
