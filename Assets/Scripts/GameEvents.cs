namespace Driball
{
    using System;

    public static class GameEvents
    {
        public static event Action<int> OnGemCollected;
        public static event Action OnPlayerHit;
        public static event Action OnTimesUp;
        public static event Action OnGemCompleted;
        public static event Action OnEnemyKill;
        public static event Action OnGameOver;
        public static event Action OnEnemySpawned;
        public static event Action OnPlayerSpawned;
        public static event Action OnPlayerLanded;

        public static void GemCollected(int points) => OnGemCollected?.Invoke(points);
        public static void PlayerHit() => OnPlayerHit?.Invoke();
        public static void PlayerSpawned() => OnPlayerSpawned?.Invoke();
        public static void PlayerLanded() => OnPlayerLanded?.Invoke();
        public static void TimesUp() => OnTimesUp?.Invoke();
        public static void GemCompleted() => OnGemCompleted?.Invoke();
        public static void EnemyKill() => OnEnemyKill?.Invoke();
        public static void EnemySpawned() => OnEnemySpawned?.Invoke();
        public static void GameOver() => OnGameOver?.Invoke();
    }
}