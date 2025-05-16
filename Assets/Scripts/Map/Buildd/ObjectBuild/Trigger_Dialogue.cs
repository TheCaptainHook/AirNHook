
using UnityEngine;


public class Trigger_Dialogue : BuildObj
{
    [CustomHeader("Trigger Dialogue")]
    
    public int _DialogueId;
    [ReadOnly]
    public bool OnExcuted;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerSM component) && !OnExcuted)
        {
            OnExcuted = true;
   
            //Save Data Modify
            MapSaveData _MapSaveData = Managers.Data.saveData.dic[MapEditor.Instance.CurMap.mapID];
            _MapSaveData.ModifyDialogueData(_DialogueId);

            UI_Dialogue _UI = Managers.UI.ShowUI<UI_Dialogue>().GetComponent<UI_Dialogue>();
            
            _UI.StartDialogue(_DialogueId);
           
        }
    }

    public override T GetData<T>()
    {
        if(typeof(T)==typeof(DialogueData)){
            return (T)(object)new DialogueData(id,_DialogueId, false,transform.position, transform.rotation, transform.localScale);
        }

        return default(T);
    }


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


}
