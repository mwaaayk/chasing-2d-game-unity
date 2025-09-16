namespace Driball.Enemies
{
    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class ShockwaveRing : MonoBehaviour
    {
        [Header("Ring Settings")]
        [SerializeField] private int segments = 64;
        [SerializeField] private float startRadius = 0.5f;
        [SerializeField] private float maxRadius = 8f;
        [SerializeField] private float expandSpeed = 3f;
        [SerializeField] private float lineWidth = 0.2f;
        [SerializeField] private float fadeDuration = 0.5f;

        private LineRenderer lineRenderer;
        private PolygonCollider2D polygonCollider;

        private float currentRadius;
        private float fadeTimer;
        private bool isFading;

        private void Start()
        {
            InitializeComponents();
            currentRadius = startRadius;
            UpdateVisualsAndCollider();
        }

        private void Update()
        {
            if (isFading)
            {
                HandleFading();
            }
            else
            {
                ExpandRing();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentRadius);
        }

        private void InitializeComponents()
        {
            lineRenderer = GetComponent<LineRenderer>();
            polygonCollider = GetComponent<PolygonCollider2D>();

            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }

        #region Ring Logic
        private void ExpandRing()
        {
            currentRadius += expandSpeed * Time.deltaTime;
            UpdateVisualsAndCollider();

            if (currentRadius >= maxRadius)
            {
                StartFading();
            }
        }

        private void StartFading()
        {
            isFading = true;
            fadeTimer = fadeDuration;
        }

        private void HandleFading()
        {
            fadeTimer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

            Color color = lineRenderer.startColor;
            color.a = alpha;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            if (fadeTimer <= 0f)
                Destroy(gameObject);
        }
        #endregion

        #region Visual & Collider
        private void UpdateVisualsAndCollider()
        {
            UpdateLineRenderer();
            UpdatePolygonCollider();
        }

        private void UpdateLineRenderer()
        {
            lineRenderer.positionCount = segments;

            for (int i = 0; i < segments; i++)
            {
                float angle = GetAngle(i);
                Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * currentRadius;
                lineRenderer.SetPosition(i, point);
            }
        }

        private void UpdatePolygonCollider()
        {
            Vector2[] outer = new Vector2[segments];
            Vector2[] inner = new Vector2[segments];

            float outerRadius = currentRadius + lineWidth * 0.5f;
            float innerRadius = Mathf.Max(0f, currentRadius - lineWidth * 0.5f);

            for (int i = 0; i < segments; i++)
            {
                float angle = GetAngle(i);

                outer[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * outerRadius;
                inner[segments - 1 - i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * innerRadius;
            }

            Vector2[] ringPath = new Vector2[outer.Length + inner.Length];
            outer.CopyTo(ringPath, 0);
            inner.CopyTo(ringPath, outer.Length);

            polygonCollider.pathCount = 1;
            polygonCollider.SetPath(0, ringPath);
        }

        private float GetAngle(int index)
        {
            return (float)index / segments * Mathf.PI * 2f;
        }
        #endregion
    }
}