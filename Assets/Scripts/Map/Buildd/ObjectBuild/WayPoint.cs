using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class WayPoint : MonoBehaviour
{
    public bool onWayPoint;
    NetworkStartPosition networkStartPosition;
    [SerializeField] ParticleSystem particle;

    SpawnPointObj spawnPointObj;


    private void Awake()
    {
        networkStartPosition = GetComponent<NetworkStartPosition>();
        spawnPointObj = MapEditor.Instance.startPositionObject.GetComponent<SpawnPointObj>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!onWayPoint)
            {
                if (spawnPointObj.onSpawn) { spawnPointObj.EnableNetWorkStartPosition(); }

                CheckOtherWayPoint();

                particle.Play();

            }
        }
    }


    private void CheckOtherWayPoint()
    {
        foreach(Transform tr in MapEditor.Instance.objectTransform)
        {
            WayPoint wp = tr.GetComponent<WayPoint>();
            if(wp != null)
            {
                if (wp.onWayPoint)
                {
                    wp.EnableNetWorkStartPosition();
                }
               
            }
        }

        onWayPoint = true;
        networkStartPosition.enabled = true;
    }

    public void EnableNetWorkStartPosition()
    {
        onWayPoint = false;
        networkStartPosition.enabled = false;
    }
}
