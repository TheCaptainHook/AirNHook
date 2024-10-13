
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UI_SaveAndLoad : UI_Base
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Animator animator;

    private Stack<SaveAndLoadTask> stack = new();
    private bool onPrograss;

    public override void OnEnable()
    {
        text.text = "";
        animator.enabled = true;
    }


    public void SaveData(Task task){

        stack.Push(new SaveAndLoadTask(Managers.Data.language.GetSentence(2014),true,task));
        SaveAndLoad();
    }
    public void LoadData(Task task){
        stack.Push(new SaveAndLoadTask(Managers.Data.language.GetSentence(2013),false,task));
        SaveAndLoad();
    }
    public void SaveAndLoad(){
        if(!onPrograss){
            StartCoroutine(SaveAndLoadCo());
        }
    }

    IEnumerator SaveAndLoadCo(){
        onPrograss = true;
        while(stack.Count > 0){
            SaveAndLoadTask task = stack.Pop();
            text.text = task.text;
            yield return new WaitUntil(()=>task.task.IsCompleted);
            Complete(task);
            yield return new WaitForSeconds(2f);
        }
        onPrograss = false;
        CloseUI();
       
    }
    


    public void Complete(SaveAndLoadTask task){
        if(task.loadAndSave){
            text.text= $" {Managers.Data.language.GetSentence(2016)}";
        }else{
            text.text= $" {Managers.Data.language.GetSentence(2015)}";
        }
    }
/// <summary>
/// loadAndSave : true -> Save, false -> Load
/// </summary>
    public struct SaveAndLoadTask{
        public string text;
        public bool loadAndSave; 
        public Task task;
        public SaveAndLoadTask(string text,bool loadAndSave, Task task){
            this.text = text;
            this.loadAndSave = loadAndSave;
            this.task = task;
        }
    }
}
