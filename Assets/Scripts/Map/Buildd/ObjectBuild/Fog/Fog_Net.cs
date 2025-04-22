
using UnityEngine;
using Mirror;
using System.Collections;

public class Fog_Net : NetworkBehaviour
{
   public Vector2 size;
    

    private BoxCollider2D Collider => GetComponent<BoxCollider2D>();
    private ParticleSystem MainPartice => GetComponent<ParticleSystem>();


    #region Init
    public bool onSync;
    private Fog Fog => GetComponent<Fog>();


    [Server]
    public void Server_InitSync()
    {
        var data = Fog.ObjectData;
        Rpc_InitSync(data);
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        if (onSync) return;
        this.size = data.size;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        SetParticleSetting();
        onSync = true;
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



    #region ------------------------------------------Inner Camer Effect
    [TargetRpc]
    private void TRpc_InnerFogEffect(NetworkConnection target,bool inOut)
    {
        Camera.main.GetComponent<PlayerCameraView>()._CameraGlobalVolumeController.InnerFog(inOut);
    }

    [Command]
    public void Cmd_InnerFog(GameObject obj,bool inOut)
    {
        if(obj.TryGetComponent(out NetworkIdentity identity))
        {
            var conn = identity.connectionToClient;
            TRpc_InnerFogEffect(conn,inOut);
        }
    }
    #endregion  

    #region ------------------------------------------Server_Util

     public void SetParticleSetting(){
        SetParticleShapeScale();
        SetParticleEmissionRate();
        MainPartice.Play();
    }
    private void SetParticleEmissionRate()
    {
        float rate = size.x * size.y * 0.5f;
        var emission = MainPartice.emission;
        emission.rateOverTime = rate;
      
    } 
    private void SetParticleShapeScale()
    {
        Collider.size = size;
        var shape = MainPartice.shape;
        shape.scale = size;
    }

    #endregion

    //IEnumerator WaitForSync()
    //{
    //    yield return new WaitUntil(() => size != Vector2.zero);
    //    SetParticleSetting();
    //}



}