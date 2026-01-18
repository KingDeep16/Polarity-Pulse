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

    void OnEnable()
    {
        // 1. Immediately clear the trail memory
        if (tr != null)
        {
            tr.Clear();
            tr.enabled = false; // Keep it off initially
        }

        // 2. Clear any accidental particle bursts
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Clear();
            ps.Stop();
        }

        // 3. Start the "Silent Spawn" routine
        StartCoroutine(SilentSpawnRoutine());
    }

    private System.Collections.IEnumerator SilentSpawnRoutine()
    {
        // Wait for the end of the very first frame to ensure 
        // the spawner has finished moving and parenting this object.
        yield return new WaitForEndOfFrame();

        // Now that we are safely at the spawn position, wipe again and enable
        if (tr != null)
        {
            tr.Clear();
            tr.enabled = true;
        }

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null) ps.Play();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);

        if (tr != null && tr.enabled)
        {
            UpdateTrailGradient();
        }

        if (Vector3.Distance(transform.position, Vector3.zero) < 0.05f)
            gameObject.SetActive(false);
    }

    void UpdateTrailGradient()
    {
        float distance = Vector3.Distance(transform.position, Vector3.zero);
        float t = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, distance);
        float smoothedAlpha = Mathf.SmoothStep(0f, 1f, t);
        float finalAlpha = smoothedAlpha * maxAlpha;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(currentBaseColor, 0.0f), new GradientColorKey(currentBaseColor, 1.0f) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(finalAlpha, 0.0f),
                new GradientAlphaKey(finalAlpha * 0.5f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        tr.colorGradient = gradient;
    }

    public void Setup(bool typeA, Color col, float moveSpeed)
    {
        isColorA = typeA;
        speed = moveSpeed;
        currentBaseColor = col;
        GetComponent<SpriteRenderer>().color = col;
    }
}