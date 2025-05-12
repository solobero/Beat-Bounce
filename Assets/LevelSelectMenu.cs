using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectMenu : MonoBehaviour
{
    public GameObject menuPanel;                 // El panel que contiene todo el menú
    public GameObject gameplayElements;          // Un objeto vacío que contiene todos los elementos de juego
    
    public Button[] levelButtons;                // Botones de niveles
    public TextMeshProUGUI[] levelTitles;        // Textos para mostrar nombres de canciones
    
    public AudioManager audioManager;            // Referencia al AudioManager
    public GameManager gameManager;              // Referencia al GameManager
    
    void Start()
    {
        // Configurar los botones
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i; // Importante para la captura en la lambda
            levelButtons[i].onClick.AddListener(() => StartLevel(levelIndex));
            
            // Si tienes referencias a los títulos y el AudioManager está configurado, muestra los nombres de las canciones
            if (levelTitles.Length > i && audioManager != null && audioManager.musicTracks.Length > i)
            {
                levelTitles[i].text = "Nivel " + (i + 1) + "\n" + audioManager.musicTracks[i].name;
            }
        }
        
        // Inicialmente ocultar los elementos de juego
        if (gameplayElements != null)
            gameplayElements.SetActive(false);
            
        // Mostrar el menú
        if (menuPanel != null)
            menuPanel.SetActive(true);
    }
    
    public void StartLevel(int levelIndex)
    {
        Debug.Log("Iniciando nivel: " + levelIndex);
        
        // Ocultar el menú
        menuPanel.SetActive(false);
        
        // Mostrar elementos de juego
        gameplayElements.SetActive(true);
        
        // Configurar el juego para el nivel seleccionado
        if (gameManager != null)
        {
            // Definir BPM según el nivel
            if (audioManager != null && audioManager.trackBPMs.Length > levelIndex)
            {
                gameManager.bpm = audioManager.trackBPMs[levelIndex];
            }
            
            // Iniciar el juego con la canción seleccionada
            audioManager.PlayMusic(levelIndex);
            
            // Llamar al método de inicio del juego
            gameManager.GameStart();
        }
    }
    
    public void ShowMenu()
    {
        // Ocultar elementos de juego
        if (gameplayElements != null)
            gameplayElements.SetActive(false);
            
        // Mostrar el menú
        menuPanel.SetActive(true);
    }
}