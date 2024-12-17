
using Steamworks;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Puzzle_1_Parts : MonoBehaviour,IInteractable
{
    public Puzzle_1_Item onSocketItem;
    private bool onSocket;
    
    private UI_Base _E_Btn;
    private bool is_E_BtnEnabled;
    [SerializeField] float _BtnOffset;

    
    private SpriteRenderer sprite; //Test
    public float boomArea;
    [SerializeField] ParticleSystem[] particles;

    [ReadOnly]
    public int puzzleAnswer;
    [ReadOnly]
    public bool isCorrectAnswer;

    #region Components
    private Collider2D col;
    #endregion
    [Header("Interactable")]
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;
    private void Awake(){
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    #region Insert,Remove
    public void InsertSocket(Puzzle_1_Item item)
    {
        if (onSocketItem)
        {
            RemoveSocket();
            onSocketItem = item;
            return;
        }
        else
        {
            onSocketItem = item;
            HideEButton();

            col.enabled = false;
            col.enabled = true;
        }

    }
    #endregion

    #region Answer
    
    public void SetAnswer(int answer)
    {
        puzzleAnswer = answer;
    }
    public void CheckAnswer()
    {
        if (!onSocketItem)
        {
            WrongAnswer();
            return;
        }

        if (onSocketItem.socketNumber == puzzleAnswer) 
        {
            InCorrectAnswer();
        }
        else
        {
            WrongAnswer();
        }
    }

    public void Boom(ref HashSet<Collider2D> col)
    {
        if (isCorrectAnswer) return;
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");

        //effect

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, boomArea,playerLayerMask);
        foreach (Collider2D c in cols)
        {
            col.Add(c);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, boomArea);
    }

    private void InCorrectAnswer()
    {
        isCorrectAnswer = true;
        onSocketItem.GetComponent<Collider2D>().enabled = false;
        //animation
        sprite.color = Color.green; //test
    }
    private void WrongAnswer()
    {
        RemoveSocket();
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collider){
        if(isCorrectAnswer) return;
         if(collider.TryGetComponent(out HookSM component)){
            Transform grabItem = component.GetGrabbedItem();
            if(grabItem != null){
                if(grabItem.TryGetComponent(out Puzzle_1_Item component1)){
                    
                    ShowBtn();
                    component1.PossibleInsertSocket(this);
                }
            }
            else
            {
                if (onSocketItem)
                {
                    ShowBtn();
                }
                
            }
        }
    }

    
    private void OnTriggerExit2D(Collider2D collider){
        if(collider.TryGetComponent(out HookSM component)){
            
            Transform grabItem = component.GetGrabbedItem();
            if(grabItem != null){
                if(grabItem.TryGetComponent(out Puzzle_1_Item component1)){
                    HideEButton();
                    component1.UnPossibleInsertSocket();
                }
                
            }
        }
        
    }
    public void RemoveSocket()
    {
        if (onSocketItem)
        {
            foreach(var p in particles) p.Play();
            onSocketItem.RemoveSocket();
            onSocketItem = null;
            HideEButton();
        }

        isCorrectAnswer = false;
        //animation
        sprite.color = Color.red; //test
    }
    #region UI
    private void ShowBtn()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    public void ShowEButton()
    {

        return;
    }
    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }


    #endregion

    #region Interaction
    public void Interaction(Transform accessor = null)
    {
        if (onSocketItem != null && !isCorrectAnswer)
        {
            RemoveSocket();
        }
    }
    public bool CanInteract()
    {
        return true;
    }

    public void Interacting(bool value) { }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }
    #endregion






}
