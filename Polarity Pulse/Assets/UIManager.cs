using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Menu Canvas Groups")]
    public CanvasGroup mainMenuCanvas;   // The Home Page
    public CanvasGroup pauseMenuCanvas;  // The Pause Menu
    public CanvasGroup hudCanvas;        // HUD (Score, Multiplier, Pause Button)

    [Header("HUD (Gameplay UI)")]
    public TextMeshProUGUI hudScoreText;
    public TextMeshProUGUI hudMultiplierText;

    [Header("Game Over UI")]
    public CanvasGroup gameOverCanvasGroup;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Color Unlock UI")]
    public GameObject unlockPanel;
    public Image unlockDisplayImage;
    public Sprite[] colorUnlockSprites;

    [Header("Pause Menu UI")]
    public TextMeshProUGUI pauseMenuScoreText;
    public TextMeshProUGUI pauseMenuBestScoreText; // Add this and drag the "Best Score" TMP object here

    [Header("Sub-Menu Panels")]
    public GameObject settingsPanel;
    public GameObject howToPlayPanel;

    private bool isPaused = false;

    void Awake() { Instance = this; }

    void Start()
    {
        // On mobile, we start at the Home Page
        ShowMainMenu();
    }

    public void StartGame()
    {
        mainMenuCanvas.DOFade(0f, 0.4f).SetUpdate(true).OnComplete(() => {
            mainMenuCanvas.gameObject.SetActive(false);
            // Important: Ensure the CanvasGroup no longer blocks clicks
            mainMenuCanvas.blocksRaycasts = false;
            mainMenuCanvas.interactable = false;

            hudCanvas.gameObject.SetActive(true);
            hudCanvas.blocksRaycasts = true;
            Time.timeScale = 1f;
        });
    }

    // --- NEW MOBILE NAVIGATION LOGIC ---

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
        mainMenuCanvas.gameObject.SetActive(true);
        mainMenuCanvas.alpha = 1f;

        hudCanvas.gameObject.SetActive(false);
        pauseMenuCanvas.gameObject.SetActive(false);
        gameOverCanvasGroup.gameObject.SetActive(false);
        unlockPanel.SetActive(false);
    }



    // This function will be linked to your on-screen Pause Button
    public void TogglePause()
    {
        if (gameOverCanvasGroup.gameObject.activeSelf) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            // 1. Get current score and saved high score
            int currentScore = ScoreManager.Instance.currentScore;
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            // 2. Update the pause menu texts
            if (pauseMenuScoreText != null)
                pauseMenuScoreText.text = "SCORE: " + currentScore.ToString();

            if (pauseMenuBestScoreText != null)
                pauseMenuBestScoreText.text = "BEST SCORE: " + highScore.ToString();

            Time.timeScale = 0f;
            pauseMenuCanvas.gameObject.SetActive(true);
            pauseMenuCanvas.alpha = 0f;
            pauseMenuCanvas.DOFade(1f, 0.2f).SetUpdate(true);
        }
        else
        {
            pauseMenuCanvas.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() => {
                pauseMenuCanvas.gameObject.SetActive(false);
                Time.timeScale = 1f;
            });
        }
    }

    public void ResumeGame()
    {
        isPaused = false; // Reset the pause state tracking

        // Smoothly fade out the Pause Menu
        pauseMenuCanvas.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            pauseMenuCanvas.gameObject.SetActive(false); // Hide the panel
            Time.timeScale = 1f; // Resume physics and game time
        });
    }

    public void GoToHome()
    {
        // Reloading the scene brings us back to the Start() logic (Main Menu)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- RETAINED GAMEPLAY LOGIC ---

    public void ShowColorUnlockPrompt(int index)
    {
        if (index >= colorUnlockSprites.Length) return;
        unlockDisplayImage.sprite = colorUnlockSprites[index];
        unlockPanel.SetActive(true);
        unlockPanel.transform.localScale = Vector3.zero;
        Time.timeScale = 0f;

        Sequence unlockSequence = DOTween.Sequence().SetUpdate(true);
        unlockSequence.Append(unlockPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        unlockSequence.Append(unlockPanel.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2, 0.5f));
        unlockSequence.AppendInterval(1.2f);
        unlockSequence.Append(unlockPanel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        unlockSequence.OnComplete(() => {
            Time.timeScale = 1f;
            unlockPanel.SetActive(false);
        });
    }

    public void UpdateHUD(int score, int multiplier)
    {
        hudScoreText.text = "SCORE: " + score.ToString();
        hudMultiplierText.text = "X" + multiplier;
        hudMultiplierText.gameObject.SetActive(multiplier > 1);
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
            .OnUpdate(() => finalScoreText.text = "SCORE: " + tempScore).SetEase(Ease.OutQuad).SetUpdate(true);
        DOTween.To(() => tempBest, x => tempBest = x, targetBest, 1.5f)
            .OnUpdate(() => highScoreText.text = "BEST: " + tempBest).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void RestartGame()
    {
        GameObject[] pulses = GameObject.FindGameObjectsWithTag("Pulse");
        foreach (GameObject p in pulses) Destroy(p);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- SETTINGS ---
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        // Optional: Use DOTween to "pop" it in
        settingsPanel.transform.localScale = Vector3.zero;
        settingsPanel.transform.DOScale(Vector3.one, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    public void CloseSettings()
    {
        settingsPanel.transform.DOScale(Vector3.zero, 0.2f).SetUpdate(true).OnComplete(() => {
            settingsPanel.SetActive(false);
        });
    }

    // --- HOW TO PLAY ---
    public void OpenHowToPlay()
    {
        howToPlayPanel.SetActive(true);
        howToPlayPanel.transform.localScale = Vector3.zero;
        howToPlayPanel.transform.DOScale(Vector3.one, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.transform.DOScale(Vector3.zero, 0.2f).SetUpdate(true).OnComplete(() => {
            howToPlayPanel.SetActive(false);
        });
    }

    // --- EXIT GAME ---
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit(); // This works in the final build
    }
}