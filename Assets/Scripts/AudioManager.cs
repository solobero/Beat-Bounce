using UnityEngine;
using System.Collections;
using System;

public class AudioManager : MonoBehaviour
{
    [Header("------Audio Source------")]
    [SerializeField] private AudioSource musicSource;

    [Header("------Audio Clips------")]
    public AudioClip[] musicTracks;
    public float[] trackBPMs;
    public int currentTrackIndex = 0;

    [Header("------Beat Detection------")]
    public bool useBeatDetection = true;
    [Range(0.05f, 0.5f)]
    public float beatThreshold = 0.2f;

    public float[] songDurations = { 70f, 70f, 70f, 70f };

    private BeatDetection beatDetector;

    public event Action OnSongComplete;

    private Coroutine timerCoroutine;

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

        if (songDurations == null || songDurations.Length == 0)
        {
            Debug.LogWarning("No hay duraciones de canciones configuradas. Usando valor predeterminado de 70 segundos.");
            songDurations = new float[] { 70f, 70f, 70f, 70f };
        }
        else if (songDurations.Length != musicTracks.Length)
        {
            Debug.LogWarning("El número de duraciones configuradas no coincide con el número de pistas de audio. Ajustando...");
            float[] tempDurations = new float[musicTracks.Length];
            for (int i = 0; i < musicTracks.Length; i++)
            {
                if (i < songDurations.Length)
                    tempDurations[i] = songDurations[i];
                else
                    tempDurations[i] = 70f;
            }
            songDurations = tempDurations;
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
        PlayMusic(currentTrackIndex);
    }

    public void PlayMusic(int trackIndex)
    {
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
            currentTrackIndex = 0;
        }

        StopSongCompletionCheck();

        AudioClip trackToPlay = musicTracks[currentTrackIndex];
        if (trackToPlay == null)
        {
            Debug.LogError("La pista en el índice " + currentTrackIndex + " es nula.");
            return;
        }

        musicSource.clip = trackToPlay;
        musicSource.loop = true;
        musicSource.Stop();
        musicSource.time = 0;
        musicSource.Play();

        if (currentTrackIndex < songDurations.Length)
        {
            float duration = songDurations[currentTrackIndex];
            timerCoroutine = StartCoroutine(SongTimer(duration));
            Debug.Log("Iniciando temporizador para la canción " + currentTrackIndex + " con duración: " + duration + " segundos");
        }

        Debug.Log("Reproduciendo pista: " + trackToPlay.name + " (índice: " + currentTrackIndex + ")");

        ConfigureBeatDetectorForTrack(currentTrackIndex);

        StartCoroutine(VerificarReproduccion());
    }

    private IEnumerator SongTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        Debug.Log("Temporizador de canción completado después de " + duration + " segundos");

        if (OnSongComplete != null)
        {
            OnSongComplete();
        }
    }

    private void ConfigureBeatDetectorForTrack(int trackIndex)
    {
        if (beatDetector == null || trackBPMs == null || trackIndex < 0 || trackIndex >= trackBPMs.Length)
            return;

        float bpm = trackBPMs[trackIndex];
        beatDetector.minimumTimeBetweenBeats = 60f / bpm * 0.5f;

        switch (trackIndex)
        {
            case 0:
                beatDetector.threshold = 0.15f;
                beatDetector.frequencyBand = 1;
                break;
            case 1:
                beatDetector.threshold = 0.2f;
                beatDetector.frequencyBand = 2;
                break;
            case 2:
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

    public float GetCurrentBPM()
    {
        if (trackBPMs != null && currentTrackIndex < trackBPMs.Length)
        {
            return trackBPMs[currentTrackIndex];
        }
        return 120f;
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

    public void StopSongCompletionCheck()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            Debug.Log("Temporizador de canción detenido");
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            StopSongCompletionCheck();
            Debug.Log("Música detenida");
        }
    }
}
