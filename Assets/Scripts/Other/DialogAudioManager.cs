using System;
using UnityEngine;

public class DialogAudioManager : MonoBehaviour
{
    public Audio[] AudioArray;
    public static DialogAudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        foreach (var s in AudioArray)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.spatialBlend = 0.5f;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.Loop;
            s.source.pitch = s.pitch;
        }
    }
    public void Play(string name)
    {
        Audio s = Array.Find(AudioArray, AudioArray => AudioArray.name == name);
        s.source.Play();
    }
}
