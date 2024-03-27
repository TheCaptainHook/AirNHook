using Mirror;
using UnityEngine;

public class GrabbableObject : NetworkBehaviour, IInteractable
{
    public Transform player;
    private Rigidbody2D _rigidbody2D;
    public ObjectTypeEnum objectType = ObjectTypeEnum.Grab;
    [SyncVar]
    public bool isGrabbed;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isGrabbed && player != null)
        {
            transform.position = player.position;
        }
    }

    public void Interaction(Transform accessor)
    {
        if (player != null && !ReferenceEquals(player, accessor))
            return;

        if (isGrabbed)
        {
            Release();
        }
        else
        {
            player = accessor;
            Grab();
        }
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectType;
    }

    private void Grab()
    {
        isGrabbed = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.velocity = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;
    }

    private void Release()
    {
        isGrabbed = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        player = null;
    }
}
