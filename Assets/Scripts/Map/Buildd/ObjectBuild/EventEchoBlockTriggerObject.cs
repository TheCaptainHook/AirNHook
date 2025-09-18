
using UnityEngine;

public class EventEchoBlockTriggerObject : BuildObj
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Managers.AcManager.SetInterrupted();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        var col = Col as BoxCollider2D;
        Gizmos.DrawWireCube((Vector2)transform.position + col.offset, col.size);
    }
#endif


    #region  Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ObjectData))
        {
            var col = Col as BoxCollider2D;
            return (T)(object)new ObjectData(id, transform.position, col.offset, col.size);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        if (typeof(T) == typeof(ObjectData))
        {
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);
        }
    }
    public override void SetData(ObjectData data)
    {
        base.SetData(data);
        var col = Col as BoxCollider2D;
        col.offset = data.scale;
        col.size = data.size;
    }
    #endregion

}
