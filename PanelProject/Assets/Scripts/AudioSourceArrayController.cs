using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using TMPro;

public class AudioSourceArrayController : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    AudioSource audioSource;
    [SerializeField] int audioClipIndex = 0;
    [SerializeField] TextMeshProUGUI songNameText;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[audioClipIndex];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.commaKey.wasPressedThisFrame)
        {
            PlayPreviousSong();
        }
        if (Keyboard.current.periodKey.wasPressedThisFrame)
        {
            PlayNextSong();
        }

        songNameText.text = audioSource.clip.name;
    }

    public void PlayPreviousSong()
    {
        audioSource.clip = GetPreviousSong();
        audioSource.Play();
    }

    public void PlayNextSong()
    {
        audioSource.clip = GetNextSong();
        audioSource.Play();
    }

    private AudioClip GetPreviousSong()
    {
        audioClipIndex = audioClipIndex <= 0 ? audioClips.Count - 1 : audioClipIndex - 1;
        return audioClips[audioClipIndex];
    }

    private AudioClip GetNextSong()
    {
        audioClipIndex = audioClipIndex >= audioClips.Count - 1 ? 0 : audioClipIndex + 1;
        return audioClips[audioClipIndex];
    }
}
