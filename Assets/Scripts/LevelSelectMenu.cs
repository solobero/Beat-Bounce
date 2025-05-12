using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject gameplayElements;

    public Button[] levelButtons;

    public AudioManager audioManager;
    public GameManager gameManager;

    void Start()
    {
        if (gameplayElements != null)
            gameplayElements.SetActive(false);

        if (menuPanel != null)
            menuPanel.SetActive(true);

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

            int levelIndex = i;
            levelButtons[i].onClick.AddListener(() => StartLevel(levelIndex));
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

        menuPanel.SetActive(false);

        if (gameplayElements != null)
            gameplayElements.SetActive(true);

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

        audioManager.PlayMusic(levelIndex);
        gameManager.GameStart();
    }

    public void ShowMenu()
    {
        if (audioManager != null)
        {
            audioManager.StopMusic();
        }

        if (gameplayElements != null)
            gameplayElements.SetActive(false);

        menuPanel.SetActive(true);
    }
}