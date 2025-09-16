namespace Driball.Managers
{
    using System.Collections.Generic;
    using UnityEngine;

    public class SFXManager : MonoBehaviour
    {
        [Header("Audio Source & Volume")]
        [SerializeField] private AudioSource audioSource;
        [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

        [Header("Cooldown & Pitch Settings")]
        [SerializeField] private float soundCooldown = 0.05f;
        [SerializeField] private float gemPitchStep = 0.05f;
        [SerializeField] private float gemPitchMax = 1.2f;
        [SerializeField] private float gemResetDelay = 2f;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip gemSound;
        [SerializeField] private AudioClip enemyKillSound;
        [SerializeField] private AudioClip playerHitSound;
        [SerializeField] private AudioClip playerLandedSound;
        [SerializeField] private AudioClip enemySpawnSound;

        private readonly Dictionary<AudioClip, float> lastPlayTime = new();
        private float currentGemPitch = 1f;
        private float lastGemTime = 0f;

        private void OnEnable()
        {
            GameEvents.OnGemCollected += HandleGemCollected;
            GameEvents.OnEnemyKill += () => PlaySFX(enemyKillSound);
            GameEvents.OnPlayerHit += () => PlaySFX(playerHitSound);
            GameEvents.OnPlayerLanded += () => PlaySFX(playerLandedSound);
            GameEvents.OnEnemySpawned += () => PlaySFX(enemySpawnSound);
        }

        private void OnDisable()
        {
            GameEvents.OnGemCollected -= HandleGemCollected;
            GameEvents.OnEnemyKill -= () => PlaySFX(enemyKillSound);
            GameEvents.OnPlayerHit -= () => PlaySFX(playerHitSound);
            GameEvents.OnPlayerLanded -= () => PlaySFX(playerLandedSound);
            GameEvents.OnEnemySpawned -= () => PlaySFX(enemySpawnSound);
        }

        private void Update() => ResetGemPitchIfNeeded();

        public void PlaySFX(AudioClip clip, float pitch = 1f)
        {
            if (clip == null || IsOnCooldown(clip)) return;

            lastPlayTime[clip] = Time.time;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip, sfxVolume);
        }

        private void HandleGemCollected(int points)
        {
            IncreaseGemPitch();
            PlaySFX(gemSound, currentGemPitch);
        }

        private void IncreaseGemPitch()
        {
            currentGemPitch = Mathf.Min(currentGemPitch + gemPitchStep, gemPitchMax);
            lastGemTime = Time.time;
        }

        private void ResetGemPitchIfNeeded()
        {
            if (currentGemPitch > 1f && Time.time - lastGemTime >= gemResetDelay)
                currentGemPitch = 1f;
        }

        private bool IsOnCooldown(AudioClip clip)
        {
            return lastPlayTime.TryGetValue(clip, out float lastTime) && Time.time - lastTime < soundCooldown;
        }
    }
}