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

        [SerializeField] private bool _isEnabled;

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

        private void UpdateLaser()
        {
            if (Physics2D.Raycast(transform.position, transform.right))
            {
                RaycastHit2D _hit = Physics2D.Raycast(transform.position, transform.right.normalized, _defDistanceRay);
                DrawLaser(_hit.point);
                _endVFX.SetActive(true);
                _endVFX.transform.position = _hit.point;
            }
            else
            {
                DrawLaser(_firePoint.transform.right * _defDistanceRay);
                _endVFX.SetActive(false);
            }
        }

        private void DrawLaser(Vector2 endPos)
        {
            _lineRenderer.SetPosition(0,_firePoint.position);
            _lineRenderer.SetPosition(1, new Vector3(endPos.x,_firePoint.position.y,0));
        }
    }
}
