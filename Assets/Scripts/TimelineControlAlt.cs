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

public class TimelineControlAlt : MonoBehaviour
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
    private bool isPaused = false;
    public bool isRewinding = false;


    // Start is called before the first frame update
    public void Pause()
    {
        isPaused = true;
        playableDirector.Pause();
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
        SetImage(false, playImage);
        SetImage(false, pauseImage);
        SetImage(true, rewindImage);
        DOVirtual.Float(normalVolume.weight, 0, transitionDuration, normalVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        DOVirtual.Float(pauseVolume.weight, 0, transitionDuration, pauseVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        DOVirtual.Float(rewindVolume.weight, 1, transitionDuration, rewindVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
        audioController.AudioReverse();
    }

    public void setRewind(bool value)
    {
        isRewinding = value;
        if (!isRewinding)
        {
            playableDirector.Resume();
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
        int j = Mathf.FloorToInt((float)(playableDirector.duration / 0.033)) + 1;
        positions = new Vector3[dynamicObjects.Length,j];
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
        if(isRewinding)
        {
            double timeDifference = Time.deltaTime;
            if (playableDirector.time > timeDifference)
                playableDirector.time -= timeDifference;
            int timeIndex = Mathf.FloorToInt((float)(playableDirector.time / 0.033));
            for (int i = 0; i < positions.GetLength(0); i++)
            {
                if (positions[i, timeIndex]!=Vector3.zero)
                    dynamicObjects[i].gameObject.transform.position =  positions[i, timeIndex];
                positions[i, timeIndex+1] = Vector3.zero;
                positions[i, timeIndex] = Vector3.zero;
            }
        }
        else
        {
            int timeIndex = Mathf.FloorToInt((float)(playableDirector.time / 0.033));
            for (int i = 0; i < positions.GetLength(0); i++)
            {
                positions[i,timeIndex] = dynamicObjects[i].gameObject.transform.position;
            }
        }
        if (!isPaused)
        {
            if (!isRewinding)
            {
                playableDirector.Resume();
                SetImage(true, playImage);
                SetImage(false, pauseImage);
                SetImage(false, rewindImage);
                DOVirtual.Float(normalVolume.weight, 1, transitionDuration, normalVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
                DOVirtual.Float(pauseVolume.weight, 0, transitionDuration, pauseVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
                DOVirtual.Float(rewindVolume.weight, 0, transitionDuration, rewindVolumeWeight).SetUpdate(true).SetEase(Ease.InOutSine);
                audioController.AudioForward();
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
}
