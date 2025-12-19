using System.Collections;
using Mirror;
using UnityEngine;

public class HomingTurret_Net : ActivatableObject_Net_Entity
{
    [SerializeField] private bool _onReady;
    [SerializeField] GameObject _target;

    [Header("Layer")]
    [SerializeField] LayerMask _detactLayer;
    [SerializeField] LayerMask _obstacleLayer;
    [Header("Parts")]
    [SerializeField] Transform _turretTopTR;
    [Header("Fire Point")]
    [SerializeField] Transform[] _firePoints;

    //TESDT
    [SerializeField] private GameObject _missilePrefab;

    [Header("Launch")]
    private bool _onLunch = false;
    private int _maxLaunchCount = 3;
    [SerializeField] float _maxLaunchDelay = 3;
    private WaitForSeconds _maxLaunchDelayWFS;


    Animator _animator;
    Animator Animator { get { return _animator ??= GetComponent<Animator>(); } }

    protected override void Active() //RPC
    {
        _onReady = true;
        
    }
    protected override void Deactive() //RPC
    {
        _onReady = false;
    }


    [ServerCallback]
    void FixedUpdate()
    {
        if(!_onReady) return;
        if(_onLunch) return;

        UpdateTargetDetection();
    }

    private GameObject _s_target;
    #region  Detect
    [Server]
    private void UpdateTargetDetection()
    {
        _s_target = DetectTargetInRange();
        if(_s_target != null)
        {
            if(ObstacleCheck(_target))
            {
                _s_target = null;
                return;
            }

            //================Rotate
            //================Rotate
            //================Launch Missile
            Server_LaunchMissile(_s_target);
            //================Launch Missile
        }
       
    }
    private IEnumerator TimerCo()
    {
        yield return _maxLaunchDelayWFS ??= new WaitForSeconds(_maxLaunchDelay);

    }
    [SerializeField] float _detectRadius = 10f;
    private Collider2D[] _detectBuffer = new Collider2D[16];
    private GameObject DetectTargetInRange()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, _detectRadius, _detectBuffer,_detactLayer);
        if(hitCount == 0) return null;

        float closestSqrDist = float.MaxValue;
        GameObject closest = null;

        for(int i=0; i<hitCount; i++)
        {
            Collider2D col = _detectBuffer[i];
            if(col == null) continue;
            if(col.gameObject == gameObject) continue;
            if(!col.TryGetComponent(out NetworkIdentity _)) continue;

            
            float sqrDist = ((Vector2)col.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closest = col.gameObject;
            }
       
        }

        return closest;
    }

    private bool ObstacleCheck(GameObject target)
    {
        Vector2 dir = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
        float dist = Vector2.Distance(target.transform.position, transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, _obstacleLayer);
        if(hit.collider != null)
        {
            return true;
        }

        return false;
    }


#endregion

#region Rotate
[Server]
private void Server_RotateTurret(GameObject obj)
{
     if(obj.TryGetComponent(out NetworkIdentity identity)) Rpc_RotateTurret(identity.netId);
        
}
[ClientRpc]
private void Rpc_RotateTurret(uint netId)
{
    GameObject obj = NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity.gameObject : null;
    if(obj == null) return;

    Vector2 dir = ((Vector2)obj.transform.position - (Vector2)transform.position).normalized;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    // _turretTopTR.rotation = Quaternion.Euler(0f, 0f, angle);
}

#endregion

#region Launch Missile
[SerializeField] private float _launchDelay = 0.5f;
private WaitForSeconds _launchDelayWFS;
[Server]
private void Server_LaunchMissile(GameObject obj)
{
    _onLunch = true;
    if(obj.TryGetComponent(out NetworkIdentity identity)) Rpc_LaunchMissile(identity.netId);        
}
[ClientRpc]
private void Rpc_LaunchMissile(uint netId)
{
    GameObject obj = NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity.gameObject : null;
    if(obj == null) return;
    
    _target = obj;
    StartCoroutine(LaunchMissileCoroutine());
}

private IEnumerator LaunchMissileCoroutine()
{
    for(int i = 0; i< _maxLaunchCount; i++)
    {
        LaunchMissile(_firePoints[i].position, _target);
        yield return _launchDelayWFS ??= new WaitForSeconds(_launchDelay);        
    }
    //========Reload
    //========Reload
    _onLunch = false;
}
private void LaunchMissile(Vector2 position, GameObject target)
{
    // var obj = Instantiate(_missilePrefab, position, Quaternion.identity);
    var obj = Managers.Pooling.D_GetItem(_missilePrefab);
    obj.transform.position = position;
    obj.transform.rotation = _turretTopTR.rotation;

    obj.SetActive(true);
    Animator.SetTrigger("Fire");

    if(obj.TryGetComponent(out HomingMissile missile))
    {
        missile.SetTarget(target.transform);
    }
}

#endregion



}
