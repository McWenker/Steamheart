using UnityEngine;
using System.Collections;
public class MusicPlayer : MonoBehaviour
{
    public AudioClip introClip;
    public AudioClip loopClip;
    public AudioSequence sequence { get; private set; }

    void Start()
    {
        sequence = gameObject.AddComponent<AudioSequence>();
        if (introClip == null)
            sequence.Play(loopClip);
        else
            sequence.Play(introClip, loopClip);
        AudioSequenceData data = sequence.GetData(loopClip);
        data.source.loop = true;
    }

    public void AdjustVolume(float val)
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        for(int i = 0; i < sources.Length; ++i)
            sources[i].volume = val;
        PlayerOptions.Volume = val;
    }
}