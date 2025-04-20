using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------Audio Source------")]
    [SerializeField] private AudioSource musicSource;

    [Header("------Audio Clip------")]
    public AudioClip background;
    
    [Header("------Beat Detection------")]
    public bool useBeatDetection = true;
    [Range(0.05f, 0.5f)]
    public float beatThreshold = 0.2f;

    private BeatDetection beatDetector;

    private void Awake()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
            
        if (background != null && musicSource != null)
        {
            musicSource.clip = background;
            Debug.Log("AudioClip asignado exitosamente: " + background.name);
        }
        else
        {
            Debug.LogError("¡PROBLEMA! AudioClip no está asignado o el AudioSource es nulo");
        }
        
        if (useBeatDetection && GetComponent<BeatDetection>() == null)
        {
            beatDetector = gameObject.AddComponent<BeatDetection>();
            ConfigureBeatDetector();
        }
        else if (useBeatDetection)
        {
            beatDetector = GetComponent<BeatDetection>();
            ConfigureBeatDetector();
        }
    }

    private void Start()
    {
        if (musicSource != null && background != null && musicSource.clip == null)
        {
            musicSource.clip = background;
            Debug.Log("AudioClip asignado en Start: " + background.name);
        }
    }
    
    private void ConfigureBeatDetector()
    {
        if (beatDetector != null)
        {
            beatDetector.threshold = beatThreshold;
            beatDetector.frequencyBand = 1;
            beatDetector.minimumTimeBetweenBeats = 0.2f;
            beatDetector.showDebug = true;
            Debug.Log("BeatDetector configurado con threshold: " + beatThreshold);
        }
    }

    public void PlayMusic()
    {
        if (musicSource == null)
        {
            Debug.LogError("AudioSource es nulo. Buscando en el GameObject...");
            musicSource = GetComponent<AudioSource>();
            
            if (musicSource == null)
            {
                Debug.LogError("No se encontró AudioSource en este GameObject.");
                return;
            }
        }
        
        if (musicSource.clip == null)
        {
            Debug.LogError("No hay clip asignado. Intentando asignar background...");
            
            if (background != null)
            {
                musicSource.clip = background;
                Debug.Log("Clip asignado manualmente: " + background.name);
            }
            else
            {
                Debug.LogError("No hay background disponible para asignar.");
                return;
            }
        }
        
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.Stop();
        musicSource.time = 0;
        musicSource.Play();
        
        Debug.Log("Reproducción forzada: " + musicSource.clip.name);
        Debug.Log("isPlaying después de Play(): " + musicSource.isPlaying);
        Debug.Log("time después de Play(): " + musicSource.time);
        
        StartCoroutine(VerificarReproduccion());
    }

    private System.Collections.IEnumerator VerificarReproduccion()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (musicSource != null)
        {
            Debug.Log("Verificación después de 0.5s - isPlaying: " + musicSource.isPlaying);
            Debug.Log("Verificación después de 0.5s - time: " + musicSource.time);
            
            if (!musicSource.isPlaying)
            {
                Debug.LogError("¡La música no está reproduciendo después de 0.5s! Intentando forzar nuevamente...");
                musicSource.Stop();
                musicSource.time = 0;
                musicSource.Play();
            }
        }
    }
}