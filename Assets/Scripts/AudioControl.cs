using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource revereseMusicSource;
    [SerializeField] private AudioSource reverseEffectSound;
    [SerializeField] private AudioMixerSnapshot defaultAudioMix;
    [SerializeField] private AudioMixerSnapshot freezeSnapshot;
    [SerializeField] private float transitionTime = .05f;
    [SerializeField] private AudioMixer mixer;


    // Start is called before the first frame update
    void Start()
    {
        AudioForward();
    }

    public void AudioForward()
    {
        mixer.SetFloat("ForwardMusicVolume", 0f);
        mixer.SetFloat("BackwardMusicVolume", -80f);
        reverseEffectSound.Stop();
        defaultAudioMix.TransitionTo(transitionTime);
    }

    public void AudioReverse()
    {
        mixer.SetFloat("ForwardMusicVolume", -80f);
        mixer.SetFloat("BackwardMusicVolume", 0f);
        reverseEffectSound.Play();
        defaultAudioMix.TransitionTo(transitionTime);
    }

    public void AudioStop()
    {
        mixer.SetFloat("ForwardMusicVolume", 0f);
        mixer.SetFloat("BackwardMusicVolume", -80f);
        freezeSnapshot.TransitionTo(transitionTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
