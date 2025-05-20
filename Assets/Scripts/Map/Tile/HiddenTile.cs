using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenTile : MonoBehaviour
{
    private TilemapRenderer _tilemapRenderer;

    private void Awake()
    {
        _tilemapRenderer = GetComponent<TilemapRenderer>();
        _tilemapRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        other.gameObject.GetComponent<PlayerSM>().spriteMask.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        other.gameObject.GetComponent<PlayerSM>().spriteMask.SetActive(false);
    }
}
