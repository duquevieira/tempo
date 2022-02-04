using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineHandler : MonoBehaviour
{

    [SerializeField] TimelineControl[] timelineControllers;
    // Start is called before the first frame update
    void Start()
    {
        Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        foreach (TimelineControl controller in timelineControllers)
        {
            controller.Pause();
        }
    }

    public bool IsPaused()
    {
        foreach (TimelineControl controller in timelineControllers)
        {
            if (controller.IsPaused()) return true;
        }
        return false;
    }

    public bool CanRewind()
    {
        foreach (TimelineControl controller in timelineControllers)
        {
            if (controller.CanRewind()) return true;
        }
        return false;
    }
    public bool CanPlay()
    {
        foreach (TimelineControl controller in timelineControllers)
        {
            if (controller.CanPlay()) return true;
        }
        return false;
    }

    public bool Rewind()
    {
        bool result = false;
        foreach (TimelineControl controller in timelineControllers)
        {
            if (!controller.IsRewinding() || controller.IsPaused())
            {
                controller.Rewind();
                controller.IgnoreTime(true);
                result = true;
            }
        }
        return result;
    }
    public bool Play()
    {
        bool result = false;
        foreach (TimelineControl controller in timelineControllers)
        {
            if (controller.IsRewinding() || controller.IsPaused())
            {
                controller.Play();
                controller.IgnoreTime(true);
                result = true;
            }
        }
        return result;
    }

    public void Fast()
    {
        foreach (TimelineControl controller in timelineControllers)
        {
            controller.Fast();
            controller.IgnoreTime(false);
        }
    }
}
