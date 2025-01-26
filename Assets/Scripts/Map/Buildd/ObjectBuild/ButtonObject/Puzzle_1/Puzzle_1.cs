
using UnityEngine;
using System;
using Random = UnityEngine.Random;
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
    public string answer; //test

    // private string[] puzzle_1_Items = new string[] { 
    //     "Puzzle_1_Item (1)", 
    //     "Puzzle_1_Item (2)", 
    //     "Puzzle_1_Item (3)",
    //     "Puzzle_1_Item (4)",
    //     "Puzzle_1_Item (5)",
    //     "Puzzle_1_Item (6)"
    // };
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
                GetTargetPositions(),
                GetPosition(),
                transform.localScale,
                GetPartsPosition(),
                GetItemPosition(),
                onHint,
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

    public override void SetData<T>(T data)
    {
        try {
            if (typeof(T) == typeof(ButtonObjectStruct))
            {
                ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                ButtonObjectData = buttonData;
                FindTargetObject();

                partsPosition = buttonData.partsPositions;
                itemsPosition = buttonData.itemPositions;
                if (buttonData.onHint)
                {
                    onHint = true;
                    hintPosition = buttonData.hintPosition;
                }
                else
                {
                    onHint = false;
                    hintPosition = default;
                }

                Setting();
                //Setting parts and Item;
            }

        } catch (Exception ex) {
            Debug.Log($"name : {gameObject.name},{ex}");
        }
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

    //private void Update() //test
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        Power();

    //    }

    //}

//     private void Setting() {
//         int previousNum = 0;
//         for (int i = 0; i < partsPosition.Length; i++) {
//             int num = Random.Range(1, 7);
//             while(previousNum == num) num = Random.Range(1, 7);
//             previousNum = num;
//             GameObject obj;
//             answer += num.ToString();

// #if UNITY_EDITOR
//             Puzzle_1_Helper helper = GetComponent<Puzzle_1_Helper>();
//             helper.Init();
//             if (Application.isPlaying)
//             {
//                 obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[num - 1]);
//             }
//             else
//             {
//                 obj = helper.Add_Item();
//                 if (onHint) 
//                 {
//                     hintScreen.gameObject.SetActive(true);
//                 }

//             }

// #else

//              obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[num - 1]);
//              //hint
//               if (onHint) 
//                {
//                     hintScreen.gameObject.SetActive(true);
//                      SetHint();
//                }
           
// #endif

//             obj.transform.position = itemsPosition[i];
//             obj.transform.SetParent(itemContainer);

//             CreateParts(partsPosition[i],num,i);

//             // obj.transform.position = Vector2.zero; // test
//             // puzzle_1_Parts[i].SetAnswer(num); //test

           
//         }

//         SetHint();
        

//     }


    //-------------------------------------------------------------Network 250126
    private void Editor_Setting(int index)
    {
        GameObject obj;
        Puzzle_1_Helper helper = GetComponent<Puzzle_1_Helper>();
            helper.Init();
            if (Application.isPlaying)
            {
                // obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[num - 1]);
                puzzle_net.Server_CreatePuzzle_Item(
                    index,itemsPosition[index],partsPosition[index]
                );
            }
            else
            {
                obj = helper.Add_Item();
                if (onHint) 
                {
                    hintScreen.gameObject.SetActive(true);
                }

                obj.transform.position = itemsPosition[index];
                obj.transform.SetParent(itemContainer);

                CreateParts(partsPosition[index],0,index);

            }
    }

     private void Setting() {
        for (int i = 0; i < partsPosition.Length; i++) {

#if UNITY_EDITOR
            Editor_Setting(i);

#else

         puzzle_net.Server_CreatePuzzle_Item(
                    i,itemsPosition[i],partsPosition[i]
                );
           
#endif
           
        }
        puzzle_net.Server_SetHintPosition(hintPosition);

    }
    //-------------------------------------------------------------Network 250126

    public void Net_SetHint(string answer,Vector2 hintPoition)
    {
        SetHint(answer,hintPoition);
    }
    private void SetHint(string answer,Vector2 hintPosition)
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


    private void CreateParts(Vector2 pot,int answer,int index){
        Puzzle_1_Parts obj = Instantiate(partsPrefab).GetComponent<Puzzle_1_Parts>();
        obj.transform.SetParent(partsContainer);
        puzzle_1_Parts.Add(obj);
        obj.transform.position = pot;
        obj.Settting(this,answer,index);

    }
    public void Net_CreateParts(Vector2 pot, int answer,int index)
    {
        CreateParts(pot,answer,index);
    }
    


    protected override void Activation()
    {
        PrograssButtonActivatedObject(true);
        Debug.Log("Activation");
    }

    #region Hint

    #endregion

    #region Answer
    private void Power()
    {
        if (CheckAnswer())
        {
            Activation();
            hintScreen.Correct();
        }
        else
        {
            Boom();
            Wrong();
            hintScreen.False();
        }
    }
    private bool CheckAnswer()
    {
        int num = 0;
        foreach (Puzzle_1_Parts parts in puzzle_1_Parts)
        {
            parts.CheckAnswer();
            if (parts.isCorrectAnswer) num++;
        }

        return num == puzzle_1_Parts.Count;
    }

    private void Boom()
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

    #region Charging // Network processing required
    public void Charging()
    {
        if(button.Charging())
        {
            Power();
        }
       
    }
    private void Wrong()
    {
        button.Wrong();
    }

    #region NetWork
    public void Net_Charging()
    {
        Puzzle_Net.CmdCharging();
    }
    public void Net_Wrong()
    {
        Wrong();
    }


    // public void Net_CreatePuzzleItem(int num)
    // {
    //     Managers.Stage.CmdBatchObject(puzzle_1_Items[num - 1]);
    // }

    #endregion


    #endregion

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
