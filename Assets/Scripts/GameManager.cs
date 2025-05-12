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
    public LevelSelectMenu levelMenu;
    
    [Header("Beat Settings")]
    public float bpm = 120f;
    private float beatInterval;
    
    [Header("Game Settings")]
    public float initialGracePeriod = 5f;
    public TextMeshProUGUI countdownText;
    
    // Estado del juego
    private bool isGameRunning = false;
    
    // Corrutina activa
    private Coroutine obstacleCoroutine = null;

    void Start()
    {
        beatInterval = 60f / bpm;
        
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        
        Debug.Log("GameManager iniciado - beatInterval: " + beatInterval);
    }
    
    // Llamado desde botones o LevelSelectMenu
    public void GameStart()
    {
        // Limpiar el estado anterior
        StopAllCoroutines();
        if (obstacleCoroutine != null)
        {
            StopCoroutine(obstacleCoroutine);
            obstacleCoroutine = null;
        }
        
        // Eliminar obstáculos existentes
        GameObject[] existingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obj in existingObstacles)
        {
            Destroy(obj);
        }
        
        // Configurar estado inicial
        player.SetActive(true);
        playButton.SetActive(false);
        score = 0;
        scoreText.text = "0";
        isGameRunning = false;
        
        // Iniciar con periodo de gracia
        StartCoroutine(StartWithCountdown());
        
        Debug.Log("GameStart llamado - esperando " + initialGracePeriod + " segundos antes de generar obstáculos");
    }
    
    private IEnumerator StartWithCountdown()
    {
        // Mostrar cuenta regresiva
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }
        
        // Esperar período de gracia con cuenta regresiva
        for (int i = Mathf.CeilToInt(initialGracePeriod); i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f);
            Debug.Log("Cuenta regresiva: " + i);
        }
        
        // Mostrar ¡YA!
        if (countdownText != null)
        {
            countdownText.text = "¡YA!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }
        
        // Iniciar el juego
        isGameRunning = true;
        
        // Iniciar puntuación
        InvokeRepeating("ScoreUp", 0f, 1f);
        
        // Iniciar generación de obstáculos con corrutina básica
        obstacleCoroutine = StartCoroutine(SpawnObstaclesSimple());
        
        Debug.Log("Período de gracia terminado - Comenzando a generar obstáculos");
    }
    
    // Método simplificado para generar obstáculos
    private IEnumerator SpawnObstaclesSimple()
    {
        Debug.Log("Iniciando generación de obstáculos con intervalo: " + beatInterval);
        
        // Pequeña pausa inicial
        yield return new WaitForSeconds(0.2f);
        
        // Obstáculo inicial para verificar que funciona
        SpawnSingleObstacle();
        
        while (isGameRunning)
        {
            yield return new WaitForSeconds(beatInterval);
            SpawnSingleObstacle();
        }
    }
    
    // Método para generar un solo obstáculo
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
    
    // Método para actualizar la puntuación
    void ScoreUp()
    {
        if (isGameRunning)
        {
            score++;
            scoreText.text = score.ToString();
        }
    }
    
    // Llamado cuando el jugador pierde
    // Reemplaza el método ShowGameOver en GameManager.cs
    public void ShowGameOver()
    {
        Debug.Log("Game Over - Deteniendo juego");
        
        // Detener el juego
        isGameRunning = false;
        
        // Detener la música
        if (audioManager != null)
        {
            audioManager.StopMusic();
        }
        
        // Detener generación de obstáculos
        if (obstacleCoroutine != null)
        {
            StopCoroutine(obstacleCoroutine);
            obstacleCoroutine = null;
        }
        
        // Detener incremento de puntuación
        CancelInvoke("ScoreUp");
        
        // Ocultar cuenta regresiva si está visible
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        
        // Mostrar pantalla de Game Over
        GameOverScreen.Setup(score);
        
        // Volver al menú después de un tiempo
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
    
    // Método para actualizar el BPM (llamado desde LevelSelectMenu)
    public void UpdateBPM(float newBPM)
    {
        bpm = newBPM;
        beatInterval = 60f / bpm;
        Debug.Log("BPM actualizado a: " + bpm + ", beatInterval: " + beatInterval);
    }
    
    // Método para prueba manual (puedes asignarlo a un botón)
    public void TestSpawnObstacle()
    {
        SpawnSingleObstacle();
    }
}