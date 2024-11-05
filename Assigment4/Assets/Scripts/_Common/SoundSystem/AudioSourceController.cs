using UnityEngine;

namespace GameBase.AudioPlayer
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceController : MonoBehaviour
    {
        public AudioSource audioSource;
        public bool IsLowPriority = false;
    }
}
