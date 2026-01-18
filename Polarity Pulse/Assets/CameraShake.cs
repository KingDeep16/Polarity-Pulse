using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    // Call this when the player successfully catches a pulse
    public void ShakeOnPoint()
    {
        // Parameters: Duration, Strength, Vibrato, Randomness
        transform.DOComplete(); // Stop previous shake if overlapping
        transform.DOShakePosition(0.15f, 0.2f, 10, 90).OnComplete(() => transform.localPosition = originalPos);
    }

    // Call this when a new color is unlocked for a "thump" feel
    public void ShakeOnUnlock()
    {
        // A slower, deeper shake for milestones
        transform.DOComplete();
        transform.DOShakePosition(0.5f, 0.5f, 5, 90).SetUpdate(true).OnComplete(() => transform.localPosition = originalPos);
    }

    // Call this on Game Over for a violent jolt
    public void ShakeOnGameOver()
    {
        transform.DOComplete();
        // A heavy, long shake to signal the crash
        transform.DOShakePosition(0.6f, 1.0f, 20, 90).SetUpdate(true).OnComplete(() => transform.localPosition = originalPos);
    }
}