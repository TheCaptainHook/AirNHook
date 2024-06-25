using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MapObjects
{
    public class BridgeBox : BuildObj
    {
        // 제대로 작동 안함, 사실상 비주얼 테스트용 코드들
        
        [SerializeField] private float _defDistanceRay = 50f;
        public float _curDistanceRay;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _firePoint;
        [SerializeField] LayerMask _layerMask;
        [SerializeField] private bool _isEnabled;
        // [SerializeField] private EdgeCollider2D _edgeCollider2D;
        
        bool onHit;
        bool onRecoveryRay;
        private float _curBridgeDistance;
        private Vector2 _bridgeVector2;

        private void Awake()
        {
            _isEnabled = true;
            _lineRenderer.enabled = _isEnabled;
            // _edgeCollider2D.enabled = _isEnabled;
        }
        
        private void FixedUpdate()
        {
            if(!turnOff)
            {
                UpdateLaser();
            }
            else
            {
                _isEnabled = false;
                _lineRenderer.enabled = false;
                // _edgeCollider2D.enabled = false;
            }
        }


        private void UpdateLaser()
        {
            _lineRenderer.enabled = onHit;
            // _edgeCollider2D.enabled = onHit;
            
            RaycastHit2D hit = Physics2D.Raycast(_firePoint.position,_firePoint.right, _curDistanceRay,_layerMask);
            Debug.DrawRay(_firePoint.position, _firePoint.right * _curDistanceRay, Color.blue);
            if (hit.collider is not null)
            {
                StartCoroutine(Co_BridgeDrawing(hit.point));
                onHit = true;
                _curDistanceRay = hit.distance;
            }
            else
            {
                onHit = false;
                _curBridgeDistance = 0f;
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

        private void DrawBridge(Vector2 endPos)
        {
            _lineRenderer.SetPosition(0,_firePoint.position);
            _lineRenderer.SetPosition(1,_bridgeVector2);
            // _edgeCollider2D.points[1] = _firePoint.position;
            // _edgeCollider2D.points[2] = _firePoint.position;
            // _edgeCollider2D.points[0] = endPos;
            // _edgeCollider2D.points[3] = endPos;
        }

        IEnumerator Co_BridgeDrawing(Vector2 endPos)
        {
            _bridgeVector2.x = _curBridgeDistance;
            _bridgeVector2.y = endPos.y;
            
            while (_curBridgeDistance < endPos.x)
            {
                _curBridgeDistance += Time.deltaTime+0.005f;
                DrawBridge(endPos);
                yield return null;
            }
        }
        
#if UNITY_EDITOR

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawRay(_firePoint.position, _firePoint.right * 50);
        // }
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
