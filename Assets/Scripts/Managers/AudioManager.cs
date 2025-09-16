namespace Driball.Managers 
{
    using UnityEngine;
    using UnityEngine.Audio;

    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        public void SetMusicVolume(float volume) => audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        public void SetSoundVolume(float volume) => audioMixer.SetFloat("SoundVolume", Mathf.Log10(volume) * 20);
    }
}
