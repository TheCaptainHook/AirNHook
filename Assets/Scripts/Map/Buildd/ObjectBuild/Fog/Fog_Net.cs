
using UnityEngine;
using Mirror;
using System.Collections;

public class Fog_Net : NetworkBehaviour
{
    [SyncVar] public Vector2 size;
    

    private BoxCollider2D Collider => GetComponent<BoxCollider2D>();
    private ParticleSystem MainPartice => GetComponent<ParticleSystem>();



    [Server]
    public void Server_SetSize(Vector2 size)
    {
        this.size = size;
        //Rpc_SetFogSize();
    }

    //[ClientRpc]
    //private void Rpc_SetFogSize()
    //{
    //    SetParticleSetting();
    //}


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

    IEnumerator WaitForSync()
    {
        yield return new WaitUntil(() => size != Vector2.zero);
        SetParticleSetting();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(WaitForSync());
    }

}