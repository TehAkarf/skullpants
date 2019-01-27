using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    AudioSource[] audioSources;
    float volume = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Load all audio sources
        audioSources = gameObject.GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Equals))) // Raise volume in 0.1 increments
        {
            if (volume < 1.0f)
            {
                volume += 0.1f;
            }
            else
            {
                volume = 1.0f;
            }
            ChangeVolume(volume);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.Minus)) // Lower volume in 0.1 increments
        {
            if (volume > 0.0f)
            {
                volume -= 0.1f;
            }
            else
            {
                volume = 0.0f;
            }
            ChangeVolume(volume);
        }
    }

    void PlaySound(int value)
    {
        // 0 = walking, 1 = cat's meow, 2 = jump
        switch (value)
        {
            case 0: // Play walking SFX
                audioSources[0].Play();
                break;
            case 1: // Play cat's meow SFX
                audioSources[1].Play();
                break;
            case 2: // Play jumping SFX
                audioSources[2].Play();
                break;
            default: // Play Nothing... for you
                break;
        }
    }

    void ChangeVolume(float value)
    {
        // Change volume for all music
        for (int i = 0; i < audioSources.Length - 1; i++)
        {
            audioSources[i].volume = value;
        }
    }
}
