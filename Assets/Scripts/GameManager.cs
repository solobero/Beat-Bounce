using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject obstacle;
    public GameOverScreen GameOverScreen;
    public Transform spawnPoint;
    int score = 0;
    public GameObject playButton;
    public TextMeshProUGUI scoreText;
    public GameObject player;
    public AudioManager audioManager;
    
    [Header("Beat Settings")]
    public float bpm = 120f;
    private float beatInterval;
    public bool useRealTimeBeatDetection = true;
    
    private BeatDetection beatDetection;
    private bool gameStarted = false;

    void Start()
    {
        beatInterval = 60f / bpm;
        beatDetection = audioManager.GetComponent<BeatDetection>();
        
        if (beatDetection == null && useRealTimeBeatDetection)
        {
            Debug.LogWarning("BeatDetection no encontrado. Agrégalo al mismo GameObject que AudioManager.");
            useRealTimeBeatDetection = false;
        }
    }
    
    public void GameStart()
    {
        player.SetActive(true);
        playButton.SetActive(false);
        gameStarted = true;

        if (useRealTimeBeatDetection)
        {
            beatDetection.OnBeat.AddListener(SpawnObstacleOnBeat);
        }
        else
        {
            StartCoroutine("SpawnObstacles");
        }
        
        InvokeRepeating("ScoreUp", 2f, 1f);
        audioManager.PlayMusic();
    }
    
    public void SpawnObstacleOnBeat()
    {
        if (gameStarted)
        {
            Instantiate(obstacle, spawnPoint.position, Quaternion.identity);
        }
    }
    
    IEnumerator SpawnObstacles()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            Instantiate(obstacle, spawnPoint.position, Quaternion.identity); 
            yield return new WaitForSeconds(beatInterval);
        }
    }

    void ScoreUp()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void ShowGameOver()
    {
        // Detenemos la generación de obstáculos
        gameStarted = false;
        
        // Cancelamos la invocación repetida de ScoreUp
        CancelInvoke("ScoreUp");
        
        // Si estamos usando detección de beat, removemos el listener
        if (useRealTimeBeatDetection)
        {
            beatDetection.OnBeat.RemoveListener(SpawnObstacleOnBeat);
        }
        else
        {
            StopCoroutine("SpawnObstacles");
        }
        
        // Mostramos la pantalla de GameOver con la puntuación actual
        GameOverScreen.Setup(score);
    }
}