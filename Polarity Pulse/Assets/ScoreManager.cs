using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Settings")]
    public int comboThreshold = 5;

    [HideInInspector] public int currentScore = 0;
    [HideInInspector] public int comboCount = 0;
    [HideInInspector] public int multiplier = 1;

    void Awake() { Instance = this; }

    public void AddPoint()
    {
        // 1. Calculate Multiplier Logic
        comboCount++;
        if (comboCount >= comboThreshold)
        {
            multiplier++;
            comboCount = 0;
            UIManager.Instance.TriggerMultiplierJuice(multiplier);
        }

        // 2. Add Score
        currentScore += (1 * multiplier);

        // 3. Update HUD
        UIManager.Instance.UpdateHUD(currentScore, multiplier);

        // 4. Trigger "Catch" Feedback
        if (CameraShake.Instance != null) CameraShake.Instance.ShakeOnPoint();

        // 5. Check for Color Unlock Milestones
        CheckForMilestones();
    }

    private bool unlocked50 = false;
    private bool unlocked100 = false;
    private bool unlocked150 = false;

    private void CheckForMilestones()
    {
        if (currentScore >= 50 && !unlocked50)
        {
            unlocked50 = true;
            UIManager.Instance.ShowColorUnlockPrompt(0);
            if (CameraShake.Instance != null) CameraShake.Instance.ShakeOnUnlock();
        }
        else if (currentScore >= 100 && !unlocked100)
        {
            unlocked100 = true;
            UIManager.Instance.ShowColorUnlockPrompt(1);
            if (CameraShake.Instance != null) CameraShake.Instance.ShakeOnUnlock();
        }
        else if (currentScore >= 150 && !unlocked150)
        {
            unlocked150 = true;
            UIManager.Instance.ShowColorUnlockPrompt(2);
            if (CameraShake.Instance != null) CameraShake.Instance.ShakeOnUnlock();
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
        multiplier = 1;
        UIManager.Instance.UpdateHUD(currentScore, multiplier);
    }
}