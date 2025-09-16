namespace Driball.Enemies
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private float lifeSpan = 5f;

        [Header("Effects")]
        [SerializeField] private EnemyShadow shadowPrefab;
        [SerializeField] private GameObject deathVFX;

        public Transform PlayerTarget { get; set; }

        protected Rigidbody2D rb { get; private set; }
        protected EnemyShadow shadow { get; private set; }

        private float lifeTimer;

        private void Awake() => rb = GetComponent<Rigidbody2D>();

        private void Start()
        {
            InitializeShadow();
            ResetLifeTimer();
        }

        protected virtual void OnEnable()
        {
            ResetLifeTimer();
            SetShadowActive(true);
        }

        protected virtual void OnDisable() => SetShadowActive(false);

        protected virtual void FixedUpdate() => UpdateLifeTimer();

        private void InitializeShadow()
        {
            if (shadowPrefab != null)
            {
                shadow = Instantiate(shadowPrefab);
                shadow.Caster = transform;
            }
        }

        private void ResetLifeTimer() => lifeTimer = lifeSpan;

        private void SetShadowActive(bool active)
        {
            if (shadow != null)
                shadow.gameObject.SetActive(active);
        }

        private void UpdateLifeTimer()
        {
            lifeTimer -= Time.fixedDeltaTime;
            if (lifeTimer <= 0f)
            {
                EndLifespan();
            }
        }

        protected virtual void EndLifespan()
        {
        }

        public void Die()
        {
            PlayDeathVFX();
            GameEvents.EnemyKill();
            gameObject.SetActive(false);
        }

        private void PlayDeathVFX()
        {
            if (deathVFX == null) return;

            GameObject vfx = Instantiate(deathVFX, transform.position, Quaternion.identity);
        }
    }
}