using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager sInstance;

    AudioSource[] audioSources;
    float volume = 0.5f;

    public bool isWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        // Load all audio sources
        audioSources = gameObject.GetComponents<AudioSource>();

        if (sInstance == null)
        {
            sInstance = this;
        }
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftBracket)) // Raise volume in 0.1 increments
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
        if (Input.GetKey(KeyCode.RightBracket)) // Lower volume in 0.1 increments
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

    public void PlaySound(int value)
    {
        // 0 = walking, 1 = cat's meow, 2 = jump
        switch (value)
        {
            case 0: // Play walking SFX
                audioSources[0].Play();
                isWalking = true;
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


    public void StopWalkLoop()
    {
        isWalking = false;
        audioSources[0].Stop();
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
