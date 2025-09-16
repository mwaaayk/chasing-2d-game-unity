namespace Driball.Enemies
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyChase : Enemy
    {
        [Header("Chase Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotationSpeed = 360f; // degrees per second
        [SerializeField] private float deactivateDistance = 30f;

        private bool isChasing = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            isChasing = true;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (PlayerTarget == null)
            {
                CheckDeactivateDistance();
                MoveForward();
                return;
            }

            if (isChasing)
            {
                RotateTowardsPlayer();
            }

            MoveForward();
            CheckDeactivateDistance();
        }

        private void RotateTowardsPlayer()
        {
            Vector2 direction = (PlayerTarget.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        private void MoveForward()
        {
            rb.velocity = transform.up * moveSpeed;
        }

        private void CheckDeactivateDistance()
        {
            if (Vector2.Distance(Vector2.zero, transform.position) > deactivateDistance)
            {
                gameObject.SetActive(false);
            }
        }

        protected override void EndLifespan()
        {
            base.EndLifespan();
            isChasing = false;
        }
    }
}