using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePointerEntity : MonoBehaviour,IPointerExitHandler, IPointerEnterHandler,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler
{
   public virtual void OnPointerEnter(PointerEventData data)
   {
        
   }

   public virtual void OnPointerExit(PointerEventData data)
   {

   }

   public virtual void OnPointerClick(PointerEventData data)
   {

   }
    public virtual void OnPointerDown(PointerEventData data)
    {

    }
    public virtual void OnPointerUp(PointerEventData data)
    {

    }

}
