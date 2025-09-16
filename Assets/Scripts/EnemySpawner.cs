namespace Driball
{
    using Driball.Enemies;
    using Driball.Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemySpawner : MonoBehaviour
    {
        [System.Serializable]
        private class EnemyPool
        {
            public GameObject Prefab;
            [HideInInspector] public List<Enemy> Pool = new();
        }

        [Header("Enemy Prefabs")]
        [SerializeField] private EnemyPool[] enemyPools;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int maxEnemiesPerType = 10;
        [SerializeField] private float spawnRadius = 8f;

        private bool isSpawning = false;
        private Transform player;
        private GameManager gameManager;
        private Coroutine spawnRoutine;

        public void InitializeSpawner(Transform player, GameManager gameManager)
        {
            this.player = player;
            this.gameManager = gameManager;

            foreach (EnemyPool pool in enemyPools)
            {
                PrewarmPool(pool);
            }
        }

        public void StartSpawning()
        {
            if (isSpawning) return;

            isSpawning = true;
            spawnRoutine = StartCoroutine(SpawnLoop());
        }

        public void StopSpawning()
        {
            if (!isSpawning) return;

            isSpawning = false;
            if (spawnRoutine != null)
                StopCoroutine(spawnRoutine);
        }

        private IEnumerator SpawnLoop()
        {
            float minInterval = spawnInterval * 0.2f;
            float maxInterval = spawnInterval;

            while (isSpawning)
            {
                float dynamicInterval = Mathf.Lerp(maxInterval, minInterval, gameManager.GetGemPercentageProgress());
                yield return new WaitForSeconds(dynamicInterval);

                SpawnEnemy();
                GameEvents.EnemySpawned();
            }
        }

        private void PrewarmPool(EnemyPool pool)
        {
            for (int i = 0; i < maxEnemiesPerType; i++)
            {
                GameObject obj = Instantiate(pool.Prefab, transform);
                obj.SetActive(false);

                if (obj.TryGetComponent(out Enemy enemy))
                {
                    enemy.PlayerTarget = player;
                    pool.Pool.Add(enemy);
                }
            }
        }

        private void SpawnEnemy()
        {
            if (enemyPools.Length == 0) return;

            EnemyPool pool = enemyPools[Random.Range(0, enemyPools.Length)];
            Enemy enemy = GetPooledEnemy(pool);

            if (enemy != null)
            {
                enemy.transform.position = GetRandomPosition();
                enemy.gameObject.SetActive(true);
            }
        }

        private Enemy GetPooledEnemy(EnemyPool pool)
        {
            foreach (Enemy enemy in pool.Pool)
            {
                if (!enemy.gameObject.activeInHierarchy)
                    return enemy;
            }
            return null;
        }

        private Vector2 GetRandomPosition()
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            return (Vector2)transform.position + randomCircle;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}