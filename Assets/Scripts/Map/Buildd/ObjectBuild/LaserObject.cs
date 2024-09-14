using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
    public class LaserObject : BuildObj
    {
        [CustomHeader("LaserObject")]
        [SerializeField] private float _defDistanceRay = 50f;
        public float _curDistanceRay;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _endVFX;
        [SerializeField] LayerMask _layerMask;
        [SerializeField] private bool _isEnabled;
        
        [Header("Effect")]
        [SerializeField] ParticleSystem hitEffectParticle;

        private Ray ray;
        bool onHit;
        bool onRecoveryRay;

        #region Editor Property
        public Coroutine editor_showLaserCoroutine;
        #endregion

        private void Awake()
        {
            _isEnabled = true;

            _endVFX.SetActive(_isEnabled);
            _lineRenderer.enabled = _isEnabled;
        }


        private void FixedUpdate()
        {
            if(!MapEditor.Instance.stageClear && !turnOff)
            {
                UpdateLaser();
            }
            else
            {
                _isEnabled = false;
                _endVFX.SetActive(_isEnabled);
                _lineRenderer.enabled = false;
            }
        }
        private void UpdateLaser()
        {
            Vector2 start = transform.position;
            Vector2 dir = transform.right;

            int hitCount = 0;  
            
            
            for (int i = 0; i < 10; i++)
            {
                ray = new Ray(start, dir);  
                RaycastHit2D rh = Physics2D.Raycast(ray.origin, ray.direction, _defDistanceRay);
                if (rh.collider != null)
                {
                    Vector2 colDir = rh.normal;
                    // Debug.DrawLine(start, rh.point, Color.green);
                    DrawLaser(i,start, rh.point);
                    hitCount++;
                    //Check collider
                     if(rh.collider.TryGetComponent(out IDamageable component)  && Application.isPlaying){
                         SetHitParticleRotate(start,rh.point); // todo 0914
                         component.TakeDamage();
                         break;
                     }else if(rh.collider.TryGetComponent(out MirrorObject component1)){
                         start = rh.point;
                         dir = Vector2.Reflect(ray.direction, colDir);
                     }else if(rh.collider.TryGetComponent(out LaserTriggerButton component2)){
                            if(Application.isPlaying){
                                Debug.Log("Is playing,Detected Laser Object");
                                SetHitParticleRotate(start,rh.point); // todo 0914
                                component2.SendMessage("Charging",SendMessageOptions.DontRequireReceiver);
                            }else{
                                Debug.Log("Detected Laser Trigger Object");
                            }
                         break;
                     }
                }
                else
                {
                    if(hitCount == 0){
                        _lineRenderer.positionCount = 0;
                    }
                    break;
                }

            }

        }
        private void SetHitParticleRotate(Vector3 start,Vector3 hitPoint){
            if(hitEffectParticle.transform.position != hitPoint){
                hitEffectParticle.transform.position = hitPoint;
            }

            Vector2 direction = hitPoint - start;
            float angle = Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(angle,-90,0);

            if(hitEffectParticle.transform.rotation != rotation){
                hitEffectParticle.transform.rotation = Quaternion.Euler(angle,-90,0);
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
            Debug.Log("Editor Laser");
            UpdateLaser();
        }
       public void ResetLaser(){
        _lineRenderer.positionCount = 0;
        _isEnabled = false;
        _endVFX.SetActive(_isEnabled);
        _lineRenderer.enabled = _isEnabled;
       }

        #endregion


        IEnumerator Co_RecoveryRay()
        {
            onRecoveryRay = true;

            while (!onHit && _curDistanceRay <_defDistanceRay)
            {
                _curDistanceRay += Time.deltaTime+1f;
                yield return null;
            }
            onRecoveryRay = false;

        }


        //private void DrawLaser(Vector2 endPos)
        //{
        //    _lineRenderer.SetPosition(0,_firePoint.position);
        //    _lineRenderer.SetPosition(1, endPos);
        //}
        private void DrawLaser(int num,Vector2 start, Vector2 endPos)
        {
            _lineRenderer.positionCount = num + 2;
            _lineRenderer.SetPosition(num, start);
            _lineRenderer.SetPosition(num+1, endPos);
        }


#if UNITY_EDITOR

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(_firePoint.position, _firePoint.right * 50);
        //}
#endif

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

