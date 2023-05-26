using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SpinButton : MonoBehaviour
{
    
    public Button spinButton;
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    private AudioSource audioSource1;
    private AudioSource audioSource2;

    private void Awake()
    {
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource1.clip = audioClip1;

        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.clip = audioClip2;
    }

    private void Start()
    {
        spinButton = GetComponent<Button>();
        spinButton.onClick.AddListener(StartSpinning);
        
    }

    private void StartSpinning()
    {
        FindObjectOfType<ImageCycler>().StartSpinning();
        PlayAudioClip1();

    }
    private void PlayAudioClip1()
    {
        audioSource1.Play();
        PlayAudioClip2();
    }
    private void PlayAudioClip2()
    {
        audioSource2.Play(); // Play audio clip 2 immediately
    }
}
