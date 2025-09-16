namespace Driball.Managers 
{
    using Cinemachine;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int targetPoints = 100;
        [SerializeField] private float startTime = 180f;

        [Header("UI References")]
        [SerializeField] private Image pointsBar;
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private CanvasGroup gameHUD;
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject defeatScreen;
        [SerializeField] private GameObject timesUpScreen;
        [SerializeField] private GameObject pauseScreen;

        [Header("Gameplay References")]
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private GemSpawner gemSpawner;
        [SerializeField] private CountdownTimer countdownTimer;
        [SerializeField] private CinemachineVirtualCamera cinemachine;
        [SerializeField] private GameObject playerPrefab;

        private Controls controls;
        private Transform player;
        private int points;
        private bool isPaused;

        private const int PointDeductionOnHit = 5;
        private const float DelayBeforeDefeat = 3f;

        private void Awake() => controls = new Controls();

        private void OnEnable()
        {
            EnableInput();
            SubscribeEvents();
        }

        private void OnDisable()
        {
            DisableInput();
            UnsubscribeEvents();
        }

        private void Start()
        {
            player = InstantiatePlayer();
            SetupCameraFollow(player);

            enemySpawner.InitializeSpawner(player, this);
            enemySpawner.StartSpawning();
            gemSpawner.StartSpawning();

            countdownTimer.StartTimer(startTime);
        }

        private Transform InstantiatePlayer()
        {
            GameObject playerObj = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
            return playerObj.transform;
        }

        private void SetupCameraFollow(Transform target)
        {
            cinemachine.LookAt = target;
            cinemachine.Follow = target;
        }

        #region Input
        private void EnableInput()
        {
            controls.UI.Enable();
            controls.UI.Pause.performed += OnPauseInput;
        }

        private void DisableInput()
        {
            controls.UI.Pause.performed -= OnPauseInput;
            controls.UI.Disable();
        }

        private void OnPauseInput(InputAction.CallbackContext ctx) => TogglePause();
        #endregion

        #region Points
        private void UpdateGemPoints(int gainedPoints)
        {
            points = Mathf.Min(points + gainedPoints, targetPoints);
            UpdatePointsUI();

            if (points >= targetPoints)
            {
                GameEvents.GemCompleted();
            }
        }

        private void ReduceGemPoints()
        {
            if (points - PointDeductionOnHit < 0)
            {
                GameEvents.GameOver();
            }

            points = Mathf.Max(points - PointDeductionOnHit, 0);
            UpdatePointsUI();
        }

        private void UpdatePointsUI()
        {
            pointsBar.fillAmount = GetGemPercentageProgress();
            pointsText.text = $"{points} / {targetPoints}";
        }

        public float GetGemPercentageProgress() => (float)points / targetPoints;
        #endregion

        #region Screens
        private void ShowDefeatScreen()
        {
            HideHUD();
            defeatScreen.SetActive(true);
        }

        private void ShowVictoryScreen()
        {
            HideHUD();
            victoryScreen.SetActive(true);
        }

        private void ShowTimesUpScreen()
        {
            HideHUD();
            timesUpScreen.SetActive(true);
            StartCoroutine(ShowDefeatAfterDelay());
        }

        private IEnumerator ShowDefeatAfterDelay()
        {
            yield return new WaitForSeconds(DelayBeforeDefeat);
            timesUpScreen.SetActive(false);
            ShowDefeatScreen();
        }
        #endregion

        #region Pause
        public void TogglePause()
        {
            isPaused = !isPaused;

            Time.timeScale = isPaused ? 0 : 1;
            pauseScreen.SetActive(isPaused);

            if (isPaused)
            {
                HideHUD();
            }
            else
            {
                ShowHUD();
            }
        }

        private void ShowHUD() => gameHUD.alpha = 1;
        private void HideHUD() => gameHUD.alpha = 0;
        #endregion

        #region Scene Management
        public void Restart()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        public void GoToMainMenu() => SceneManager.LoadScene("Main Menu");
        #endregion

        #region Events
        private void SubscribeEvents()
        {
            GameEvents.OnGemCollected += UpdateGemPoints;
            GameEvents.OnPlayerHit += ReduceGemPoints;

            GameEvents.OnGemCompleted += ShowVictoryScreen;
            GameEvents.OnGameOver += ShowDefeatScreen;
            GameEvents.OnTimesUp += ShowTimesUpScreen;

            GameEvents.OnGemCompleted += enemySpawner.StopSpawning;
            GameEvents.OnGameOver += enemySpawner.StopSpawning;
            GameEvents.OnTimesUp += enemySpawner.StopSpawning;

            GameEvents.OnGemCompleted += gemSpawner.StopSpawning;
            GameEvents.OnGameOver += gemSpawner.StopSpawning;
            GameEvents.OnTimesUp += gemSpawner.StopSpawning;

            GameEvents.OnGemCompleted += countdownTimer.StopTimer;
            GameEvents.OnGameOver += countdownTimer.StopTimer;
            GameEvents.OnTimesUp += countdownTimer.StopTimer;
        }

        private void UnsubscribeEvents()
        {
            GameEvents.OnGemCollected -= UpdateGemPoints;
            GameEvents.OnPlayerHit -= ReduceGemPoints;

            GameEvents.OnGemCompleted -= ShowVictoryScreen;
            GameEvents.OnGameOver -= ShowDefeatScreen;
            GameEvents.OnTimesUp -= ShowTimesUpScreen;

            GameEvents.OnGemCompleted -= enemySpawner.StopSpawning;
            GameEvents.OnGameOver -= enemySpawner.StopSpawning;
            GameEvents.OnTimesUp -= enemySpawner.StopSpawning;

            GameEvents.OnGemCompleted -= gemSpawner.StopSpawning;
            GameEvents.OnGameOver -= gemSpawner.StopSpawning;
            GameEvents.OnTimesUp -= gemSpawner.StopSpawning;

            GameEvents.OnGemCompleted -= countdownTimer.StopTimer;
            GameEvents.OnGameOver -= countdownTimer.StopTimer;
            GameEvents.OnTimesUp -= countdownTimer.StopTimer;
        }
        #endregion
    }
}
