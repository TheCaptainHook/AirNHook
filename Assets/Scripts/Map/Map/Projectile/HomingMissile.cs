using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
  private Rigidbody2D rb;
  public Rigidbody2D RB {get{rb??=GetComponent<Rigidbody2D>(); return rb;} }

  [SerializeField] private float _moveSpeed = 10f;
  [SerializeField] private float _rotateSpeed = 200f;
  [SerializeField] private float _lifeTime = 5f;
  [SerializeField] private float _maxHomingDistance = 200f;

  private bool _onTarget = false;
  [SerializeField] private Transform _target;
  private bool _onReady = false;


    private void FixedUpdate()
    {
        // if(!_onReady) return;
        Launch();
    }

    private void Launch()
    {
        //======Target Distance Check
        if(TargetDistanceCheck(_target))_onTarget = true;
        else _onTarget = false;
        //======Target Distance Check

        if(_onTarget)
        {
            Homing(_target);
        }
        else
        {
            //=======일직선으로
            RB.velocity = Vector2.zero;
        }
    }
    private void Homing(Transform target)
    {
        Vector2 dir = ((Vector2)target.position - RB.position).normalized;
        float cross = Vector3.Cross(transform.right,dir).z;
        RB.angularVelocity = -cross * _rotateSpeed;
        RB.velocity = transform.right * _moveSpeed;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _onTarget = true;
    }


  


    #region  Util
    private bool TargetDistanceCheck(Transform target)
    {
        if(target == null) return false;

        float sqrDist = (target.transform.position - transform.position).sqrMagnitude;
        Debug.Log(sqrDist);

        if(sqrDist < _maxHomingDistance * _maxHomingDistance)
        {
            return true;
        }
        return false;
    }

    #endregion
}
