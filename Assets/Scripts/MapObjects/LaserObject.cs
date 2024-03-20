using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapObjects
{
    public class LaserObject : MonoBehaviour
    {
        [SerializeField] private float _defDistanceRay = 50f;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _endVFX;
        [SerializeField] LayerMask _layerMask;
        [SerializeField] private bool _isEnabled;

        private Transform _transform;

        private void Start()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            Toggle();
            UpdateLaser();
        }

        private void Toggle()
        {
            _lineRenderer.enabled = _isEnabled;
            _endVFX.SetActive(_isEnabled);
        }

        //private void UpdateLaser()
        //{

        //    if (Physics2D.Raycast(_transform.position, transform.right))
        //    {
        //        RaycastHit2D _hit = Physics2D.Raycast(_transform.position, transform.right.normalized, _defDistanceRay);
        //        DrawLaser(_hit.point);
        //        _endVFX.SetActive(true);
        //        _endVFX.transform.position = _hit.point;
        //    }
        //    else
        //    {
        //        DrawLaser(transform.position + transform.right.normalized * _defDistanceRay);
        //        _endVFX.SetActive(false);
        //    }
        //}

        private void UpdateLaser()
        {
            RaycastHit2D hit = Physics2D.Raycast(_transform.position,transform.right.normalized, _defDistanceRay,_layerMask);

            if (hit.collider != null)
            {
                // 레이캐스트에 충돌한 객체가 IDamageable을 가진 경우
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    // 대미지를 입힘
                    damageable.Die();
                }

                // 레이저 그리기
                DrawLaser(hit.point);
                _endVFX.SetActive(true);
                _endVFX.transform.position = hit.point;
            }
            else
            {
                DrawLaser(_firePoint.position + _firePoint.right.normalized * _defDistanceRay);
                _endVFX.SetActive(false);
            }
        }


        private void DrawLaser(Vector2 endPos)
        {
            _lineRenderer.SetPosition(0,_firePoint.position);
            _lineRenderer.SetPosition(1, endPos);
        }
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_firePoint.position, _firePoint.right * 50);
        }
#endif
    }
}
