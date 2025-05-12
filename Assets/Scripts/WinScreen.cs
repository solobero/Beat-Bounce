using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    private void Start()
    {
        gameObject.SetActive(false);
    }
    
    public void Setup(int score)
    {
        gameObject.SetActive(true);
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
    
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}