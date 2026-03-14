
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_1_Parts : BuildObj,IInteractable
{

    private UI_Base _E_Btn;

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
    // private Collider2D col;
    #endregion
    [Header("Interactable")]
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Mount;


    private Puzzle_1_Parts_Net net;
    private Puzzle_1_Parts_Net Net { get { net ??= GetComponent<Puzzle_1_Parts_Net>();  return net; } }
    private void Awake()
    {
        // col = GetComponent<Collider2D>();
        pathFinder = GetComponent<PathFinder>();
        lineRenderer.colorGradient = wrongGradient;
    }

    #region ------------------------------------------------------Network Field

    public bool OnCorrect => Net.isCorrectAnswer;
    #endregion

    #region Insert,Remove Effect

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


    public void Net_RemovEffect()
    {
        foreach (var p in particles) p.Play();
    }

    #endregion

    #region  Clean    
    // public void Clean()
    // {
    //     if (Net.onSocket) Net.DisConnect(false);

    //     numbering[index].SetActive(false);
    //     lineRenderer.positionCount = 0;

    //     puzzleAnswer = 0;
    //     this.index = 0;
    //     this.puzzle_1 = null;

    //     col.enabled = true;

    // }

    public override void Clean()
    {
        StopAllCoroutines();

         if (Net.onSocket) Net.DisConnect(false);

        numbering[index].SetActive(false);
        lineRenderer.positionCount = 0;

        puzzleAnswer = 0;
        this.index = 0;
        this.puzzle_1 = null;

        Col.enabled = true;
    }
    #endregion

    #region Answer
    public void Settting(Puzzle_1 puzzle_1, int answer, int index)
    {
        puzzleAnswer = answer;
        this.index = index;
        this.puzzle_1 = puzzle_1;
        if (Application.isPlaying)
            DrawPath(transform, puzzle_1.transform);
        numbering[index].SetActive(true);
        //
        lineRenderer.colorGradient = wrongGradient;

    }
   
    public bool CheckAnswer() //Server
    {
        if (!Net.onSocket)
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

    // public void Boom(ref HashSet<Collider2D> col)
    // {
    //     if (OnCorrect) return;
    //     int playerLayerMask = 1 << LayerMask.NameToLayer("Player");

    //     //effect

    //     Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, boomArea,playerLayerMask);
    //     foreach (Collider2D c in cols)
    //     {
    //         col.Add(c);
    //     }
    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, boomArea);
    }

    private void InCorrectAnswer() //Server
    {
        Net.Cmd_SetCorrect(true);
    }

    public void Net_SetCorrectEffect() //Rpc
    {
        lineRenderer.colorGradient = correctGradient;
    }
    private void WrongAnswer()
    {
        // Net.Cmd_RemoveEffect();
        Net.Cmd_DisConnect(true);
    }
    #endregion

    private float condition_InsertVelocityValue = 15;
    private bool ChackVelocity(Puzzle_1_Item item)
    {
        Debug.Log(item._rb.velocity.magnitude);
        return item._rb.velocity.magnitude >= condition_InsertVelocityValue;
    }

    public Puzzle_1_Item insert_Item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (!NetworkServer.active) return;
        if (collision != null)
        {
            if (collision.TryGetComponent(out Puzzle_1_Item item))
            {
                var velocity = item._rb.velocity.magnitude;
                if (velocity >= condition_InsertVelocityValue)
                {
                    if (insert_Item == null)
                    {
                        insert_Item = item;
                        Interaction(item.transform);
                    }
                }
            }
        }
    }
    #region InHale Insert Item

    #endregion
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




    #endregion

    #region Interaction

    public void Interaction(Transform accessor = null)
    {
        if(accessor != null && !accessor.TryGetComponent(out AirSM air))
        {
            if (Net.onSocket)
            {
                Net.Cmd_DisConnect(false);
            }

            if (accessor.TryGetComponent(out NetworkIdentity identity))
                Net.Cmd_Connection(identity.netId);
        }
        else
        {
            if (Net.onSocket)
            {
                Net.Cmd_DisConnect(false);
            }
        }
      
    }

    public bool CanInteract()
    {
        if (Hook_IsInteractionValid()) return true;
        if(Air_IsInteractionValid()) return true;

        return false;
    }
    private bool Hook_IsInteractionValid()
    {
        var hook = Managers.Game.Player.TryGetComponent(out HookSM component) ? component : null;
        if (hook == null) return false;

        if (hook.GetGrabbedItem() == null) 
        {
            if (Net.onSocket) return true;
            else return false;
        }
      
        if (hook.GetGrabbedItem().TryGetComponent<InteractableObject_Puzzle_1_Item>(out var puzzleItem))
            return true;
        else
            return false;

    }
    private bool Air_IsInteractionValid()
    {
        var air = Managers.Game.Player.TryGetComponent(out AirSM component) ? component : null;
        if (air == null) return false;

        if (Net.onSocket) return true;
        else return false;
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
