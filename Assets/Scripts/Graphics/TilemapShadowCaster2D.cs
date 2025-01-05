using UnityEngine;
using UnityEngine.Rendering.Universal;
     
namespace Game.Utils
{
    /// <summary>
    /// Generates ShadowCaster2Ds for every continuous block of a tilemap on Awake, applying some settings.
    /// </summary>
    public class TilemapShadowCaster2D : MonoBehaviour
    {
        [SerializeField]
        protected CompositeCollider2D m_TilemapCollider;
     
        [SerializeField]
        protected bool m_SelfShadows = false;
     
        protected virtual void Reset()
        {
            m_TilemapCollider = GetComponent<CompositeCollider2D>();
        }
     
        protected virtual void Awake()
        {
            ShadowCaster2DGenerator.GenerateTilemapShadowCasters(m_TilemapCollider, m_SelfShadows);
        }
    }
}
