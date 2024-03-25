using System;
using UnityEngine;

namespace Mirror
{
    /// <summary>Start position for player spawning, automatically registers itself in the NetworkManager.</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Start Position")]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-start-position")]
    public class NetworkStartPosition : MonoBehaviour
    {
        public void Awake()
        {
            NetworkManager.RegisterStartPosition(transform);
        }

        public void OnEnable()
        {
            NetworkManager.RegisterStartPosition(transform);
        }

        private void OnDisable()
        {
            NetworkManager.UnRegisterStartPosition(transform);
        }
    }
}
