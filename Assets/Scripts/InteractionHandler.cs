using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    const int FOOTCACHE = 6;

    [SerializeField] private Image tapImage;
    [SerializeField] private Image walkingImage;
    private int footCount;
    private GameObject[] footSteps;
    [SerializeField] TimelineControl[] timelineControllers;
    [SerializeField] Pathfinding pathfinder;
    [SerializeField] private GameObject footStep;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject clickFX;
    private GameObject clickInstance;

    // Start is called before the first frame update
    void Start()
    {

        SetWalkingImage(false);
        SetTapImage(false);
        footSteps = new GameObject[FOOTCACHE];
        footCount = 0;
        foreach (TimelineControl controller in timelineControllers)
        {
            controller.Pause();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = true;
        bool canRewind = true;
        bool canPlay = true;

        foreach (TimelineControl controller in timelineControllers)
            if (pathfinder.RemainingDistance() <= pathfinder.StoppingDistance())
            {
                if (!controller.IsPaused())
                    isMoving = false;
                if (!controller.CanRewind())
                    canRewind = false;
                if (!controller.CanPlay())
                    canPlay = false;
            }
        if (isMoving)
        {
            if (pathfinder.RemainingDistance() <= pathfinder.StoppingDistance())
            {
                if (pathfinder.PathAmount() > 0)
                {
                    pathfinder.SetNextDestination();
                    if (footCount == footSteps.Length)
                        footCount = 0;
                    if (footSteps[footCount] != null)
                        Destroy(footSteps[footCount]);
                    footSteps[footCount++] = Instantiate(footStep, this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.transform.parent);
                    SetWalkingImage(true);
                }
                else
                {
                    SetWalkingImage(false);
                }

            }

            if (Input.GetButton("Click"))
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (clickInstance != null)
                        Destroy(clickInstance);
                    clickInstance = Instantiate(clickFX, hit.point, Quaternion.identity, this.gameObject.transform.parent);
                    pathfinder.Djikstra(hit);
                    //Debug.Log(pathFound.Count);
                }

            }
        }
        if (Input.GetButton("Click"))
        {
            SetTapImage(true);
        }
        if (Input.GetButtonUp("Click"))
        {
            SetTapImage(false);
        }

        if (canRewind)
        {
            if (Input.GetButtonDown("Rewind"))
            {
                foreach (TimelineControl controller in timelineControllers)
                {
                    controller.Rewind();
                    controller.IgnoreTime(true);
                }
                pathfinder.ClearPath();
                SetWalkingImage(false);
                //Debug.Log("rewind");
            }
            if (Input.GetButtonUp("Rewind"))
            {
                foreach (TimelineControl controller in timelineControllers)
                {
                    controller.Fast();
                    controller.IgnoreTime(false);
                }
            }
        }

        if (canPlay)
        {
            if (Input.GetButtonDown("Time"))
            {
                foreach (TimelineControl controller in timelineControllers)
                {
                    controller.Play();
                    controller.IgnoreTime(true);
                }
                //Debug.Log("play");
                pathfinder.ClearPath();
                SetWalkingImage(false);
            }
            if (Input.GetButtonUp("Time"))
            {
                foreach (TimelineControl controller in timelineControllers)
                {
                    controller.Fast();
                    controller.IgnoreTime(false);
                }
            }
        }

    }

    private void SetWalkingImage(bool v)
    {
        var walkingColor = walkingImage.color;
        walkingColor.a = v ? 1f : .3f;
        walkingImage.color = walkingColor;
    }

    private void SetTapImage(bool v)
    {
        var tapColor = tapImage.color;
        tapColor.a = v ? 1f : .3f;
        tapImage.color = tapColor;
    }
}
