using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    public bool showVisualizer = true;
    public int sampleSize = 512;
    public float heightMultiplier = 100f;
    public float widthMultiplier = 5f;
    public Color barColor = Color.cyan;
    
    [Header("Beat Markers")]
    public bool showBeatMarkers = true;
    public Color beatColor = Color.red;
    public float beatMarkerDuration = 0.2f;
    
    private AudioSource audioSource;
    private float[] spectrumData;
    private bool[] beatActive;
    private float[] beatTimer;
    private BeatDetection beatDetector;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spectrumData = new float[sampleSize];
        beatActive = new bool[8]; 
        beatTimer = new float[8];
        
        beatDetector = GetComponent<BeatDetection>();
        if (beatDetector != null)
        {
            beatDetector.OnBeat.AddListener(OnBeatVisualized);
        }
    }
    
    void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        
        for (int i = 0; i < beatActive.Length; i++)
        {
            if (beatActive[i])
            {
                beatTimer[i] -= Time.deltaTime;
                if (beatTimer[i] <= 0)
                {
                    beatActive[i] = false;
                }
            }
        }
    }
    
    void OnBeatVisualized()
    {
        if (beatDetector != null && beatDetector.frequencyBand < beatActive.Length)
        {
            beatActive[beatDetector.frequencyBand] = true;
            beatTimer[beatDetector.frequencyBand] = beatMarkerDuration;
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showVisualizer || !Application.isPlaying)
            return;
            
        float barWidth = widthMultiplier;
        float xStart = transform.position.x - (sampleSize * barWidth / 2);
        
        for (int i = 0; i < sampleSize; i++)
        {
            if (spectrumData != null && i < spectrumData.Length)
            {
                float height = spectrumData[i] * heightMultiplier;
                float xPos = xStart + (i * barWidth);
                
                int bandIndex = Mathf.Min(i * 8 / sampleSize, 7);
                
                Gizmos.color = (beatActive[bandIndex] && showBeatMarkers) ? beatColor : barColor;
                
                Vector3 pos = new Vector3(xPos, transform.position.y + height / 2, transform.position.z);
                Vector3 size = new Vector3(barWidth * 0.9f, height, 0.1f);
                Gizmos.DrawCube(pos, size);
            }
        }
        
        if (beatDetector != null && showBeatMarkers)
        {
            Gizmos.color = Color.yellow;
            
            int bandStart = Mathf.FloorToInt(sampleSize * beatDetector.frequencyBand / 8f);
            int bandEnd = Mathf.FloorToInt(sampleSize * (beatDetector.frequencyBand + 1) / 8f);
            
            float xStartBand = xStart + (bandStart * barWidth);
            float xEndBand = xStart + (bandEnd * barWidth);
            
            Vector3 thresholdStart = new Vector3(xStartBand, transform.position.y + beatDetector.threshold * heightMultiplier, transform.position.z);
            Vector3 thresholdEnd = new Vector3(xEndBand, transform.position.y + beatDetector.threshold * heightMultiplier, transform.position.z);
            
            Gizmos.DrawLine(thresholdStart, thresholdEnd);
        }
    }
}