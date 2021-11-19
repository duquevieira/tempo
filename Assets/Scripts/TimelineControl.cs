using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector = null;
    private bool isPlaying;
 
    
    // Start is called before the first frame update
    void Start()
    {
        playableDirector.time = 0;
        isPlaying = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Time"))
        {
            playableDirector.Resume();
            isPlaying = true;
        }
        if (Input.GetButtonDown("Rewind"))
        {
            playableDirector.Resume();
            isPlaying = true;
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
        }
    }

    public void Play()
    {
        playableDirector.Play();
        if (!isPlaying)
            playableDirector.Pause();
    }
}
