using UnityEngine;

public class ScreenSpinController : MonoBehaviour
{
    [Header("Phase Thresholds")]
    public int startSpinScore = 20;
    public int stopSpinScore = 50;
    public int restartSpinScore = 70;

    [Header("Spin Settings")]
    public float baseSpinSpeed = 25f;
    public float accelerationFactor = 0.5f;

    private float currentRotation = 0f;
    private float currentActiveSpeed = 0f;
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null) Debug.LogError("ScreenSpinController: No ScoreManager found in scene!");
    }

    void Update()
    {
        if (scoreManager == null) return;

        int score = scoreManager.currentScore;
        float targetSpeed = 0f;

        // Logic for Game Phases
        if (score >= restartSpinScore)
        {
            // Phase 3: Spin restarts and gets faster as score climbs
            targetSpeed = baseSpinSpeed + ((score - restartSpinScore) * accelerationFactor);
        }
        else if (score >= stopSpinScore)
        {
            // Phase 2: Spin is disabled while player learns the 3rd color
            targetSpeed = 0f;
        }
        else if (score >= startSpinScore)
        {
            // Phase 1: Initial Spin activation
            targetSpeed = baseSpinSpeed;
        }

        // Smoothly ramp the speed so the transition feels 'high-end'
        currentActiveSpeed = Mathf.MoveTowards(currentActiveSpeed, targetSpeed, 20f * Time.deltaTime);

        if (currentActiveSpeed != 0)
        {
            currentRotation += currentActiveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }
}