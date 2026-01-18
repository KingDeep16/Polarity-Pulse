using UnityEngine;

public class PulseController : MonoBehaviour
{
    public float speed = 6f;
    public bool isColorA;

    [Header("Fade Settings")]
    public float fadeStartDistance = 4f;
    public float fadeEndDistance = 1f;
    public float maxAlpha = 0.2f;

    private TrailRenderer tr;
    private Color currentBaseColor;

    void Awake()
    {
        tr = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);

        if (tr != null)
        {
            UpdateTrailGradient();
        }

        if (Vector3.Distance(transform.position, Vector3.zero) < 0.01f)
            gameObject.SetActive(false);
    }

    void UpdateTrailGradient()
    {
        float distance = Vector3.Distance(transform.position, Vector3.zero);

        // 1. Calculate the raw linear percentage
        float t = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, distance);

        // 2. APPLY SMOOTHING
        // Use Mathf.SmoothStep for a 'S-Curve' (slow start, slow end)
        // Or use (t * t) for a 'Quadratic' fade (dissolves slowly at first, then accelerates)
        float smoothedAlpha = Mathf.SmoothStep(0f, 1f, t);

        float finalAlpha = smoothedAlpha * maxAlpha;

        Gradient gradient = new Gradient();

        // 3. COLOR KEYS (Keep HDR for glow)
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(currentBaseColor, 0.0f);
        colorKeys[1] = new GradientColorKey(currentBaseColor, 1.0f);

        // 4. ALPHA KEYS
        // To make the fade feel even more gradual, we can add a middle key
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0] = new GradientAlphaKey(finalAlpha, 0.0f); // Head of pulse
        alphaKeys[1] = new GradientAlphaKey(finalAlpha * 0.5f, 0.5f); // Halfway back
        alphaKeys[2] = new GradientAlphaKey(0.0f, 1.0f); // Tail end

        gradient.SetKeys(colorKeys, alphaKeys);
        tr.colorGradient = gradient;
    }

    public void Setup(bool typeA, Color col, float moveSpeed)
    {
        isColorA = typeA;
        speed = moveSpeed;
        currentBaseColor = col;
        GetComponent<SpriteRenderer>().color = col;

        if (tr != null)
        {
            tr.emitting = false;
            tr.Clear();
            tr.emitting = true;
        }
    }
}