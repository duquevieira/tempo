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
    [Header("UI")]
    [SerializeField] private Image playImage;
    [SerializeField] private Image rewindImage;
    [SerializeField] private Image pauseImage;
    [SerializeField] private TextMeshProUGUI rewindText;
    [SerializeField] private TextMeshProUGUI playText;
    [Header("PostFX")]
    [SerializeField] private Volume pauseVolume;
    [SerializeField] private Volume rewindVolume;
    [SerializeField] private Volume normalVolume;
    [Header("Properties")]
    [SerializeField] private float transitionDuration;
    [SerializeField] private AudioControl audioController;
    [SerializeField] private GameObject[] dynamicObjects;
    private Vector3[,] positions;
    private Quaternion[,] rotations;
    private bool isPaused = false;
    private bool isRewinding = false;
    private const double TIMEFACTOR = 0.1;
    [SerializeField] private Material[] standards;
    [SerializeField] private Material[] hoverMaterials;
    [SerializeField] private FloatArray[] hoverranges;
    private List<GameObject> snapimages;
    

    [SerializeField] WatterEffectHandler[] watterhandlers;
    [SerializeField] private ParticleSystemReverseSimulationSuperSimple[] playingParticles;
    [SerializeField] private int[] snapshots;
    [SerializeField] private Material skyDome;
    [SerializeField] private Vector2 originalOffset;
    [SerializeField] private Vector2 skyDomeIncrement;
    [SerializeField] [Range(1,10)] private float fastForwardFactor = 2;
    [SerializeField] Slider slider;
    [SerializeField] GameObject point;
    [SerializeField] RectTransform bar;
    [SerializeField] [Range(0, 1)]  private float tolerence = 1;

    [Serializable]
    public class FloatArray{
        public float[] array;
    }

    private float previousTime;
    private float target;
    private bool fastToggle;
    private bool ignoreTime;

    internal void ResetSlider()
    {
        foreach(GameObject i in snapimages)
        {
            Destroy(i);
        }
        slider.gameObject.SetActive(false);//can not be disabled
    }
    public void InititalizeSlider()
    {
        slider.gameObject.SetActive(true);//can not be anabled if not disabled
        snapimages = new List<GameObject>();
        slider.maxValue = (float)playableDirector.duration;
        slider.minValue = 0;
        slider.value = 0;
        foreach (int i in snapshots)
        {
            GameObject ball = Instantiate(point, bar.gameObject.transform);
            float factor = (float)i / ((float)playableDirector.duration * (float)60);
            ball.transform.position = new Vector3(bar.position.x - (bar.rect.width / 2), bar.position.y, 0);
            ball.transform.Translate(new Vector3(bar.rect.width * factor, 0, 0));
            snapimages.Add(ball);
        }
    }

    // Start is called before the first frame update
    public void SetNoSpeed()
    {
        Play();
    }
    
    public void SetTime(float value)
    {
        Play();
        if (value >= previousTime)
        {
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
            int i = 0;
            HoverUpdate();
            foreach (WatterEffectHandler handler in watterhandlers)
            {
                handler.Toggle(true);
            }
        }
        else
        {
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
            HoverUpdate();
            foreach (WatterEffectHandler handler in watterhandlers)
            {
                handler.Toggle(true);
            }
        }
        previousTime = value;
        playableDirector.time = value;
        Debug.Log(value);
    }


    public void IgnoreTime(bool b)
    {
        ignoreTime = b;
    }

    public void HoverUpdate()
    {
        int i = 0;
        foreach (Material hover in hoverMaterials)
        {
            bool willHover = false;
            if (hoverranges[i] != null)
                for (int k = 0; k < hoverranges[i].array.Length-2; k +=2)
                {
                    if (hoverranges[i - 1].array[k] >= playableDirector.time * 60 && hoverranges[i - 1].array[k + 1] <= playableDirector.time * 60)
                    {
                        willHover = true;
                    }
                }
            if (willHover)
            {
                hover.SetFloat("_isOn", 1.0f);
            }
            i++;
        }
    }

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
        HoverUpdate();
        foreach (WatterEffectHandler handler in watterhandlers)
        {
            handler.Toggle(false);
        }
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
        HoverUpdate();
        foreach (WatterEffectHandler handler in watterhandlers)
        {
            handler.Toggle(true);
        }
    }

    public void Rewind()
    {
        isRewinding = true;
        isPaused = false;
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
        HoverUpdate();
        foreach (WatterEffectHandler handler in watterhandlers)
        {
            handler.Toggle(true);
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
        /*string[] properties = skyDome.GetTexturePropertyNames();
        foreach (string s in properties)
            Debug.Log(s);*/

        skyDome.SetTextureOffset("_MainTex",originalOffset);
        slider.maxValue = (float)playableDirector.duration;
        HoverUpdate();
        snapimages = new List<GameObject>();
        slider.maxValue = (float)playableDirector.duration;
        slider.minValue = 0;
        slider.value = 0;
        foreach (int k in snapshots)
        {
            GameObject ball = Instantiate(point, bar.gameObject.transform);
            float factor = (float)k / ((float)playableDirector.duration * (float)60);
            ball.transform.position = new Vector3(bar.position.x-(bar.rect.width/2), bar.position.y, 0);
            ball.transform.Translate(new Vector3(bar.rect.width*factor,0,0));
            snapimages.Add(ball);
        }

        //Debug.Log(playableDirector.duration);
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

    public void Fast()
    {
        fastToggle = true; 
        float auxMin = float.MaxValue;
        for(int i = 0; i<snapshots.Length;i++)
        {
            int snap = snapshots[i];
            float calc = Math.Abs((float)snap / (float)60 - (float)playableDirector.time);
            if (calc < auxMin)
            {
                auxMin = calc;
                target = (float)snap / (float)60;
            }
        }
        Debug.Log(playableDirector.playableGraph.GetRootPlayable(0).GetSpeed());
        if (target < playableDirector.time)
        {
            Rewind();
            Debug.Log("<");
        }
        else
        {
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(fastForwardFactor);
            Play();
            Debug.Log(">");
            
        }
        Debug.Log(playableDirector.playableGraph.GetRootPlayable(0).GetSpeed());
    }

    // Update is called once per frame
    private void Update()
    {
        double timeDifference = Time.deltaTime;

        float auxTimeFactor = 1;
        if (fastToggle)
            auxTimeFactor = fastForwardFactor;

        if (playableDirector.time <= 0)
        {
            if (isRewinding)
            {
                Pause();
                fastToggle = false;
                //playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            }
        }
        else if (playableDirector.time >= playableDirector.duration)
        {
            if (!isRewinding)
            {
                Pause();
                fastToggle = false;
                //playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            }
        }
        if (!ignoreTime)
        {
            if (playableDirector.time >= target - (timeDifference * auxTimeFactor)*tolerence && playableDirector.time <= target + (timeDifference * auxTimeFactor)*tolerence)
            {
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                Pause();
                fastToggle = false;
                
            }
        }
        if (!isPaused)
        {
            if (isRewinding)
            {
                
                playableDirector.time -= timeDifference*auxTimeFactor;
                   
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
                
                foreach(Material standard in standards)
                {
                    standard.SetFloat("_isOn", standard.GetFloat("_isOn") - (float)timeDifference*auxTimeFactor);
                }
                skyDome.SetTextureOffset("_MainTex", skyDome.GetTextureOffset("_MainTex") - (skyDomeIncrement * (float)timeDifference));
            }
            else
            {
                if (fastToggle)
                {
                    playableDirector.time += timeDifference * auxTimeFactor - timeDifference;
                }
                int timeIndex = Mathf.FloorToInt((float)(playableDirector.time / TIMEFACTOR));
                for (int i = 0; i < positions.GetLength(0); i++)
                {
                    positions[i, timeIndex] = dynamicObjects[i].gameObject.transform.position;
                    rotations[i, timeIndex] = dynamicObjects[i].gameObject.transform.rotation;
                }
                foreach (Material standard in standards)
                {
                    standard.SetFloat("_isOn", standard.GetFloat("_isOn") + (float)timeDifference*auxTimeFactor);
                }
                skyDome.SetTextureOffset("_MainTex", skyDome.GetTextureOffset("_MainTex") + (skyDomeIncrement*(float)timeDifference));

            }
            slider.value = (float)playableDirector.time;
        }
    }

    private void SetImage(bool v, Image img)
    {
        var Color = img.color;
        Color.a = v ? 1f : .3f;
        img.color = Color;
    }

    public bool HasEnded()
    {
        return playableDirector.duration == playableDirector.time || (isRewinding & !isPaused);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public bool IsRewinding()
    {
        return isRewinding;
    }

    public bool CanRewind()
    {
        return playableDirector.time>0;
    }

    public bool CanPlay()
    {
        return playableDirector.time <playableDirector.duration;
    }
}

