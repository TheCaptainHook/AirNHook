using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Turret_Net : NetworkBehaviour
{
    private Turret Turret => GetComponent<Turret>();


    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem fireEffect;

 
 

    [Server]
    public void Fire()
    {
        ReloadAmmo();
        Rpc_FireEffect();
        Turret.ReadyToFire();

    }

    [ClientRpc]
    private void Rpc_FireEffect()
    {
        Turret.Net_Effect();
    }

    private void ReloadAmmo()
    {
        Projectile_Shell shell = Managers.Pooling.N_GetItme(typeof(Projectile_Shell).Name).GetComponent<Projectile_Shell>();
        Vector2 target = firePoint.TransformPoint(Vector2.zero);
        shell.Setting(target, firePoint.right);
        shell.gameObject.SetActive(true);
    }


    public override void OnStartClient()
    {
        if (isServer) return;
        base.OnStartClient();
        Turret.isClient = true;
    }
}
