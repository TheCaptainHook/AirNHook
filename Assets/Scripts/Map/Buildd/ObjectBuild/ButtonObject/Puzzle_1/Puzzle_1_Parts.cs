
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle_1_Parts : MonoBehaviour,IInteractable
{

    private UI_Base _E_Btn;
    private bool is_E_BtnEnabled;
    [SerializeField] float _BtnOffset;

    [Header("Answer")]
    //test
    [SerializeField] Gradient wrongGradient;
    [SerializeField] Gradient correctGradient;
    [SerializeField] private GameObject _holderOpened;
    [SerializeField] private GameObject _holderClosed;
    [SerializeField] List<GameObject> numbering;

    public float boomArea;
    [Space(20)]
    [Header("Effect")]
    [SerializeField] ParticleSystem[] particles;

    [ReadOnly]
    public int puzzleAnswer;

    [ReadOnly]
    public int index;

    private Puzzle_1 puzzle_1;
    [SerializeField] LineRenderer lineRenderer;
    private PathFinder pathFinder;
    #region Components
    private Collider2D col;
    #endregion
    [Header("Interactable")]
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;


    private Puzzle_1_Parts_Net Parts_Net{
        get{return GetComponent<Puzzle_1_Parts_Net>();}
    }

    private void Awake(){
        col = GetComponent<Collider2D>();

        pathFinder = GetComponent<PathFinder>();
        lineRenderer.colorGradient = wrongGradient;
    }

    #region ------------------------------------------------------Network Field
    private bool OnSocket => (Parts_Net.item) ? true : false;
    public bool OnCorrect => Parts_Net.isCorrectAnswer;
    #endregion

    #region Insert,Remove
    public void InsertSocket(GameObject item)
    {
        Parts_Net.Cmd_SetSocketItem(item);
    }


    public void InsertAnimation(bool val)
    {
        if (val)
        {
            _holderOpened.SetActive(false);
            _holderClosed.SetActive(true);
        }
        else
        {
            _holderOpened.SetActive(true);
            _holderClosed.SetActive(false);
        }
    }
 

    public void Net_RemovEffect(){
        foreach (var p in particles) p.Play();
    }

    #endregion

    #region Answer

    public void Settting(Puzzle_1 puzzle_1,int answer,int index)
    {
        puzzleAnswer = answer;
        this.index = index;
        this.puzzle_1 = puzzle_1;
        if(Application.isPlaying)
        DrawPath(transform, puzzle_1.transform);
        numbering[index].SetActive(true);
        //
        lineRenderer.colorGradient=wrongGradient;

    }
    public bool CheckAnswer()
    {
        if (!OnSocket)
        {
            WrongAnswer();
            return false;
        }

        if (Parts_Net.GetItem.socketNumber == puzzleAnswer)
        {
            InCorrectAnswer();
            return true;
        }
        else
        {
            WrongAnswer();
            return false;
        }
    }

    public void Boom(ref HashSet<Collider2D> col)
    {
        if (OnCorrect) return;
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
        Parts_Net.Cmd_SetCorrect(true);
    }

    public void Net_SetCorrectEffect()
    {
        lineRenderer.colorGradient = correctGradient;
    }
    private void WrongAnswer()
    {

        Parts_Net.Cmd_RemoveEffect();
        Parts_Net.Cmd_SetSocketItem(null);
    }
    #endregion


    #region Network
   
    #endregion


    

    private void OnTriggerEnter2D(Collider2D collider){
        if(OnCorrect) return;
         if(collider.TryGetComponent(out HookSM component)){
            Transform grabItem = component.GetGrabbedItem();
            if(grabItem != null){
                if(grabItem.TryGetComponent(out Puzzle_1_Item component1)){

                    //ShowBtn();
                    Parts_Net.Cmd_ShowShowBtn();
                    // component1.PossibleInsertSocket(this);
                    component1.Net_HandleSetParts(this);
                }
            }
            else
            {
                if (OnSocket)
                {
                    //ShowBtn();
                    Parts_Net.Cmd_ShowShowBtn();
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
                    component1.Net_HandleSetParts(null);
                }
                
            }
        }
        
    }

    #region UI
    public void ShowBtn()
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
    private bool IsCorrectAnswer => Parts_Net.isCorrectAnswer;
    public void Interaction(Transform accessor = null)
    {
        if ( OnSocket && !IsCorrectAnswer)
        {

            Parts_Net.Cmd_SetSocketItem(null);
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



    #region Path
    public void DrawPath(Transform start, Transform end)
    {
        Vector2Int startPot = pathFinder.WorldToGrid(start.position);
        Vector2Int endPot = pathFinder.WorldToGrid(end.position);

        List<Vector2Int> path = pathFinder.FindPath(startPot, endPot);

        if (path == null) { return; }

        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 worldPosition = pathFinder.GridToWorld(path[i]);
            lineRenderer.SetPosition(i, worldPosition);
        }
    }

    #endregion

    public Vector2 GetPosition()
    {
        return new Vector2(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
            );
    }


}
