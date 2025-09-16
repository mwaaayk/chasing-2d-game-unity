namespace Driball
{
    using TMPro;
    using UnityEngine;
    using System.Collections;

    public class CountdownTimer : MonoBehaviour
    {
        [Header("UI Reference")]
        [SerializeField] private TextMeshProUGUI timerText;

        private float currentTime;
        private Coroutine timerCoroutine;

        public void StartTimer(float time)
        {
            StopTimer();

            currentTime = time;
            UpdateTimerUI();
            timerCoroutine = StartCoroutine(RunTimer());
        }

        public void StopTimer()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }

        public void ResumeTimer()
        {
            if (timerCoroutine == null)
                timerCoroutine = StartCoroutine(RunTimer());
        }

        private IEnumerator RunTimer()
        {
            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                if (currentTime < 0) currentTime = 0;

                UpdateTimerUI();
                yield return null;
            }

            OnTimerEnd();
        }

        private void OnTimerEnd()
        {
            GameEvents.TimesUp();
            timerCoroutine = null;
        }

        private void UpdateTimerUI()
        {
            if (timerText == null) return;

            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}