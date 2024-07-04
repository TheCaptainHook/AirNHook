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
        _tilemapRenderer.maskInteraction = SpriteMaskInteraction.None;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        _tilemapRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        _tilemapRenderer.maskInteraction = SpriteMaskInteraction.None;
    }
}
