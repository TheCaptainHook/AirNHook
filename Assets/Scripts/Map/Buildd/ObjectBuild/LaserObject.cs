using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapObjects
{
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
            RaycastHit2D hit = Physics2D.Raycast(_firePoint.position,_firePoint.right, _curDistanceRay,_layerMask);
            Debug.DrawRay(_firePoint.position, _firePoint.right * _curDistanceRay, Color.blue);
            if (hit.collider is not null)
            {
                onHit = true;
                _curDistanceRay = hit.distance;
                //Debug.DrawRay(_firePoint.position, _firePoint.right * _curDistanceRay, Color.green);
                // 레이캐스트에 충돌한 객체가 IDamageable을 가진 경우
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    // If successful, apply damage
                    damageable.TakeDamage();
                }


                DrawLaser(hit.point);
                _endVFX.SetActive(true);
                _lineRenderer.enabled = true;
                _endVFX.transform.position = hit.point;

            }
            else
            {
                onHit = false;
                if (!onRecoveryRay)
                {
                    StartCoroutine(Co_RecoveryRay());
                }
            }


           

        }

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


        private void DrawLaser(Vector2 endPos)
        {
            _lineRenderer.SetPosition(0,_firePoint.position);
            _lineRenderer.SetPosition(1, endPos);
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
}
