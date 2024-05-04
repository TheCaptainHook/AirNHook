using Mirror;
using UnityEngine;

public class NetworkRigidbodySync : NetworkBehaviour
{
    private Rigidbody2D _rigidbd;
    
    // sync velocity
    [SyncVar] public Vector2 velocity = Vector2.zero;
    private readonly ClientSyncState _previousValue = new ClientSyncState();
    private readonly float _velocitySensitivity = 0.1f;

    private void Awake()
    {
        _rigidbd = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        
        UpdateVelocity();
        SendVelocity();
    }

    [Client]
    private void UpdateVelocity()
    {
        velocity = _rigidbd.velocity;
    }

    [Client]
    private void SendVelocity()
    {
        float now = Time.time;
        if (now < _previousValue.nextSyncTime)
            return;

        Vector2 currentVelocity = velocity;

        bool velocityChanged = ((_previousValue.velocity - currentVelocity).sqrMagnitude > _velocitySensitivity * _velocitySensitivity);
        //((previousValue.angularVelocity - currentAngularVelocity).sqrMagnitude > angularVelocitySensitivity * angularVelocitySensitivity);
        
        // if angularVelocity has changed it is likely that velocity has also changed so just sync both values
        // however if only velocity has changed just send velocity
        if (velocityChanged)
        {
            CmdSendVelocity(currentVelocity);
            _previousValue.velocity = currentVelocity;
        }
        
        // only update syncTime if either has changed
        if (velocityChanged)
            _previousValue.nextSyncTime = now + syncInterval;
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSendVelocity(Vector2 velocity)
    {
        this.velocity = velocity;
    }
    
    public class ClientSyncState
    {
        public float nextSyncTime;
        public Vector2 velocity;
    }
}
