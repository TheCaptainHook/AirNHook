using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AudioClipData
{
    public AudioType audioType;
    public AudioClip audioClip;
}

[CreateAssetMenu(fileName = "AudioClipSO")]
public class AudioClipSO : ScriptableObject
{
    [field: SerializeField] public List<AudioClipData> audioList;
}
