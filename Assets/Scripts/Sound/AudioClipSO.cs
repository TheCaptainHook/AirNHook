using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipSO")]
public class AudioClipSO : ScriptableObject
{
    [field: SerializeField] public List<AudioClip> audioList;
}
