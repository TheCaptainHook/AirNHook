using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Trigger_Dialogue : BuildObj
{
    [CustomHeader("Trigger Dialogue")]
    
    public int _DialogueId;
    [ReadOnly]
    public bool OnExcuted;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player component) && !OnExcuted)
        {
            OnExcuted = true;
   
            //Save Data Modify
            MapSaveData _MapSaveData = Managers.Data.saveData.dic[MapEditor.Instance.CurMap.mapID];
            _MapSaveData.ModifyDialogueData(_DialogueId);

            //UI_Dialogue active
            UI_Dialogue _UI = Managers.UI.ShowUI<UI_Dialogue>().GetComponent<UI_Dialogue>();
            _UI.SetData(_DialogueId);
           
        }
    }

    // public DialogueData GetDialogueData()
    // {
    //     DialogueData data = new DialogueData(id,_DialogueId, false,transform.position, transform.rotation, transform.localScale);
    //     return data;
    // }
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(DialogueData)){
            return (T)(object)new DialogueData(id,_DialogueId, false,transform.position, transform.rotation, transform.localScale);
        }

        return default(T);
    }

    // public void SetDialogueData(DialogueData data)
    // {
    //     ObjectData = new ObjectData(data.id, data.position, data.scale);
    //     transform.position = data.position;
    //     transform.localScale = data.scale;
    //     _DialogueId = data.dialogueId;
    //     OnExcuted = data.excuted;

    // }

    public override void SetData<T>(T data)
    {
        if(typeof(T)== typeof(DialogueData)){
            DialogueData ddate = (DialogueData)(object)data;
            ObjectData = new ObjectData(ddate.id, ddate.position, ddate.scale);
            transform.position = ddate.position;
            transform.localScale = ddate.scale;
            _DialogueId = ddate.dialogueId;
            OnExcuted = ddate.excuted;
        }
    }

    /// <summary>
    ///This method is used in the MapEditor_Editor Save process.
    /// </summary>
    //public override void SetTileData()
    //{
    //    Debug.Log("Trigger Obejcg Data Save");
    //    ObjectData = new ObjectData(id, transform.position, transform.localScale, _DialogueId);
    //}

    //public override void SetData(ObjectData data)
    //{
    //    base.SetData(data);
    //    _DialogueId = data.dialogueId;
    //}
}
