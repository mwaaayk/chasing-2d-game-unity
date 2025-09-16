namespace Driball.Enemies
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyShooter : Enemy
    {
        [Header("Shooting Settings")]
        [SerializeField] private GameObject shockwavePrefab;
        [SerializeField] private float attackInterval = 2f;

        [Header("Visual Settings")]
        [SerializeField] private float spinSpeed = 180f;

        private float attackCooldown = 0f;

        private void Update()
        {
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
            HandleShockwave();
        }

        private void HandleShockwave()
        {
            if (shockwavePrefab == null) return;

            attackCooldown += Time.deltaTime;
            if (attackCooldown >= attackInterval)
            {
                SpawnShockwave();
                attackCooldown = 0f;
            }
        }

        private void SpawnShockwave()
        {
            GameObject projectile = Instantiate(shockwavePrefab);
            projectile.transform.position = transform.position;
        }

        protected override void EndLifespan()
        {
            base.EndLifespan();
            gameObject.SetActive(false);
        }
    }
}