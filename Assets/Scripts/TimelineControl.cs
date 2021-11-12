using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    public PlayableDirector playableDirector = null;
    public TimelineAsset timelineAsset;
    
    // Start is called before the first frame update
    void Start()
    {
        playableDirector.time = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            playableDirector.Resume();
        if (Input.GetKeyDown(KeyCode.Return))
            playableDirector.Resume();
        double timeDifference = Time.deltaTime;
        if (Input.GetKey(KeyCode.Return))
        {
            playableDirector.time -= timeDifference;
        }
        if (Input.GetKeyUp(KeyCode.Return))
            playableDirector.Pause();
    }
}
