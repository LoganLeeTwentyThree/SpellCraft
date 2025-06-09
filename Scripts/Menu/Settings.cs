using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider master;
    [SerializeField] private Slider sfx;
    [SerializeField] private Slider music;

    private void Start()
    {
        master.value = PlayerPrefs.HasKey("Master") ? PlayerPrefs.GetFloat("Master") : 0.75f;
        sfx.value = PlayerPrefs.HasKey("SFX") ? PlayerPrefs.GetFloat("SFX") : 1;
        music.value = PlayerPrefs.HasKey("Music") ? PlayerPrefs.GetFloat("Music") : 0.25f;
    }

    public void SetMasterVolume()
    {
        PlayerPrefs.SetFloat("Master", master.value);
    }

    public void SetSFXVolume()
    {
        PlayerPrefs.SetFloat("SFX", sfx.value);
    }

    public void SetMusicVolume()
    {
        PlayerPrefs.SetFloat("Music", music.value);
    }
}
