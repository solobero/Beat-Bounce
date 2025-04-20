using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class BeatDetection : MonoBehaviour
{
    [Header("Beat Detection Settings")]
    public int sampleSize = 1024;
    public int frequencyBand = 0;
    public float threshold = 0.3f;
    public float minimumTimeBetweenBeats = 0.2f;
    
    [Header("Debug")]
    public bool showDebug = true;
    public float currentSpectrum = 0f;
    public int beatsDetected = 0;
    
    [Header("Events")]
    public UnityEvent OnBeat;
    
    private AudioSource audioSource;
    private float[] spectrumData;
    private float prevSpectrumValue = 0f;
    private float timeLastBeat = 0f;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spectrumData = new float[sampleSize];
        
        if (OnBeat == null)
            OnBeat = new UnityEvent();
    }
    
    void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        
        float bandValue = 0f;
        int bandStart = Mathf.FloorToInt(sampleSize * frequencyBand / 8f);
        int bandEnd = Mathf.FloorToInt(sampleSize * (frequencyBand + 1) / 8f);
        
        for (int i = bandStart; i < bandEnd; i++)
        {
            bandValue += spectrumData[i];
        }
        
        bandValue /= (bandEnd - bandStart);
        currentSpectrum = bandValue;
        
        if (bandValue > threshold && bandValue > prevSpectrumValue && Time.time > timeLastBeat + minimumTimeBetweenBeats)
        {
            OnBeatDetected();
            timeLastBeat = Time.time;
        }
        
        prevSpectrumValue = bandValue;
    }
    
    void OnBeatDetected()
    {
        beatsDetected++;
        if (showDebug)
            Debug.Log($"Beat detected! Total: {beatsDetected}");
        
        OnBeat.Invoke();
    }
}