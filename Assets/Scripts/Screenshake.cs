namespace Driball
{
    using Cinemachine;
    using UnityEngine;
    using System.Collections;

    public class Screenshake : MonoBehaviour
    {
        public static Screenshake Instance;
        private CinemachineVirtualCamera cinemachineVirtualCamera;
        private Coroutine shakeRoutine;

        [SerializeField] private float _defaultShakeTime = 0.2f;

        private void Awake()
        {
            Instance = this;
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start() => StopShake();

        public void ShakeCamera(float intensity, float duration = -1f)
        {
            if (shakeRoutine != null)
                StopCoroutine(shakeRoutine);

            if (duration <= 0) duration = _defaultShakeTime;

            shakeRoutine = StartCoroutine(DoShake(intensity, duration));
        }

        private IEnumerator DoShake(float intensity, float duration)
        {
            CinemachineBasicMultiChannelPerlin cbmp = cinemachineVirtualCamera
                .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cbmp.m_AmplitudeGain = intensity;

            yield return new WaitForSeconds(duration);

            StopShake();
        }

        public void StopShake()
        {
            CinemachineBasicMultiChannelPerlin cbmp = cinemachineVirtualCamera
                .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cbmp.m_AmplitudeGain = 0;

            if (shakeRoutine != null)
            {
                StopCoroutine(shakeRoutine);
                shakeRoutine = null;
            }
        }
    }
}