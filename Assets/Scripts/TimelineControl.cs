using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineControl : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector = null;
    [SerializeField] private Image playImage;
    [SerializeField] private Image rewindImage;
    [SerializeField] private Image pauseImage;
    private bool isPlaying;
 
    
    // Start is called before the first frame update
    void Start()
    {
        playableDirector.time = 0;
        isPlaying = false;
        SetImage(true, playImage);
        SetImage(false, pauseImage);
        SetImage(false, rewindImage);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Time"))
        {
            playableDirector.Resume();
            isPlaying = true;
            SetImage(true,playImage);
            SetImage(false, pauseImage);
            SetImage(false, rewindImage);
        }
        if (Input.GetButtonDown("Rewind"))
        {
            playableDirector.Resume();
            isPlaying = true;
            SetImage(false, playImage);
            SetImage(false, pauseImage);
            SetImage(true, rewindImage);
        }
        double timeDifference = Time.deltaTime;
        if (Input.GetButton("Rewind"))
        {
            if(playableDirector.time > timeDifference)
                playableDirector.time -= timeDifference;
        }
        if (Input.GetButtonUp("Rewind"))
        {
            playableDirector.Pause();
            isPlaying = false;
            SetImage(false, playImage);
            SetImage(true, pauseImage);
            SetImage(false, rewindImage);
        }
    }

    private void SetImage(bool v, Image img)
    {
        var Color = img.color;
        Color.a = v ? 1f : .3f;
        img.color = Color;
    }

    public void Play()
    {
        playableDirector.Play();
        if (!isPlaying)
            playableDirector.Pause();
    }
}
