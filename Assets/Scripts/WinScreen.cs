using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    private void Start()
    {
        // Aseguramos que la pantalla esté desactivada al iniciar
        gameObject.SetActive(false);
    }
    
    public void Setup(int score)
    {
        gameObject.SetActive(true);
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
    
    // Método para el botón de menú
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}