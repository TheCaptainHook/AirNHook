
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Puzzle_1_Parts : MonoBehaviour,IInteractable
{

    private UI_Base _E_Btn;
    private bool is_E_BtnEnabled;
    private bool _isMounted;
    private InteractableObject_Puzzle_1_Item _puzzleItem;
    private Vector2 _topOfObj = new Vector2(0, 1f);
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
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Mount;


    private Puzzle_1_Parts_Net Net;

    private void Awake(){
        Net = GetComponent<Puzzle_1_Parts_Net>();
        col = GetComponent<Collider2D>();

        pathFinder = GetComponent<PathFinder>();
        lineRenderer.colorGradient = wrongGradient;
    }

    #region ------------------------------------------------------Network Field

    public bool OnCorrect => Net.isCorrectAnswer;
    #endregion

    #region Insert,Remove Effect
    //public void InsertSocket(GameObject item)
    //{
    //    Net.Cmd_SetSocketItem(item);
    //}


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
    public bool CheckAnswer() //Server
    {
        if (!onSocket)
        {
            WrongAnswer();
            return false;
        }

        if (insert_Item.socketNumber == puzzleAnswer)
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

    private void InCorrectAnswer() //Server
    {
        Net.Cmd_SetCorrect(true);
    }

    public void Net_SetCorrectEffect()
    {
        lineRenderer.colorGradient = correctGradient;
    }
    private void WrongAnswer()
    {
        Net.Cmd_RemoveEffect();
        Net.Cmd_DisConnect();
    }
    #endregion


    public bool onSocket;

    //private bool OnSocket => (Net.item) ? true : false;

    //private void OnTriggerEnter2D(Collider2D collider){
    //    if (Net.isCorrectAnswer) return;
    //
    //    if (collider.TryGetComponent(out HookSM component))
    //    {
    //        Transform grabItem = component.GetGrabbedItem();
    //        if (grabItem != null)
    //        {
    //            if (grabItem.TryGetComponent(out Puzzle_1_Item item))
    //            {
    //
    //                if (component.TryGetComponent(out NetworkIdentity identity))
    //                {
    //                    if (identity.isLocalPlayer)
    //                    {
    //                        ShowE();
    //                        item.ContectParts(this);
    //                        //In Part
    //                    }
    //                }
    //
    //            }
    //        }
    //        else
    //        {
    //            if(onSocket)
    //            {
    //                if (component.TryGetComponent(out NetworkIdentity identity))
    //                {
    //                    if(identity.isLocalPlayer)
    //                    ShowE();
    //                }
    //            }
    //        }
    //    }
    //}
    //
    //
    //private void OnTriggerExit2D(Collider2D collider){
    //    if (onSocket || Net.isCorrectAnswer) return;
    //
    //    if (collider.TryGetComponent(out HookSM component)){
    //        Transform grabItem = component.GetGrabbedItem();
    //        if(grabItem != null){
    //            if (grabItem.TryGetComponent(out Puzzle_1_Item item))
    //            {
    //                if (component.TryGetComponent(out NetworkIdentity identity))
    //                {
    //                    if (identity.isLocalPlayer)
    //                    {
    //                        HideE();
    //                        item.ContectParts(null);
    //                    }
    //
    //                }
    //            }
    //        }
    //
    //        //Net.Cmd_ShowE(component.gameObject, false);
    //    }
    //    
    //}

    public Puzzle_1_Item insert_Item;
    public void Connect(Puzzle_1_Item item)
    {
        if(onSocket)
        {
            DisConnect();
        }

        Debug.Log("Connect Item[Parts]");
        //HideE();

        var col = item.TryGetComponent(out Collider2D collider) ? collider : null; 
        if(col != null) col.enabled = false;
        var rb = item.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if(rb != null)
        {
            rb.simulated = false;
            rb.velocity = Vector3.zero;
        }
      
        insert_Item = item;
        if (item.TryGetComponent(out ParentConstraint parentConstraint))
        {
            if (parentConstraint.sourceCount > 0)
            {
                parentConstraint.RemoveSource(0);
            }
            SetParentConstraint(parentConstraint, transform);
        }
        InsertAnimation(true);
        onSocket = true;

        col.enabled = false;
        col.enabled = true;
    }


    public void DisConnect()
    {
        Debug.Log("DisConnect Item[Parts]");
        onSocket = false;
        if (insert_Item.TryGetComponent(out ParentConstraint component))
        {
            if (component.sourceCount > 0)
            {
                component.RemoveSource(0);
            }

        }
        InsertAnimation(false);

        var col = insert_Item.TryGetComponent(out Collider2D collider) ? collider : null;
        if (col != null) col.enabled = true;
        var rb = insert_Item.TryGetComponent(out Rigidbody2D rigidbody) ? rigidbody : null;
        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = Vector3.zero;
        }

        insert_Item.RemoveSocketEffect();

        insert_Item.parts = null;
        insert_Item = null;
   
    }




    private void SetParentConstraint(ParentConstraint constraint, Transform parent)
    {
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parent,
            weight = 1
        };
        constraint.AddSource(source);

        constraint.translationAtRest = transform.localPosition;
        constraint.translationOffsets = new Vector3[constraint.sourceCount];
        constraint.constraintActive = true;

        constraint.locked = true;
    }





    #region UI

    public void ShowEButton()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (Vector3)_topOfObj;
    }

    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }



    public void ShowE()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    public void HideE()
    {
        if(_E_Btn != null) Managers.UI.HideUI<UI_ShowEButton>();
        _E_Btn = null;
        //Managers.UI.HideUI<UI_ShowEButton>();
    }


    #endregion

    #region Interaction

    //private bool IsCorrectAnswer => Net.isCorrectAnswer;
    //public void Interaction(Transform accessor = null)
    //{
    //    if ( OnSocket && !IsCorrectAnswer)
    //    {

    //        Net.Cmd_SetSocketItem(null);
    //    }
    //}

    public void Interaction(Transform accessor = null)
    {
        //if (onSocket && !Net.isCorrectAnswer)
        //{
        //    Net.Cmd_DisConnect();
        //}

        if (_isMounted)
        {
            _isMounted = false;
            Net.Cmd_DisConnect();
            _puzzleItem = null;
        }
        else
        {
            if (!accessor.TryGetComponent<InteractableObject_Puzzle_1_Item>(out var puzzleItem)) return;

            _isMounted = true;
            _puzzleItem = puzzleItem;
            _puzzleItem.Cmd_ConnectParts(GetComponent<NetworkIdentity>().netId);
        }
    }

    public bool CanInteract()
    {
        if (_isMounted)
            return true;

        if (Managers.Game.Player.TryGetComponent<HookSM>(out var hook) && hook.GetGrabbedItem() != null && hook.GetGrabbedItem().TryGetComponent<InteractableObject_Puzzle_1_Item>(out var puzzle))
            return true;

        return false;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

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
