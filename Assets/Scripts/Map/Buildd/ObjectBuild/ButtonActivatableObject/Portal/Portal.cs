using Mirror;
using UnityEngine;

public class Portal : ActivatableObjectEntity
{
    [CustomHeader("Portal")]
    // public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    // public Vector2 btnOffset;
    public Portal targetPortal;
    [SerializeField] LayerMask layer;
    [Space(20)]
    [ReadOnly]
    public Vector2 targetPosition;




    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] GameObject _TpEffect;


    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct)) {
            return (T)(object)new ButtonActivatableObjectStruct(id, transform.position, transform.rotation, transform.localScale,activeRequirAmount ,indicatorStruct, targetPortal.transform.position);
        }

        return default(T);
    }

    protected override void AdditionalInspectorConfig()
    {
        targetPosition = ButtonActivatedObjectStruct.talPot;
    }


    public override void Clean()
    {
        Net.onSync = false;
    }
    #region Editor


#if UNITY_EDITOR
    public async override void Editor_Setting(MapEditor mapEditor)
    {
        Util util = new Util();
        await util.Delay(() =>
        {
            try
            {
                foreach (Transform tr in mapEditor.buttonActivatableObjectTransform)
                {
                    if (tr.TryGetComponent(out Portal component))
                    {
                        if (targetPosition == (Vector2)component.transform.position)
                        {
                            targetPortal = component;
                            return;
                        }
                    }
                }
            }
            catch
            {
                Debug.Log("Can't find Transform");
                return;
            }


        });
    }
#endif

    #endregion


    #endregion


    #region Portal Logic
    private void FixedUpdate()
    {
        if (NetworkServer.active)
        {
            if (Net.onActive)
            {
                ActiveOnRay();
            }
        }
        
    }

    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }

    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }


    #endregion

    #region Interactable
    //todo 0913 RayCast
    private void ActiveOnRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, .5f, layer);
        if (hit.collider != null)
        {
            // Portal_Net.Cmd_UsePortal(hit.collider.gameObject);
            var item = hit.collider.TryGetComponent(out NetworkIdentity identity) ? identity : null;
            if (item != null)
            {
                Net.Server_PlayUniqueEffect(item.netId);
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up * .5f);
    }
    #endif

    #endregion

}

