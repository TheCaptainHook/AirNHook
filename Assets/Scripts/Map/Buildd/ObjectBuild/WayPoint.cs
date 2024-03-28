using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class WayPoint : MonoBehaviour
{
    bool onWayPoint;
    NetworkStartPosition networkStartPosition;
    [SerializeField] ParticleSystem particle;
    private void Awake()
    {
        networkStartPosition = GetComponent<NetworkStartPosition>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!onWayPoint)
            {
                particle.Play();
                onWayPoint = true;
                MapEditor.Instance.startPositionObject.GetComponent<NetworkStartPosition>().enabled = false;
                MapEditor.Instance.startPositionObject = this.gameObject;
                //MapEditor.INstance.startpo = transform.position;
                networkStartPosition.enabled = true;
            }
        }
    }
}
