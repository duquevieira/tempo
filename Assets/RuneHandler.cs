using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneHandler : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Animator mAnimator;
    [SerializeField] Sprite rune;
    [SerializeField] Sprite defaultrune;
    [SerializeField] int index;
    [SerializeField] InteractionHandler handler;
    [SerializeField] TimelineHandler time;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        mAnimator.SetTrigger("open");
        handler.SelectRune(index, rune);
        time.Activate();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        mAnimator.SetTrigger("close");
        handler.SelectRune(-1, defaultrune);
        time.Deactivate();
    }
}
