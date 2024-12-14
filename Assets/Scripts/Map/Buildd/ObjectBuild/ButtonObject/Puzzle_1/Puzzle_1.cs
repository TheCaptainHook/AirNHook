 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle_1 : MonoBehaviour
{
    [SerializeField] Puzzle_1_Parts[] puzzle_1_Parts;
    private string[] puzzle_1_Items = new string[] {"Puzzle_1_Item (1)", "Puzzle_1_Item (2)", "Puzzle_1_Item (3)"};

    public int[] indexs;






   private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Suffle();

            for (int i = 0; i < 3; i++)
            {
                GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[i]);
                obj.transform.position = Vector2.zero;
                puzzle_1_Parts[i].SetAnswer(indexs[i]);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            CheckAnswer();
        }

    }

   



    #region Answer
    private void SetAnswer()
    {
        for (int i = 0; i < puzzle_1_Parts.Length; i++)
        {
            puzzle_1_Parts[i].SetAnswer(indexs[i]);
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

        return num == puzzle_1_Parts.Length;
    }
    #endregion


    #region Util

    private void GetAnswerArray()
    {
        for(int i = 0; i < puzzle_1_Parts.Length; i++)
        {
            int num = Random.Range(0, 3);
        }
    }
    private void Suffle()
    {
        for(int i = indexs.Length - 1; i >0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = indexs[i];
            indexs[i] = indexs[j];
            indexs[j] = temp;
        }
    }

    private Vector2[] GetPartsPosition()
    {
        Vector2[] pots = new Vector2[puzzle_1_Parts.Length];
        for (int i = 0; i < pots.Length; i++)
        {
            pots[i] = puzzle_1_Parts[i].transform.position;
        }
        return pots;
    }
    private void SetPartsPosition(Vector2[] pots)
    {
        for(int i = 0; i < pots.Length; i++)
        {
            puzzle_1_Parts[i].transform.position = pots[i];
        }
    }
  
    #endregion
}
