
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class Puzzle_1 : ButtonEntity
{
    [CustomHeader("Puzzle_1")]
    public List<Puzzle_1_Parts> puzzle_1_Parts;


    [SerializeField] Puzzle_1_Button button;
    [SerializeField] Puzzle_1_HintScreen hintScreen;
    [ReadOnly]
    [SerializeField] Transform partsContainer;
    [ReadOnly]
    [SerializeField] Transform itemContainer;
    [ReadOnly]
    [SerializeField] GameObject partsPrefab;
    [ReadOnly]
    public string answer; 


    private Vector2[] partsPosition;
    private Vector2[] itemsPosition; //Fill in this field through the editor
    public bool onHint;
    private Vector2 hintPosition;

    #region  Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonObjectStruct)) {
            return (T)(object)new ButtonObjectStruct(
                id,
                GetPosition(),
                transform.rotation,
                transform.localScale,
                GetTargetPositions(),
                GetLightPositions(),
                GetEncapsulationTiems(),
                
                onHint,
                GetPartsPosition(),
                GetItemPosition(),
                onHint ? hintScreen.transform.position : default 
                );
        }

        return default(T);
    }

    private Vector2 GetPosition()
    {
        return new Vector2(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y));
    }

    public override void SetData<T>(T data) //Server
    {
        // base.SetData(data);
        if (typeof(T) == typeof(ButtonObjectStruct))
        {
            ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
            ButtonObjectData = buttonData;

            FindTargetObject();
            if (buttonData.lightPositions.Count > 0) FindLightObject();
            if (buttonData.encapsulationItems.Count > 0) FindEncapsulationItem();
        }
            
        partsPosition = ButtonObjectData.partsPositions;
        itemsPosition = ButtonObjectData.itemPositions;
        if (ButtonObjectData.onHint)
        {
            onHint = true;
            hintPosition = ButtonObjectData.hintPosition;
        }
        else
        {
            onHint = false;
            hintPosition = default; 
        }

        Setting();
       
    }
    #endregion


    Puzzle_1_Net puzzle_net;
    Puzzle_1_Net Puzzle_Net
    {
        get
        {
            if (puzzle_net == null) puzzle_net = GetComponent<Puzzle_1_Net>();
            return puzzle_net;
        }
    }
    PlayerInput input;

    private void Start()
    {
        input = Managers.Game.playerInput;
    }

    private void Update()
    {
        
        if (Puzzle_Net.onActive && input.playerActions.Action.ReadValue<float>() > 0f)
        {
            if (!Managers.Game.Player.TryGetComponent(out AirSM air)) return;
            
            if (input.playerActions.Action.ReadValue<float>() > 0f)
            {
                Puzzle_Net.CmdCharging();
            }
            else
            {
                Puzzle_Net.Cmd_SoundStop();
            }

        }

    }


    //-------------------------------------------------------------Network 250126
#if UNITY_EDITOR
    Puzzle_1_Helper Helper => GetComponent<Puzzle_1_Helper>();
    private void Editor_Setting(int index)
    {
        GameObject obj;

        //Item
        obj = Helper.Add_Item();
        obj.transform.position = itemsPosition[index];
        obj.transform.SetParent(itemContainer);
        //Item

        //Part
        CreateParts(partsPosition[index], 0, index);
        //Part

    }
    private void Editor_Create_DummyItem(int index)
    {
        GameObject obj;
        obj = Helper.Add_Item();
        obj.transform.position = itemsPosition[index];
        obj.transform.SetParent(itemContainer);
    }
#endif
    //-------------------------------------------------------------------------------250307 Refactoring
    public void Setting() //Server
    {
#if UNITY_EDITOR
        Helper.Init();
#endif
        for (int i = 0; i < partsPosition.Length; i++)
        {
            if (Application.isPlaying)
            {
                Puzzle_Net.Server_CreatePuzzle_Item(i, itemsPosition[i], partsPosition[i]);
            }
#if UNITY_EDITOR
            else
            {
                Editor_Setting(i);

            }
#endif
        }

        if (Application.isPlaying)
        {
            //Create Dummy Item 
            if (itemsPosition.Length > partsPosition.Length)
            {
                for (int i = partsPosition.Length; i < itemsPosition.Length; i++)
                {
                    Puzzle_Net.Server_Create_DummyItem(itemsPosition[i]);
                }
            }
            //Create Dummy Item 
            Puzzle_Net.Server_SetHintSetting(); // Rpc, Sync Start
        }
        else
        {
            SetHint();

#if UNITY_EDITOR
            int index = itemsPosition.Length - partsPosition.Length;
            if (index <= 0) return;

            for (int i = partsPosition.Length; i < itemsPosition.Length; i++)
            {
                Editor_Create_DummyItem(i);
            }

#endif
            //Create Dummy Item 
        }

    }

    #region  Clean
    public override void Clean() //RPC
    {
        Puzzle_Net.Clean();
    }
 #endregion


    public void SetHint(string answer = "ANSWER")
    {
        if (onHint)
        {
            hintScreen.gameObject.SetActive(true);
            hintScreen.transform.position = hintPosition;
            hintScreen.SetHint(answer);
        }
        else
        {
            hintScreen.gameObject.SetActive(false);
        }
    }


    #region Network
    public void SetPart(Puzzle_1_Parts part)
    {
        if (puzzle_1_Parts == null) puzzle_1_Parts = new();
        if (puzzle_1_Parts.Contains(part)) return;
        puzzle_1_Parts.Add(part);
    }
    #endregion


    private void CreateParts(Vector2 pot,int answer,int index){
        Puzzle_1_Parts obj = Instantiate(partsPrefab).GetComponent<Puzzle_1_Parts>();
        obj.transform.SetParent(partsContainer);
        puzzle_1_Parts.Add(obj);
        obj.transform.position = pot;
        obj.Settting(this,answer,index);

    }



    public override void Activation()
    {
        PrograssButtonActivatedObject(true);
    }
    public void Net_Activation()
    {
        Activation();
    }

    #region Hint
    public (bool isHint,Vector2 position) GetHintData()
    {
        return (onHint, hintPosition);
    }
    #endregion

    #region Answer
  
    public bool CheckAnswer() //Server
    {
        int num = 0;
        foreach (Puzzle_1_Parts parts in puzzle_1_Parts)
        {

            if (parts.CheckAnswer()) num++;
        }


        Debug.Log($"{num}, count :{puzzle_1_Parts.Count}");
        return num == puzzle_1_Parts.Count;
    }

    public void Boom()
    {
        HashSet<Collider2D> col = new();
        foreach (Puzzle_1_Parts parts in puzzle_1_Parts)
        {
            parts.Boom(ref col);
        }

        if(col.Count> 0)
        {
            foreach(var c in col)
            {
               if(c.TryGetComponent(out PlayerSM component))
                {
                    component.TakeDamage(DamageType.Boom);
                }
            }
        }
    }
    #endregion

  
    public void Net_HintScreen_Correct(){
        hintScreen.Correct();
    }
    public void Net_HintScreen_False(){
        hintScreen.False();
    }
  

    #region Util


    private Vector2[] GetPartsPosition()
    {
        Vector2[] pots = new Vector2[puzzle_1_Parts.Count];
        for (int i = 0; i < pots.Length; i++)
        {
            pots[i] = puzzle_1_Parts[i].GetPosition();
        }
        return pots;
    }

    private Vector2[] GetItemPosition()
    {
        Vector2[] pots = new Vector2[itemContainer.childCount];
        for (int i = 0; i < pots.Length; i++)
        {
            pots[i] = itemContainer.GetChild(i).transform.position;
        }
        return pots;
    }
    #endregion

    #region Editor
    public void AddParts(Puzzle_1_Parts parts)
    {
        puzzle_1_Parts.Add(parts);
    }
    public void RemoveParts()
    {
        puzzle_1_Parts.RemoveAt(puzzle_1_Parts.Count - 1);
    }
    public int GetPartsCount()
    {
        return puzzle_1_Parts.Count;
    }
    public bool GetOnHint()
    {
        return onHint;
    }
    public void SetOnHint(bool onhint)
    {
        onHint = onhint;
    }
    public void RefrashPartsList()
    {
        if (puzzle_1_Parts == null) return;
        for (int i = puzzle_1_Parts.Count - 1; i >= 0; i--)
        {
            var obj = puzzle_1_Parts[i];

            if (obj == null)
            {
                puzzle_1_Parts.RemoveAt(i);
            }
        }
    }

    public void SetHintScreenText(int num)
    {
        StringBuilder sb = new();
        for(int i = 0; i < num; i++)
        {
            sb.Append(i % 2 == 0 ? 'X' : '0');
        }
        hintScreen.SetText(sb.ToString());
    }
    public void SetScreenActive(bool active)
    {
        hintScreen.gameObject.SetActive(active);
    }
    public Puzzle_1_HintScreen GetHintScreen()
    {
        return hintScreen;
    }
    #endregion
}
