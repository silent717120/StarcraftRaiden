using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("音效播放器")]
    [SerializeField] AudioSource sFXPlayer;
    [Header("音樂播放器")]
    [SerializeField] AudioData[] MusicAudio;
    [SerializeField] AudioSource MusicSFXPlayer;

    //音高
    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;

    //音量
    float SFX_Volume = 1f;
    float Music_Volume = 1f;

    public void PlayAudioStatic(AudioData audioDat)
    {
        sFXPlayer.pitch = 1;
        sFXPlayer.clip = audioDat.audioClip;
        sFXPlayer.volume = audioDat.volume;
        sFXPlayer.Play();
    }

    public void PlaySFX(AudioData audioDat)
    {
        sFXPlayer.pitch = 1;
        sFXPlayer.PlayOneShot(audioDat.audioClip, audioDat.volume * SFX_Volume);
    }

    //隨機音量效果
    public void PlayRandomSFX(AudioData audioDat)
    {
        sFXPlayer.pitch = Random.Range(MIN_PITCH,MAX_PITCH);
        PlaySFX(audioDat);
    }

    //隨機音量組數
    public void PlayRandomSFX(AudioData[] audioData)
    {
        PlayRandomSFX(audioData[Random.Range(0,audioData.Length)]);
    }

    //隨機音量組數
    public void PlayRandomSFX_OnePitch(AudioData[] audioData)
    {
        PlaySFX(audioData[Random.Range(0, audioData.Length)]);
    }

    public void SetMusic(int MusicNum)
    {
        MusicSFXPlayer.clip = MusicAudio[MusicNum].audioClip;
        MusicSFXPlayer.volume = MusicAudio[MusicNum].volume * Music_Volume;
        MusicSFXPlayer.Play();
    }

    public void SetSFXVolume(float vol) {
       SFX_Volume = vol;
    }

    public void SetMusicVolume(float vol)
    {
        Music_Volume = vol;
        MusicSFXPlayer.volume = Music_Volume;
    }

    public void StopAllAudio()
    {
        sFXPlayer.Stop();
    }
}

//儲存音頻資料
[System.Serializable]public class AudioData
{
    public AudioClip audioClip;

    public float volume;
}
