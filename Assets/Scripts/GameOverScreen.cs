using UnityEngine;
using TMPro; // Importa el namespace de TextMeshPro
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI pointsText; // Cambia Text por TextMeshProUGUI
    
    private void Start()
    {
        // Aseguramos que la pantalla esté desactivada al iniciar
        gameObject.SetActive(false);
    }
    
    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = score.ToString();
    }
    
    // Método para el botón de reinicio
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Método para el botón de menú principal (si lo necesitas)
    public void MainMenuButton()
    {
        // Cambiar "MainMenu" por el nombre de tu escena de menú principal
        SceneManager.LoadScene("MainMenu");
    }
}