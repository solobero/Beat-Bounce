using UnityEngine;
using System.Collections;
using TMPro;

public class StandaloneRhythmSystem : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject obstaclePrefab;
    public Transform spawnPoint;
    public GameObject player;
    public GameObject startButton;
    public TextMeshProUGUI scoreText;
    public AudioSource musicSource;

    [Header("Configuración del ritmo")]
    public float bpm = 120f;
    [Range(1, 4)]
    public int spawnEveryNBeats = 2;
    public bool useRandomVariation = true;
    [Range(0f, 1f)]
    public float randomVariationChance = 0.3f;

    private float beatInterval;
    private int beatCount = 0;
    private int score = 0;
    private bool gameRunning = false;

    void Start()
    {
        beatInterval = 60f / bpm;

        if (player != null)
            player.SetActive(false);
    }

    public void StartGame()
    {
        if (player != null)
            player.SetActive(true);

        if (startButton != null)
            startButton.SetActive(false);

        score = 0;
        if (scoreText != null)
            scoreText.text = "0";
        InvokeRepeating("ScoreUp", 2f, 1f);

        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.Play();
        }

        gameRunning = true;
        StartCoroutine(GenerateObstacles());
    }

    IEnumerator GenerateObstacles()
    {
        yield return new WaitForSeconds(1f);

        while (gameRunning)
        {
            beatCount++;
            bool shouldSpawn = (beatCount % spawnEveryNBeats == 0);

            if (useRandomVariation && Random.value < randomVariationChance)
            {
                shouldSpawn = !shouldSpawn;
            }

            if (shouldSpawn && obstaclePrefab != null && spawnPoint != null)
            {
                Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(beatInterval);
        }
    }

    void ScoreUp()
    {
        if (gameRunning)
        {
            score++;
            if (scoreText != null)
                scoreText.text = score.ToString();
        }
    }

    public void StopGame()
    {
        gameRunning = false;
        CancelInvoke("ScoreUp");
        StopAllCoroutines();

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }
}