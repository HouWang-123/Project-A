using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : Singleton<AudioMixerManager>
{
    public AudioMixer _audioMixer;
    
    public void ChangeMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", volume);
    }
    public void ChangeSoundVolume(float volume)
    {
        _audioMixer.SetFloat("SoundFxVolume", volume);
    }
    public void ChangeMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", volume);
    }
    public void ChangeMusicLowcutFreq(float freq)
    {
        _audioMixer.SetFloat("MusicLowcutFrequency", freq);
    }
}
