
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_1_Parts : MonoBehaviour,IInteractable
{
    public Puzzle_1_Item onSocketItem;
    private bool onSocket;
    
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
    public bool isCorrectAnswer;
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
        //sprite = GetComponent<SpriteRenderer>();
        pathFinder = GetComponent<PathFinder>();
        lineRenderer.colorGradient = wrongGradient;
    }

    #region Insert,Remove
    public void InsertSocket(Puzzle_1_Item item)
    {
        if (onSocketItem)
        {
            //RemoveSocket();
            Net_RemoveSocket(false);

            onSocketItem = item;
            return;
        }
        else
        {
            onSocketItem = item;
            HideEButton();

            // col.enabled = false;
            // col.enabled = true;
            // _holderOpened.SetActive(false);
            // _holderClosed.SetActive(true);
            Parts_Net.CmdLock();
        }

    }

    //public void Net_UnLock()
    //{
    //     isCorrectAnswer = false;
    //    //animation
    //    lineRenderer.colorGradient = wrongGradient;
    //    _holderOpened.SetActive(true);
    //    _holderClosed.SetActive(false);
    //}

    public void RemoveSocket(bool onEffect = false)
    {

        if (onEffect) foreach (var p in particles) p.Play();

        if (onSocketItem)
        {
            onSocketItem.RemoveSocket(onEffect);
            onSocketItem = null;
            HideEButton();
        }

        isCorrectAnswer = false;
        //animation
        lineRenderer.colorGradient = wrongGradient;
        _holderOpened.SetActive(true);
        _holderClosed.SetActive(false);
        //Parts_Net.CmdUnLock();
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
        
        
        //In Correct Effect
        lineRenderer.colorGradient = correctGradient;//test


    }

   
    private void WrongAnswer()
    {
        //RemoveSocket(true);
        Net_RemoveSocket(true);
    }
    #endregion


    #region Network
    public void Net_RemoveSocket(bool onEffect)
    {
        Parts_Net.CmdRemoveSocket(onEffect);
    }
    public void Net_InCorrectAnswer()
    {
        InCorrectAnswer();
    }
    public void Net_Lock()
    {
        col.enabled = false;
        col.enabled = true;
        _holderOpened.SetActive(false);
        _holderClosed.SetActive(true);
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
            //RemoveSocket();
            Parts_Net.CmdRemoveSocket(false);
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
