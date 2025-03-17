using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseField_Object_Net : NetworkBehaviour
{
    [SerializeField] GameObject base_2_Field;
    [SerializeField] GameObject main_Field;
    [SerializeField] GameObject main_Light;

    [SerializeField] SpriteRenderer main_Field_Sprite;

    private Color color = new Color(71 / 255f, 239 / 255f, 1, 1);
    private Color nonCol = new Color(71 / 255f, 239 / 255f, 1, 0);

    private EraseField_Object Main => GetComponent<EraseField_Object>();
    private Collider2D Col => GetComponent<Collider2D>();


    public bool onActive;
    #region Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ButtonActivatedObjectStruct, Main.Check_Condition_RequirAmount());
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data, bool onActive)
    {
        if (onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;
        onSync = true;
        if (!onActive) Active(false);
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
    }
    #endregion


    [ClientRpc]
    public void Rpc_Active()
    {
        //Main.Net_Active();
        Active(true);
    }
    [ClientRpc]
    public void Rpc_Deactive()
    {
        Active(false);
    }

    private void Active(bool onOff)
    {
        SetEffect(onOff);
        onActive = onOff;
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(EffectCo(onOff));

        Col.enabled = onOff;
    }



    Coroutine effectCoroutine;
    float percent = 0;
    IEnumerator EffectCo(bool onOff)
    {
        while (percent < 1)
        {
            percent += Time.fixedDeltaTime;

            if (onOff) main_Field_Sprite.color = Color.Lerp(nonCol, color, percent);
            else main_Field_Sprite.color = Color.Lerp(color, nonCol, percent);

            yield return null;
        }
        percent = 0;

        if (onOff) main_Field_Sprite.color = color;
        else main_Field_Sprite.color = nonCol;
    }

    private void SetEffect(bool onOff)
    {
        base_2_Field.SetActive(onOff);
        //main_Field.SetActive(onOff);
        main_Light.SetActive(onOff);
    }
}
