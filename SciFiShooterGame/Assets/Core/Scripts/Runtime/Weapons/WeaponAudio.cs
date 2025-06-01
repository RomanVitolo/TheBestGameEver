using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public class WeaponAudio : MonoBehaviour
    {
        [field: SerializeField] public AudioSource WeaponAudioSource { get; private set; }
    }
}