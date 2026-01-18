using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for using the Image component

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD (Gameplay UI)")]
    public TextMeshProUGUI hudScoreText;
    public TextMeshProUGUI hudMultiplierText;

    [Header("Game Over UI")]
    public CanvasGroup gameOverCanvasGroup;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Color Unlock UI")]
    public GameObject unlockPanel;         // The parent object/panel for the unlock prompt
    public Image unlockDisplayImage;      // The UI Image component that shows the icon
    public Sprite[] colorUnlockSprites;   // Element 0: Yellow, 1: Red, 2: Green

    void Awake() { Instance = this; }

    void Start()
    {
        hudScoreText.text = "Score: ";
        // Ensure the unlock panel is hidden at the start
        if (unlockPanel != null) unlockPanel.SetActive(false);
    }

    // --- NEW COLOR UNLOCK LOGIC ---

    public void ShowColorUnlockPrompt(int index)
    {
        if (index >= colorUnlockSprites.Length) return;

        // 1. Assign the correct colored sprite
        unlockDisplayImage.sprite = colorUnlockSprites[index];

        // 2. Prepare the panel (Set scale to 0 so it can pop in)
        unlockPanel.SetActive(true);
        unlockPanel.transform.localScale = Vector3.zero;

        // 3. Pause the game
        Time.timeScale = 0f;

        // 4. DOTween Animation Sequence
        // We use .SetUpdate(true) so the animation plays while Time.timeScale is 0
        Sequence unlockSequence = DOTween.Sequence();
        unlockSequence.SetUpdate(true);

        // Pop In
        unlockSequence.Append(unlockPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));

        // Slight "Heartbeat" while showing
        unlockSequence.Append(unlockPanel.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2, 0.5f));

        // Delay (Wait 1.5 seconds)
        unlockSequence.AppendInterval(1.2f);

        // Shrink Out
        unlockSequence.Append(unlockPanel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));

        // Finalize: Resume Game and hide panel
        unlockSequence.OnComplete(() => {
            Time.timeScale = 1f;
            unlockPanel.SetActive(false);
        });
    }

    // --- REMAINING EXISTING HUD & GAME OVER LOGIC ---

    public void UpdateHUD(int score, int multiplier)
    {
        hudScoreText.text = "Score: " + score.ToString();
        if (multiplier > 1)
        {
            hudMultiplierText.text = "x" + multiplier;
            hudMultiplierText.gameObject.SetActive(true);
        }
        else
        {
            hudMultiplierText.gameObject.SetActive(false);
        }
    }

    public void TriggerMultiplierJuice(int multiplier)
    {
        hudMultiplierText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f).SetUpdate(true);
        if (multiplier >= 3) hudMultiplierText.color = Color.yellow;
        if (multiplier >= 5) hudMultiplierText.color = Color.red;
    }

    public void ShowGameOver()
    {
        int score = ScoreManager.Instance.currentScore;
        int best = PlayerPrefs.GetInt("HighScore", 0);

        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("HighScore", best);
        }

        Time.timeScale = 0f;
        gameOverCanvasGroup.gameObject.SetActive(true);
        gameOverCanvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
        gameOverCanvasGroup.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true).SetEase(Ease.OutBack);

        AnimateScoreText(score, best);
    }

    private void AnimateScoreText(int targetScore, int targetBest)
    {
        int tempScore = 0;
        int tempBest = 0;

        DOTween.To(() => tempScore, x => tempScore = x, targetScore, 1.5f)
            .OnUpdate(() => finalScoreText.text = "SCORE: " + tempScore)
            .SetEase(Ease.OutQuad).SetUpdate(true);

        DOTween.To(() => tempBest, x => tempBest = x, targetBest, 1.5f)
            .OnUpdate(() => highScoreText.text = "BEST: " + tempBest)
            .SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}