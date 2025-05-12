using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectMenu : MonoBehaviour
{
    public GameObject menuPanel;                 // El panel que contiene todo el menú
    public GameObject gameplayElements;          // Un objeto vacío que contiene todos los elementos de juego
    
    public Button[] levelButtons;                // Botones de niveles
    
    public AudioManager audioManager;            // Referencia al AudioManager
    public GameManager gameManager;              // Referencia al GameManager
    
    void Start()
    {
        // Inicialmente ocultar los elementos de juego
        if (gameplayElements != null)
            gameplayElements.SetActive(false);
            
        // Mostrar el menú
        if (menuPanel != null)
            menuPanel.SetActive(true);
        
        // Verificar que tenemos las referencias necesarias
        if (audioManager == null)
        {
            Debug.LogError("AudioManager no asignado en LevelSelectMenu");
            audioManager = FindObjectOfType<AudioManager>();
        }
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager no asignado en LevelSelectMenu");
            gameManager = FindObjectOfType<GameManager>();
        }
        
        // Configurar los botones
        ConfigureButtons();
    }
    
    private void ConfigureButtons()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogError("No hay botones de nivel configurados en LevelSelectMenu");
            return;
        }
        
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null)
            {
                Debug.LogError("El botón de nivel " + i + " es nulo");
                continue;
            }
            
            int levelIndex = i; // Importante para la captura en la lambda
            levelButtons[i].onClick.AddListener(() => StartLevel(levelIndex));
            
            // Eliminada la parte que modificaba el texto de los botones
        }
    }
    
    public void StartLevel(int levelIndex)
    {
        Debug.Log("Iniciando nivel: " + levelIndex);
        
        if (audioManager == null || gameManager == null)
        {
            Debug.LogError("AudioManager o GameManager no disponibles");
            return;
        }
        
        // Ocultar el menú
        menuPanel.SetActive(false);
        
        // Mostrar elementos de juego
        gameplayElements.SetActive(true);
        
        // Configurar el juego para el nivel seleccionado
        // Definir BPM según el nivel
        if (audioManager.trackBPMs != null && levelIndex < audioManager.trackBPMs.Length)
        {
            float newBPM = audioManager.trackBPMs[levelIndex];
            gameManager.UpdateBPM(newBPM);
            Debug.Log("Nivel " + levelIndex + " - BPM configurado a: " + newBPM);
        }
        else
        {
            Debug.LogWarning("No se pudo obtener BPM para el nivel " + levelIndex);
        }
        
        // Iniciar la canción primero para evitar retardos
        audioManager.PlayMusic(levelIndex);
        
        // Luego iniciar el juego
        gameManager.GameStart();
    }
    
    public void ShowMenu()
    {
        // Detener la música si está sonando
        if (audioManager != null)
        {
            audioManager.StopMusic();
        }
        
        // Ocultar elementos de juego
        if (gameplayElements != null)
            gameplayElements.SetActive(false);
            
        // Mostrar el menú
        menuPanel.SetActive(true);
    }
}