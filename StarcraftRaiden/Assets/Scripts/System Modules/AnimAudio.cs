using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAudio : MonoBehaviour
{
    [SerializeField] AudioData ShowAudio;
    [SerializeField] AudioData[] ShowAudios;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayUIAudio()
    {
        AudioManager.Instance.PlaySFX(ShowAudio);
    }
    public void PlayUIAudioRandom()
    {
        AudioManager.Instance.PlayRandomSFX(ShowAudios);
    }
    public void PlayAudioSet(int Num)
    {
        AudioManager.Instance.PlayRandomSFX(ShowAudios[Num]);
    }
}
