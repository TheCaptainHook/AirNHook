using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Trigger_Dialogue : BuildObj
{
    public int _DialogueId;
    private bool OnExcuted;

    [SerializeField] UI_Dialogue dialogue;//TESTCODE

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player component) && !OnExcuted)
        {
            Debug.Log("Trigger");
            OnExcuted = true;
            //TESTCODE
            dialogue.SetData(_DialogueId);
            //TESTCODE

            //UI_Dialogue active
            //Managers.UI.GetUI<UI_Dialogue>().GetComponent<UI_Dialogue>().SetData(_DialogueId);
        }
    }

    public ObjectData GetDialogueData()
    {
        ObjectData data = new ObjectData(id,transform.position,transform.localScale,_DialogueId);
        return data;
    }

    public void SetDialogueData(ObjectData data)
    {
        ObjectData = data;
        transform.position = data.position;
        transform.localScale = data.scale;
        _DialogueId = data.dialogueId;
    }



    /// <summary>
    ///This method is used in the MapEditor_Editor Save process.
    /// </summary>
    public override void SetTileData()
    {
        Debug.Log("Trigger Obejcg Data Save");
        ObjectData = new ObjectData(id, transform.position, transform.localScale, _DialogueId);
    }

    public override void SetData(ObjectData data)
    {
        base.SetData(data);
        _DialogueId = data.dialogueId;
    }
}
