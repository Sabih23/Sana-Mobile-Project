using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = false;
        }
    }

    public void Play(string name)
    {
        Debug.Log("Requesting to play sound: " + name);
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null)
        {
            Debug.Log("Playing sound: " + s.name);  // Log when the sound is actually played
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound not found: " + name);
        }
    }
}
