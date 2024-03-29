using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name = " ";

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    public bool isLoop = false;

    [HideInInspector]
    public AudioSource source;

    public Sound(string name, AudioClip clip, float volume, float pitch, bool isLoop)
    {
        this.name = name;
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
        this.isLoop = isLoop;
    }
}