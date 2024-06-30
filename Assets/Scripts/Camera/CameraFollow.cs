using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private float _smoothSpeed = 0.25f;
    private Vector3 _vecVelocity = Vector3.zero;
    private Transform _player => Managers.Game.Player.transform;
    private void Update()
    {
        if (Managers.Game.CurrentState is GameState.Game or GameState.Lobby)
        {
            FollowPlayer();
        }
    }
    private void FollowPlayer()
    {
        try
        {
            if (_player == null) return;

            var _playerPos = new Vector3(_player.position.x, _player.position.y + 1f, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, _playerPos, ref _vecVelocity, _smoothSpeed,
                float.MaxValue, Time.fixedDeltaTime);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
