using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("------Audio Source------")]
    [SerializeField] private AudioSource musicSource;

    [Header("------Audio Clips------")]
    public AudioClip[] musicTracks;  // Arreglo para almacenar todas tus canciones
    public float[] trackBPMs;        // BPM correspondiente a cada canción
    public int currentTrackIndex = 0; // Hacerlo público para facilitar la depuración
    
    [Header("------Beat Detection------")]
    public bool useBeatDetection = true;
    [Range(0.05f, 0.5f)]
    public float beatThreshold = 0.2f;

    private BeatDetection beatDetector;

    private void Awake()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
        
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
        // Verificar que tenemos pistas y BPMs configurados
        if (musicTracks == null || musicTracks.Length == 0)
        {
            Debug.LogError("No hay pistas de audio configuradas en AudioManager");
        }
        
        if (trackBPMs == null || trackBPMs.Length == 0)
        {
            Debug.LogError("No hay BPMs configurados en AudioManager");
        }
        else if (trackBPMs.Length != musicTracks.Length)
        {
            Debug.LogWarning("El número de BPMs configurados no coincide con el número de pistas de audio");
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

    // Método sobrecargado para soportar ambos casos: con y sin parámetro
    public void PlayMusic()
    {
        PlayMusic(currentTrackIndex); // Utiliza el índice actual si no se especifica uno
    }

    public void PlayMusic(int trackIndex)
    {
        // Validar que el índice sea válido
        if (trackIndex >= 0 && trackIndex < musicTracks.Length)
        {
            currentTrackIndex = trackIndex;
        }
        else
        {
            Debug.LogWarning("Índice de pista inválido: " + trackIndex + ". Usando la pista actual: " + currentTrackIndex);
        }
        
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
        
        if (musicTracks.Length == 0)
        {
            Debug.LogError("No hay canciones disponibles.");
            return;
        }
        
        if (currentTrackIndex >= musicTracks.Length)
        {
            Debug.LogError("Índice de pista fuera de rango: " + currentTrackIndex);
            currentTrackIndex = 0; // Usar la primera pista como fallback
        }
        
        // Asignar la canción actual
        AudioClip trackToPlay = musicTracks[currentTrackIndex];
        if (trackToPlay == null)
        {
            Debug.LogError("La pista en el índice " + currentTrackIndex + " es nula.");
            return;
        }
        
        musicSource.clip = trackToPlay;
        
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.Stop();
        musicSource.time = 0;
        musicSource.Play();
        
        Debug.Log("Reproduciendo pista: " + trackToPlay.name + " (índice: " + currentTrackIndex + ")");
        
        // Configurar BeatDetector para esta pista
        ConfigureBeatDetectorForTrack(currentTrackIndex);
        
        StartCoroutine(VerificarReproduccion());
    }

    private void ConfigureBeatDetectorForTrack(int trackIndex)
    {
        if (beatDetector == null || trackBPMs == null || trackIndex < 0 || trackIndex >= trackBPMs.Length)
            return;
        
        float bpm = trackBPMs[trackIndex];
        beatDetector.minimumTimeBetweenBeats = 60f / bpm * 0.5f;
        
        // Configuraciones específicas por pista si es necesario
        switch (trackIndex)
        {
            case 0: // Primera canción
                beatDetector.threshold = 0.15f;
                beatDetector.frequencyBand = 1;
                break;
            case 1: // Segunda canción
                beatDetector.threshold = 0.2f;
                beatDetector.frequencyBand = 2;
                break;
            case 2: // Tercera canción
                beatDetector.threshold = 0.25f;
                beatDetector.frequencyBand = 0;
                break;
            default:
                beatDetector.threshold = beatThreshold;
                beatDetector.frequencyBand = 1;
                break;
        }
        
        Debug.Log($"BeatDetector configurado para pista {trackIndex}: BPM={bpm}, threshold={beatDetector.threshold}, band={beatDetector.frequencyBand}");
    }

    // Método para obtener el BPM actual
    public float GetCurrentBPM()
    {
        if (trackBPMs != null && currentTrackIndex < trackBPMs.Length)
        {
            return trackBPMs[currentTrackIndex];
        }
        return 120f; // BPM predeterminado si no hay información disponible
    }

    private IEnumerator VerificarReproduccion()
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

    // Añade este método al AudioManager.cs
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            Debug.Log("Música detenida");
        }
    }
}