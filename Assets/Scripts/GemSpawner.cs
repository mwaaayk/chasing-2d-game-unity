namespace Driball
{
    using UnityEngine;
    using System.Collections;

    public class GemSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] gemPrefabs;
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float spawnRadius = 8f;

        private Coroutine spawnRoutine;

        public void StartSpawning()
        {
            if (spawnRoutine == null)
            {
                spawnRoutine = StartCoroutine(SpawnLoop());
            }
        }

        public void StopSpawning()
        {
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            var wait = new WaitForSeconds(spawnInterval);

            while (true)
            {
                yield return wait;
                SpawnGem();
            }
        }

        private void SpawnGem()
        {
            if (gemPrefabs.Length == 0) return;

            GameObject prefab = gemPrefabs[Random.Range(0, gemPrefabs.Length)];
            GameObject gem = Instantiate(prefab);
            gem.transform.position = GetRandomPosition();
            gem.SetActive(true);
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