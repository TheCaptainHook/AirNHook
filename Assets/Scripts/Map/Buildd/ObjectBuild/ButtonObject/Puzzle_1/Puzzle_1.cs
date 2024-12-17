
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class Puzzle_1 : ButtonEntity
{
    [CustomHeader("Puzzle_1")]
    public List<Puzzle_1_Parts> puzzle_1_Parts;

    [ReadOnly]
    [SerializeField] Transform partsContainer;
    [ReadOnly]
    [SerializeField] Transform itemContainer;
    [ReadOnly]
    [SerializeField] GameObject partsPrefab;
    [ReadOnly]
    public string answer; //test

    private string[] puzzle_1_Items = new string[] { "Puzzle_1_Item (1)", "Puzzle_1_Item (2)", "Puzzle_1_Item (3)" };
    private Vector2[] partsPosition;
    private Vector2[] itemsPosition; //Fill in this field through the editor


    #region  Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonObjectStruct)) {
            return (T)(object)new ButtonObjectStruct(
                id,
                GetTargetPositions(),
                transform.position,
                transform.localScale,
                GetPartsPosition(),
                GetItemPosition()
                );
        }

        return default(T);
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

                Setting();
                //Setting parts and Item;
            }

        } catch (Exception ex) {
            Debug.Log($"name : {gameObject.name},{ex}");
        }
    }
    #endregion


    private void Update() //test
    {
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     // GetItemAndAnswerArray();
        //     partsPosition = new Vector2[3]; //test
        //     Setting();
        // }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (CheckAnswer())
            {
                Activation();
            }
            else
            {
                Boom();
            }
        }

    }

    private void Setting() {
        for (int i = 0; i < partsPosition.Length; i++) {
            int num = Random.Range(1, 4);

            #if UNITY_EDITOR
            Puzzle_1_Helper helper = GetComponent<Puzzle_1_Helper>();
            helper.Init();
            #endif

            GameObject obj;
            if(Application.isPlaying){  
                obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[num - 1]);
            }else{
                obj = helper.Add_Item();
            }
            
            obj.transform.position = itemsPosition[i];
            obj.transform.SetParent(itemContainer);

            CreateParts(partsPosition[i],num);

            // obj.transform.position = Vector2.zero; // test
            // puzzle_1_Parts[i].SetAnswer(num); //test

            answer += num.ToString();
        }
    }

    private void CreateParts(Vector2 pot,int answer){
        Puzzle_1_Parts obj = Instantiate(partsPrefab).GetComponent<Puzzle_1_Parts>();
        obj.transform.SetParent(partsContainer);
        puzzle_1_Parts.Add(obj);
        obj.transform.position = pot;
        obj.SetAnswer(answer);

    }


    protected override void Activation()
    {
        PrograssButtonActivatedObject(true);
        Debug.Log("Activation");
    }
    

    #region Answer

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

    #region Util


    private Vector2[] GetPartsPosition()
    {
        Vector2[] pots = new Vector2[puzzle_1_Parts.Count];
        for (int i = 0; i < pots.Length; i++)
        {
            pots[i] = puzzle_1_Parts[i].transform.position;
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
    #endregion
}
