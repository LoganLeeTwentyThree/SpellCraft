using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : Singleton<SoundManager>
{

    private AudioSource audioSource;
    private AudioSource musicSource;
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    public void PlaySound(string clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(audioClips.TryGetValue(clip, out AudioClip audioClip) ? audioClip : null);
        }
    }

    override public void Populate()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        audioSource = sources[0];
        musicSource = sources[1];
        
        musicSource.clip = Resources.Load<AudioClip>("Music/Background");
        musicSource.loop = true;
        musicSource.volume = PlayerPrefs.GetFloat("Master") * PlayerPrefs.GetFloat("Music");
        musicSource.Play();

        audioSource.volume = PlayerPrefs.GetFloat("Master") * PlayerPrefs.GetFloat("SFX");
        audioClips.Clear();
        foreach(AudioClip ac in Resources.LoadAll<AudioClip>("Sounds"))
        {
            audioClips.Add(ac.name, ac);
        }
        
        return;
    }
}
