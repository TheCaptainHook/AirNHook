
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class Puzzle_1 : ButtonEntity
{
    [CustomHeader("Puzzle_1")]
    [SerializeField] Puzzle_1_Parts[] puzzle_1_Parts;

    [SerializeField] Transform partsContainer;
    [SerializeField] Transform itemContainer;

    private string[] puzzle_1_Items = new string[] {"Puzzle_1_Item (1)", "Puzzle_1_Item (2)", "Puzzle_1_Item (3)"};

    [ReadOnly]
    public string answer; //test

    //1. parts position, item position

    private Vector2[] partsPosition;
    private Vector2[] itemsPosition; //Fill in this field through the editor

    #region  Get,Set
    public override T GetData<T>()
    {
        if(typeof(T) == typeof(ButtonObjectStruct)){
            return (T)(object)new ButtonObjectStruct(id,GetTargetPositions(),transform.position,transform.localScale);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonObjectStruct))
            {
                 ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                 ButtonObjectData = buttonData;
                 FindTargetObject();

                 partsPosition = buttonData.partsPositions;
                 itemsPosition = buttonData.itemPositions;

                 //Setting parts and Item;
            }
                
        }catch(Exception ex){
                Debug.Log($"name : {gameObject.name},{ex}");
        }
    }
    #endregion


    private void Update() //test
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // GetItemAndAnswerArray();
            partsPosition = new Vector2[5]; //test
            Setting();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            CheckAnswer();
        }

    }

   private void Setting(){
    for(int i = 0; i< partsPosition.Length;i++){
        // puzzle_1_Parts[i].transform.position = partsPosition[i];

        int num = Random.Range(1, 4);
        GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[num-1]);
            
        obj.transform.position = Vector2.zero; // Set Position
        // obj.transform.position = itemsPosition[i];
            
        puzzle_1_Parts[i].SetAnswer(num);
        answer += num.ToString();
    }
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

        return num == puzzle_1_Parts.Length;
    }
    #endregion

    #region Util


    private Vector2[] GetPartsPosition()
    {
        Vector2[] pots = new Vector2[puzzle_1_Parts.Length];
        for (int i = 0; i < pots.Length; i++)
        {
            pots[i] = puzzle_1_Parts[i].transform.position;
        }
        return pots;
    }
  
    public int GetNumberOfParts(){
        int num =0;
        foreach(Transform tr in partsContainer){
            num++;
        }
        return num;
    }

    #endregion
}
