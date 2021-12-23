using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class TimelineControl : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector = null;
    [SerializeField] private Image playImage;
    [SerializeField] private Image rewindImage;
    [SerializeField] private Image pauseImage;
    [SerializeField] private TextMeshProUGUI rewindText;
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private Volume pauseVolume;
    [SerializeField] private Volume rewindVolume;
    [SerializeField] private Volume normalVolume;
    [SerializeField] private float transitionDuration;
    [SerializeField] private AudioControl audioController;
    [SerializeField] private GameObject[] dynamicObjects;
    private Vector3[,] positions;
    private Quaternion[,] rotations;
    private bool isPaused = false;
    public bool isRewinding = false;
    private const double TIMEFACTOR = 0.1;
    [SerializeField] private Material[] standards;
    [SerializeField] private ParticleSystemReverseSimulationSuperSimple[] playingParticles;


    // Start is called before the first frame update
    public void Pause()
    {
        isPaused = true;
        playableDirector.Pause();
        foreach (ParticleSystemReverseSimulationSuperSimple playing in playingParticles)
        {
            playing.simulationSpeedScale = 0;
        }
        SetImage(false, playImage);
        SetImage(true, pauseImage);
        SetImage(false, rewindImage);
        DOVirtual.Float(normalVolume.weight, 0, transitionDuration, normalVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        DOVirtual.Float(pauseVolume.weight, 1, transitionDuration, pauseVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        DOVirtual.Float(rewindVolume.weight, 0, transitionDuration, rewindVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        audioController.AudioStop();
    }

    public void Play()
    {
        isPaused = false;
        isRewinding = false;
        playableDirector.Resume();
        foreach (ParticleSystemReverseSimulationSuperSimple playing in playingParticles)
        {
            playing.simulationSpeedScale = -1;
        }
        SetImage(true, playImage);
        SetImage(false, pauseImage);
        SetImage(false, rewindImage);
        DOVirtual.Float(normalVolume.weight, 1, transitionDuration, normalVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        DOVirtual.Float(pauseVolume.weight, 0, transitionDuration, pauseVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        DOVirtual.Float(rewindVolume.weight, 0, transitionDuration, rewindVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        audioController.AudioForward();
    }

    public void setRewind(bool value)
    {
        isRewinding = value;
        if (!isRewinding)
        {
            playableDirector.Resume();
            foreach (ParticleSystemReverseSimulationSuperSimple playing in playingParticles)
            {
                playing.simulationSpeedScale = -1;
            }
            SetImage(true, playImage);
            SetImage(false, pauseImage);
            SetImage(false, rewindImage);
            DOVirtual.Float(normalVolume.weight, 1, transitionDuration, normalVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
            DOVirtual.Float(pauseVolume.weight, 0, transitionDuration, pauseVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
            DOVirtual.Float(rewindVolume.weight, 0, transitionDuration, rewindVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
            audioController.AudioForward();
        }
        else
        {
            playableDirector.Resume();
            foreach (ParticleSystemReverseSimulationSuperSimple playing in playingParticles)
            {
                playing.simulationSpeedScale = 1;
            }
            SetImage(false, playImage);
            SetImage(false, pauseImage);
            SetImage(true, rewindImage);
            DOVirtual.Float(normalVolume.weight, 0, transitionDuration, normalVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
            DOVirtual.Float(pauseVolume.weight, 0, transitionDuration, pauseVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
            DOVirtual.Float(rewindVolume.weight, 1, transitionDuration, rewindVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
            audioController.AudioReverse();
            audioController.AudioToggle();
        }
    }

    void Start()
    {
        playableDirector.time = 0;
        SetImage(true, playImage);
        SetImage(false, pauseImage);
        SetImage(false, rewindImage);
        rewindText.text = "L Shift";
        playText.text = "Space";
        int j = Mathf.FloorToInt((float)(playableDirector.duration / TIMEFACTOR)) + 1;
        positions = new Vector3[dynamicObjects.Length, j];
        rotations = new Quaternion[dynamicObjects.Length, j];
        audioController.AudioForward();
    }

    void normalVolumeWeight(float weight)
    {
        normalVolume.weight = weight;
    }

    void pauseVolumeWeight(float weight)
    {
        pauseVolume.weight = weight;
    }

    void rewindVolumeWeight(float weight)
    {
        rewindVolume.weight = weight;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isPaused)
        {
            double timeDifference = Time.deltaTime;
            if (isRewinding)
            {

                if (playableDirector.time > timeDifference)
                {
                    playableDirector.time -= timeDifference;
                }
                    
                int timeIndex = Mathf.FloorToInt((float)(playableDirector.time / TIMEFACTOR));
                for (int i = 0; i < positions.GetLength(0); i++)
                {
                    if (positions[i, timeIndex] != Vector3.zero)
                    {
                        dynamicObjects[i].gameObject.transform.DOMove(positions[i, timeIndex], (float)TIMEFACTOR);
                        dynamicObjects[i].gameObject.transform.DORotateQuaternion(rotations[i, timeIndex], (float)TIMEFACTOR);
                    }
                    positions[i, timeIndex] = Vector3.zero;
                    rotations[i, timeIndex] = Quaternion.identity;
                }
                if (playableDirector.time < timeDifference)
                {
                    Pause();
                }
                foreach(Material standard in standards)
                {
                    standard.SetFloat("_isOn", standard.GetFloat("_isOn") - (float)timeDifference);
                }
                
            }
            else
            {
                int timeIndex = Mathf.FloorToInt((float)(playableDirector.time / TIMEFACTOR));
                for (int i = 0; i < positions.GetLength(0); i++)
                {
                    positions[i, timeIndex] = dynamicObjects[i].gameObject.transform.position;
                    rotations[i, timeIndex] = dynamicObjects[i].gameObject.transform.rotation;
                }
                foreach (Material standard in standards)
                {
                    standard.SetFloat("_isOn", standard.GetFloat("_isOn") + (float)timeDifference);
                }

            }
        }
    }

    private void SetImage(bool v, Image img)
    {
        var Color = img.color;
        Color.a = v ? 1f : .3f;
        img.color = Color;
    }

    internal void Resume()
    {
        if (isPaused)
        {
            Pause();
        }
    }
    public bool HasEnded()
    {
        return playableDirector.duration == playableDirector.time || (isRewinding & !isPaused);
    }
}
