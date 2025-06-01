using UnityEngine;

namespace Core.Scripts.Runtime.AudioSystem
{
    public abstract class BaseAudioClipSO : ScriptableObject
    {
        [SerializeField] protected AudioClip _audioClip;
    }
}