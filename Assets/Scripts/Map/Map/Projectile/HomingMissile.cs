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
  private Transform _target = null;

    [SerializeField] private float _maxTimer;
    [SerializeField] private float _curTimer;

    private void FixedUpdate()
    {
        if(_curTimer >= _maxTimer)
        {
            //=============Boom
            Destroy(gameObject);
            //=============Boom
            return;
        }
        _curTimer += Time.fixedDeltaTime;
        
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
            RB.velocity = transform.right * _moveSpeed;
            // RB.velocity = Vector2.zero;
        }
    }
    private void Homing(Transform target)
    {
        Vector2 dir = ((Vector2)target.position - RB.position).normalized;
        float noise = Mathf.PerlinNoise(Time.time * 2f,0f) - 0.5f;
        float cross = Vector3.Cross(transform.right,dir).z;

        if(cross != 0) RB.angularVelocity =(cross * _rotateSpeed) + noise * 50f;
        else RB.angularVelocity = 0f;
        RB.velocity = transform.right * _moveSpeed;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        RB.AddForce(transform.right * 20f, ForceMode2D.Impulse);

        _onTarget = true;
    }


  private void Clean()
    {
        _curTimer = 0;
        _onTarget = false;
        _target = null;
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }


    #region  Util
    private bool TargetDistanceCheck(Transform target)
    {
        if(target == null) return false;

        float sqrDist = (target.transform.position - transform.position).sqrMagnitude;

        if(sqrDist < _maxHomingDistance * _maxHomingDistance)
        {
            return true;
        }
        return false;
    }

    #endregion




    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered");
    }
}
