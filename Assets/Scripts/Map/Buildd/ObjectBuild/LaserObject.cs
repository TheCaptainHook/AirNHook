using System;
using System.Collections;
using System.Collections.Generic;
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


        //private void UpdateLaser()
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(_firePoint.position,_firePoint.right, _curDistanceRay,_layerMask);
        //    Debug.DrawRay(_firePoint.position, _firePoint.right * _curDistanceRay, Color.blue);
        //    if (hit.collider is not null)
        //    {
        //        onHit = true;
        //        _curDistanceRay = hit.distance;
        //        //Debug.DrawRay(_firePoint.position, _firePoint.right * _curDistanceRay, Color.green);
        //        // 레이캐스트에 충돌한 객체가 IDamageable을 가진 경우
        //        if (hit.collider.TryGetComponent(out IDamageable damageable))
        //        {
        //            // If successful, apply damage
        //            damageable.TakeDamage();
        //        }



        //        SetLaser(hit.point);

        //    }
        //    else
        //    {
        //        onHit = false;
        //        if (!onRecoveryRay)
        //        {
        //            StartCoroutine(Co_RecoveryRay());
        //        }
        //    }

        //}
        private void UpdateLaser()
        {
            Vector2 start = transform.position;
            Vector2 dir = transform.right;

            int hitCount = 0;  

            for (int i = 0; i < 3; i++)
            {
                var ray = new Ray(start, dir);
                RaycastHit2D rh = Physics2D.Raycast(ray.origin, ray.direction, _defDistanceRay);
                if (rh.collider != null)
                {
                    Vector2 colDir = rh.normal;
                    // Debug.DrawLine(start, rh.point, Color.green);
                    DrawLaser(i,start, rh.point);
                    hitCount++;
                    //Check collider

                     if(rh.collider.TryGetComponent(out IDamageable component) && Application.isPlaying){
                         component.TakeDamage();
                         break;
                     }else if(rh.collider.TryGetComponent(out MirrorObject component1)){
                         start = rh.point;
                         dir = Vector2.Reflect(start, colDir);
                     }else{
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

        //private void SetLaser(Vector2 hit)
        //{
        //    DrawLaser(hit);
        //    _endVFX.SetActive(true);
        //    _lineRenderer.enabled = true;
        //    _endVFX.transform.position = hit;
        //}


        IEnumerator Co_RecoveryRay()
        {
            onRecoveryRay = true;
            //_lineRenderer.enabled = false;
            //_endVFX.transform.position = transform.position;

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

