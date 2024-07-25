using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Trigger_Dialogue : BuildObj
{
    public int _DialogueId;
    private bool OnExcuted;


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

    public DialogueData GetDialogueData()
    {
        DialogueData data = new DialogueData(id,_DialogueId, false,transform.position, transform.rotation, transform.localScale);
        return data;
    }

    public void SetDialogueData(DialogueData data)
    {
        ObjectData = new ObjectData(data.id, data.position, data.scale);
        transform.position = data.position;
        transform.localScale = data.scale;
        _DialogueId = data.dialogueId;
        OnExcuted = data.excuted;
        
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
