using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct Audio
{
    public string name;
    public AudioClip clip;
}

public class GunAudio
{
    [SerializeField] public AudioSource gunAudioSource;
    
    private Dictionary<string, AudioClip> loadedAudio;

    public void Init(Gun _gun)
    {
        loadedAudio = new Dictionary<string, AudioClip>();
        SetAudioSource(_gun.GetComponent<AudioSource>());
        
        foreach (Audio audioData in _gun.registerdAudio)
        {
            loadedAudio[audioData.name] = audioData.clip;
        }
    }

    public void SetAudioSource(AudioSource source)
    {
        gunAudioSource = source;
    }
    
    public void PlaySound(string soundName)
    {
        if (loadedAudio == null)
        {
            Debug.LogWarning("loadedAudio dictionary not initialized!");
            return;
        }

        if (loadedAudio.TryGetValue(soundName, out AudioClip audioClip) && audioClip != null)
        {
            gunAudioSource.clip = audioClip;
            gunAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in loadedAudio.");
        }
    }
}

