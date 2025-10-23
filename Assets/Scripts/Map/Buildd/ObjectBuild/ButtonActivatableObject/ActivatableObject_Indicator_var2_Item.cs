
using UnityEngine;

public class ActivatableObject_Indicator_var2_Item : MonoBehaviour
{
    [SerializeField] SpriteRenderer mainSprite;


    [SerializeField] GameObject on_Go;
    [SerializeField] GameObject off_Go;
  


    

    #region  Mark. (TEST/0627)
    public void Mark_Green()
    {
        if (off_Go.activeSelf) off_Go.SetActive(false);
        on_Go.SetActive(true);
    }
    public void Mark_ShutDown()
    {
        if (on_Go.activeSelf) on_Go.SetActive(false);
        off_Go.SetActive(true);
    }
    #endregion
    
}
