using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector = null;
 
    
    // Start is called before the first frame update
    void Start()
    {
        playableDirector.time = 0;

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Time"))
            playableDirector.Resume();
        if (Input.GetButtonDown("Rewind"))
            playableDirector.Resume();
        double timeDifference = Time.deltaTime;
        if (Input.GetButton("Rewind"))
        {
            if(playableDirector.time > timeDifference)
                playableDirector.time -= timeDifference;
        }
        if (Input.GetButtonUp("Rewind"))
            playableDirector.Pause();
    }
}
