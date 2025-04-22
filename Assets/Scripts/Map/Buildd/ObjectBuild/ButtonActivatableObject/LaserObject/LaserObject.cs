
using Mirror;
using System.Collections;
using UnityEngine;

public class LaserObject : ActivatableObjectEntity
    {
        [CustomHeader("LaserObject")]
        //[SerializeField] private float _defDistanceRay = 50f;
        public float _curDistanceRay;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _endVFX;
        [SerializeField] LayerMask _layerMask;
        [SerializeField] private bool _isEnabled;
        //private bool onActive;

        [Header("Effect")]
        [SerializeField] ParticleSystem hitEffectParticle;

        private Ray ray;
        // bool onHit;
        // bool onRecoveryRay;

        #region Editor Property
        public Coroutine editor_showLaserCoroutine;
    #endregion

    public override void SetData<T>(T data)
    {
        base.SetData(data);
        if(Application.isPlaying)
        {
            _Net.onSync = true;
            _Net.Server_InitSync();
        }
       
    }

    private void FixedUpdate()
    {
        if (!MapEditor.Instance.stageClear && !turnOff && _Net.onActive)
        {
            UpdateLaser();
        }
        else
        { 
            if(NetworkServer.active)
            _Net.Server_SetOnActive(false);
        }
    }
    protected override void Activation()
    {

        //onActive = true;
        if (NetworkServer.active)
            _Net.Server_SetOnActive(true);
    }

    protected override void Deactivated()
    {
        //onActive = false;
        if (NetworkServer.active)
            _Net.Server_SetOnActive(false);
    }
    

    private void Awake()
    {
        _Net = GetComponent<LaserObject_Net>();

    }
    #region Network
    private LaserObject_Net _Net;

    public void Net_Active()
    {
        _isEnabled = true;
        _endVFX.SetActive(_isEnabled);
        _lineRenderer.enabled = _isEnabled;

        //onActive = true;
    }
    public void Net_Deactive()
    {
        //onActive = false;
        _isEnabled = false;
        _endVFX.SetActive(_isEnabled);
        _lineRenderer.enabled = false;
    }
    #endregion

    private void UpdateLaser()
    {
        Vector2 start;
        Vector2 dir;
        start = _firePoint.position;
        dir = transform.right;

        int hitCount = 0;

        for (int i = 0; i < 5; i++)
        {
            ray = new Ray(start, dir);
            RaycastHit2D rh = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, _layerMask);
            if (rh.collider != null)
            {
                Vector2 colDir = rh.normal;
                DrawLaser(i, start, rh.point);
                hitCount++;

                //Check collider
                if (rh.collider.TryGetComponent(out PlayerSM component) && Application.isPlaying)
                {
                    SetHitParticleRotate(start, rh.point); // todo 0914
                    component.TakeDamage(DamageType.Fire);
                    break;

                }
                else if (rh.collider.gameObject.layer == LayerMask.NameToLayer("Mirror"))
                {
                    start = rh.point;
                    dir = Vector2.Reflect(ray.direction, colDir);
                }
                else if (rh.collider.TryGetComponent(out LaserTriggerButton component2))
                {
                    if (Application.isPlaying)
                    {
                        SetHitParticleRotate(start, rh.point); // todo 0914
                        //component2.SendMessage("Charging", SendMessageOptions.DontRequireReceiver);
                        component2.Charging();
                    }
                    else
                    {
                    }
                    break;
                }
                else
                {
                    SetHitParticleRotate(start, rh.point);
                    if (rh.collider.TryGetComponent(out IDamageable damageable))
                    {
                        if (Application.isPlaying) damageable.TakeDamage(DamageType.Fire);

                    }
                    break;
                }
            }
            else
            {
                if (hitCount == 0)
                {
                    _lineRenderer.positionCount = 0;
                }
                break;
            }

        }

    }
    private void SetHitParticleRotate(Vector3 start,Vector3 hitPoint){
            if(hitEffectParticle.transform.position != hitPoint){
                hitEffectParticle.transform.position = hitPoint;
                _endVFX.transform.position = hitPoint;
            }

            Vector2 direction = hitPoint - start;
            float angle = Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(angle,-90,0);

            if(hitEffectParticle.transform.rotation != rotation){
                hitEffectParticle.transform.rotation = Quaternion.Euler(angle,-90,0);
                _endVFX.transform.rotation = Quaternion.Euler(angle, -90, 0);
            }


            hitEffectParticle.Play();

        }

        #region  Editor
        public void Editor_Awake(){
            _isEnabled = true;

            _endVFX.SetActive(_isEnabled);
            _lineRenderer.enabled = _isEnabled;
        }
        public void Editor_UpdateLaser(){
            if(_isEnabled) UpdateLaser();
        }
       public void ResetLaser(){
        if(_lineRenderer == null) return; 
        _lineRenderer.positionCount = 0;
        _isEnabled = false;
        _endVFX.SetActive(_isEnabled);
        _lineRenderer.enabled = _isEnabled;
       }

        #endregion
        private void DrawLaser(int num,Vector2 start, Vector2 endPos)
        {
            _lineRenderer.positionCount = num + 2;
            _lineRenderer.SetPosition(num, start);
            _lineRenderer.SetPosition(num+1, endPos);
        }


        public override void TurnOff()
        {
            base.TurnOff();
            turnOff = true;
        }
        public override void TurnOn()
        {
            base.TurnOn();
            turnOff = false;
            _isEnabled = true;
        }
    }

