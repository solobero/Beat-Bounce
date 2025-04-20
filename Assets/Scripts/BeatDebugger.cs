using UnityEngine;
using System.Collections;

public class BeatDebugger : MonoBehaviour
{
    private BeatDetection beatDetection;
    private AudioManager audioManager;
    private GameManager gameManager;
    
    void Start()
    {

        beatDetection = FindObjectOfType<BeatDetection>();
        audioManager = FindObjectOfType<AudioManager>();
        gameManager = FindObjectOfType<GameManager>();
        
        Debug.Log("=== DIAGNÓSTICO DE SISTEMA DE BEATS ===");
        Debug.Log("BeatDetection encontrado: " + (beatDetection != null));
        Debug.Log("AudioManager encontrado: " + (audioManager != null));
        Debug.Log("GameManager encontrado: " + (gameManager != null));
        
        if (audioManager != null)
        {
            AudioSource audioSource = audioManager.GetComponent<AudioSource>();
            Debug.Log("AudioSource encontrado: " + (audioSource != null));
            if (audioSource != null)
            {
                Debug.Log("AudioClip asignado: " + (audioSource.clip != null));
                Debug.Log("Loop activado: " + audioSource.loop);
                Debug.Log("Volumen: " + audioSource.volume);
                Debug.Log("Mute: " + audioSource.mute);
            }
        }
        
        if (gameManager != null)
        {
            Debug.Log("useRealTimeBeatDetection: " + gameManager.useRealTimeBeatDetection);
            Debug.Log("Obstáculo prefab asignado: " + (gameManager.obstacle != null));
            Debug.Log("Punto de spawn asignado: " + (gameManager.spawnPoint != null));
        }
        
        if (beatDetection != null)
        {
            Debug.Log("Umbral de beat: " + beatDetection.threshold);
            Debug.Log("Banda de frecuencia: " + beatDetection.frequencyBand);
            
            beatDetection.OnBeat.AddListener(OnBeatTest);
        }
        
        StartCoroutine(DelayedTest());
    }
    
    IEnumerator DelayedTest()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("=== PRUEBA DESPUÉS DE 5 SEGUNDOS ===");
        
        if (beatDetection != null)
        {
            Debug.Log("Beats detectados hasta ahora: " + beatDetection.beatsDetected);
            Debug.Log("Valor actual del espectro: " + beatDetection.currentSpectrum);
        }
        
        if (audioManager != null)
        {
            AudioSource audioSource = audioManager.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log("¿Audio reproduciendo? " + audioSource.isPlaying);
                Debug.Log("Tiempo de reproducción: " + audioSource.time);
            }
        }
    }
    
    void OnBeatTest()
    {
        Debug.Log("¡BEAT DETECTADO! Tiempo: " + Time.time);
    }
}