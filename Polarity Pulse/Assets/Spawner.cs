using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public GameObject pulsePrefab;
    public Transform pulseHolder;

    [Header("Spawn Settings")]
    public float spawnRate = 1.5f;
    public float spawnRadius = 10f;
    public float pulseSpeed = 5f;

    [Header("Neon Colors")]
    public Color colorA = Color.cyan;
    public Color colorB = Color.magenta;
    public Color colorC = Color.yellow; // Unlocks at 50
    public Color colorD = Color.red;    // Unlocks at 100
    public Color colorE = Color.green;  // Unlocks at 150

    private ScoreManager scoreManager;
    private float nextSpawnTime;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnPulse();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnPulse()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 spawnPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * spawnRadius;

        GameObject newPulse = Instantiate(pulsePrefab, spawnPos, Quaternion.identity);
        if (pulseHolder != null) newPulse.transform.SetParent(pulseHolder);

        int score = (scoreManager != null) ? scoreManager.currentScore : 0;
        Color selectedColor;

        // --- NEW MULTI-COLOR LOGIC ---
        int availableCount = 2;
        if (score >= 150) availableCount = 5;
        else if (score >= 100) availableCount = 4;
        else if (score >= 50) availableCount = 3;

        int choice = Random.Range(0, availableCount);

        switch (choice)
        {
            case 0: selectedColor = colorA; break;
            case 1: selectedColor = colorB; break;
            case 2: selectedColor = colorC; break;
            case 3: selectedColor = colorD; break;
            case 4: selectedColor = colorE; break;
            default: selectedColor = colorA; break;
        }

        PulseController pc = newPulse.GetComponent<PulseController>();
        if (pc != null)
        {
            // We pass 'false' for isColorA since we are now using 
            // direct Color comparison for 3+ colors
            pc.Setup(false, selectedColor, pulseSpeed);
        }
    }
}