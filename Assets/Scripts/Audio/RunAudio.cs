using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAudio : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private bool isRunning;

    [Header("Settings")]
    [SerializeField] private Sound runSound1;
    [SerializeField] private Sound runSound2;

    private void Awake()
    {
        Init(runSound1);
        Init(runSound2);
    }

    private void Init(Sound sound)
    {
        sound.audioSource = gameObject.AddComponent<AudioSource>();
        sound.audioSource.clip = sound.audioClip;

        sound.audioSource.volume = sound.volume;
        sound.audioSource.pitch = sound.pitch;
        sound.audioSource.loop = sound.loop;

        sound.audioSource.outputAudioMixerGroup = sound.audioMixerGroup;
    }

    public void Play()
    {
        // Enable
        isRunning = true;
    }

    private void Update()
    {
        // If enabled
        if (isRunning)
        {
            // If any of the sounds are playing do nothing..
            if (runSound1.audioSource.isPlaying || runSound2.audioSource.isPlaying)
            {
                // Do nothing
            }
            else
            {
                // Randomly choose between the two to play next
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    // Play sound
                    runSound1.audioSource.Play();
                }
                else
                {
                    // Play sound
                    runSound2.audioSource.Play();
                }
            }
        }
    }

    public void Stop()
    {
        // Disable
        isRunning = false;

        // Stop playing
        runSound1.audioSource.Stop();
        runSound2.audioSource.Stop();
    }
}
