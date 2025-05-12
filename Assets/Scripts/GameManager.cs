using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject obstacle;
    public GameOverScreen GameOverScreen;
    public GameObject winScreen;
    public Transform spawnPoint;
    int score = 0;
    public GameObject playButton;
    public TextMeshProUGUI scoreText;
    public GameObject player;
    public AudioManager audioManager;
    public LevelSelectMenu levelMenu;

    [Header("Beat Settings")]
    public float bpm = 120f;
    private float beatInterval;

    [Header("Game Settings")]
    public float initialGracePeriod = 5f;
    public TextMeshProUGUI countdownText;

    private bool isGameRunning = false;
    private Coroutine obstacleCoroutine = null;

    void Start()
    {
        beatInterval = 60f / bpm;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }

        if (audioManager != null)
        {
            audioManager.OnSongComplete += ShowWinScreen;
        }

        Debug.Log("GameManager iniciado - beatInterval: " + beatInterval);
    }

    public void GameStart()
    {
        StopAllCoroutines();
        if (obstacleCoroutine != null)
        {
            StopCoroutine(obstacleCoroutine);
            obstacleCoroutine = null;
        }

        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }

        GameObject[] existingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obj in existingObstacles)
        {
            Destroy(obj);
        }

        player.SetActive(true);
        playButton.SetActive(false);
        score = 0;
        scoreText.text = "0";
        isGameRunning = false;

        if (audioManager != null)
        {
            audioManager.StopSongCompletionCheck();
        }

        StartCoroutine(StartWithCountdown());

        Debug.Log("GameStart llamado - esperando " + initialGracePeriod + " segundos antes de generar obstáculos");
    }

    private IEnumerator StartWithCountdown()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        for (int i = Mathf.CeilToInt(initialGracePeriod); i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f);
            Debug.Log("Cuenta regresiva: " + i);
        }

        if (countdownText != null)
        {
            countdownText.text = "¡YA!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        isGameRunning = true;

        InvokeRepeating("ScoreUp", 0f, 1f);

        obstacleCoroutine = StartCoroutine(SpawnObstaclesSimple());

        Debug.Log("Período de gracia terminado - Comenzando a generar obstáculos");
    }

    private IEnumerator SpawnObstaclesSimple()
    {
        Debug.Log("Iniciando generación de obstáculos con intervalo: " + beatInterval);

        yield return new WaitForSeconds(0.2f);

        SpawnSingleObstacle();

        while (isGameRunning)
        {
            yield return new WaitForSeconds(beatInterval);
            SpawnSingleObstacle();
        }
    }

    private void SpawnSingleObstacle()
    {
        if (isGameRunning && obstacle != null && spawnPoint != null)
        {
            GameObject newObstacle = Instantiate(obstacle, spawnPoint.position, Quaternion.identity);
            Debug.Log("Obstáculo generado en: " + Time.time);
        }
        else
        {
            Debug.LogWarning("No se pudo generar obstáculo - isGameRunning: " + isGameRunning);
        }
    }

    void ScoreUp()
    {
        if (isGameRunning)
        {
            score++;
            scoreText.text = score.ToString();
        }
    }

    private void ShowWinScreen()
    {
        if (isGameRunning)
        {
            Debug.Log("¡Victoria! Canción completada sin chocar.");

            isGameRunning = false;

            if (obstacleCoroutine != null)
            {
                StopCoroutine(obstacleCoroutine);
                obstacleCoroutine = null;
            }

            CancelInvoke("ScoreUp");

            if (winScreen != null)
            {
                WinScreen winScreenComponent = winScreen.GetComponent<WinScreen>();
                if (winScreenComponent != null)
                {
                    winScreenComponent.Setup(score);
                }
                else
                {
                    winScreen.SetActive(true);
                }
            }
        }
    }

    public void ShowGameOver()
    {
        Debug.Log("Game Over - Deteniendo juego");

        isGameRunning = false;

        if (audioManager != null)
        {
            audioManager.StopMusic();
            audioManager.StopSongCompletionCheck();
        }

        if (obstacleCoroutine != null)
        {
            StopCoroutine(obstacleCoroutine);
            obstacleCoroutine = null;
        }

        CancelInvoke("ScoreUp");

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        GameOverScreen.Setup(score);

        StartCoroutine(ReturnToMenuAfterDelay(3f));
    }

    IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (levelMenu != null)
        {
            levelMenu.ShowMenu();
        }
    }

    public void UpdateBPM(float newBPM)
    {
        bpm = newBPM;
        beatInterval = 60f / bpm;
        Debug.Log("BPM actualizado a: " + bpm + ", beatInterval: " + beatInterval);
    }

    public void TestSpawnObstacle()
    {
        SpawnSingleObstacle();
    }

    private void OnDestroy()
    {
        if (audioManager != null)
        {
            audioManager.OnSongComplete -= ShowWinScreen;
        }
    }
}
