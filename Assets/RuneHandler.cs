using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneHandler : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Animator mAnimator;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        mAnimator.SetTrigger("open");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        mAnimator.SetTrigger("close");
    }
}
