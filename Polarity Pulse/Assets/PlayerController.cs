using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color colorA = Color.cyan;
    public Color colorB = Color.magenta;
    public Color colorC = Color.yellow;
    public Color colorD = Color.red;
    public Color colorE = Color.green;
    public float colorTransitionSpeed = 12f;

    [Header("State")]
    public int currentColorIndex = 0;
    private SpriteRenderer sr;
    private Color targetColor;

    [Header("Effects")]
    public ParticleSystem burstEffect;
    private float startStabilityTimer = 0.5f;

    void Start()
    {
   
        sr = GetComponent<SpriteRenderer>();
        targetColor = colorA;
        sr.color = colorA;
        transform.position = Vector3.zero;
    }

    void Update()
    {
        if (startStabilityTimer > 0) startStabilityTimer -= Time.deltaTime;
        float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.12f;
        transform.localScale = new Vector3(pulse, pulse, 1f);

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            // Check if the click is over a UI element (like a button)
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                CycleColor();
            }
        }

        sr.color = Color.Lerp(sr.color, targetColor, Time.deltaTime * colorTransitionSpeed);
    }

    void CycleColor()
    {
        int score = ScoreManager.Instance.currentScore;

        // Determine how many colors the player can cycle through
        int availableColors = 2;
        if (score >= 150) availableColors = 5;
        else if (score >= 100) availableColors = 4;
        else if (score >= 50) availableColors = 3;

        currentColorIndex = (currentColorIndex + 1) % availableColors;

        // Map index to target color
        switch (currentColorIndex)
        {
            case 0: targetColor = colorA; break;
            case 1: targetColor = colorB; break;
            case 2: targetColor = colorC; break;
            case 3: targetColor = colorD; break;
            case 4: targetColor = colorE; break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (startStabilityTimer > 0) return;
        if (other.CompareTag("Pulse"))
        {
            PulseController pulse = other.GetComponent<PulseController>();
            if (pulse != null)
            {
                // We compare the Pulse's Sprite color to our current target color
                Color pulseCol = pulse.GetComponent<SpriteRenderer>().color;

                if (IsColorMatch(pulseCol, targetColor))
                {
                    SpawnBurst();
                    ScoreManager.Instance.AddPoint();
                    other.gameObject.SetActive(false);
                }
                else
                {
                    if (CameraShake.Instance != null) CameraShake.Instance.ShakeOnGameOver();
                    UIManager.Instance.ShowGameOver();
                }
            }
        }
    }

    bool IsColorMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.1f &&
               Mathf.Abs(a.g - b.g) < 0.1f &&
               Mathf.Abs(a.b - b.b) < 0.1f;
    }

    void SpawnBurst()
    {
        ParticleSystem effect = Instantiate(burstEffect, transform.position, Quaternion.identity);
        var colOverLifetime = effect.colorOverLifetime;
        colOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(targetColor, 0.0f);
        colorKeys[1] = new GradientColorKey(targetColor, 1.0f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(0.0f, 1.0f);

        gradient.SetKeys(colorKeys, alphaKeys);
        colOverLifetime.color = gradient;

        effect.Play();
        Destroy(effect.gameObject, 1f);
    }
}