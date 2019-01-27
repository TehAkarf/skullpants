using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManagerScript : MonoBehaviour
{
    AudioSource[] audioSources;
    float volume = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Load all audio sources
        audioSources = gameObject.GetComponents<AudioSource>();
        PlayMusic(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.Equals)) // Raise volume in 0.1 increments
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
        if (Input.GetKeyUp(KeyCode.Minus)) // Lower volume in 0.1 increments
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

    void PlayMusic(int value)
    {
        // 0 = Dream House, 1 = Carnival, 2 = BrokenDreams, 3 = Home
        switch (value)
        {
            case 0: // Play Dream House
                audioSources[0].Play();
                break;
            case 1: // Play Carnival
                audioSources[1].Play();
                break;
            case 2: // Play BrokeDreams
                audioSources[2].Play();
                break;
            case 3: // Play Home
                audioSources[3].Play();
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
