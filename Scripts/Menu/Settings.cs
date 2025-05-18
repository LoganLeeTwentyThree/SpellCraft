using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private enum ResState { LOW, MEDIUM, HIGH };
    private ResState curState = ResState.HIGH;
    private bool fullscreen = true;
    [SerializeField] private Slider master;
    [SerializeField] private Slider sfx;
    [SerializeField] private Slider music;

    private void Start()
    {
        master.value = PlayerPrefs.HasKey("Master") ? PlayerPrefs.GetFloat("Master") : 1;
        sfx.value = PlayerPrefs.HasKey("SFX") ? PlayerPrefs.GetFloat("SFX") : 1;
        music.value = PlayerPrefs.HasKey("Music") ? PlayerPrefs.GetFloat("Music") : 1;

        
    }
    public void SetHighQuality()
    {
        curState = ResState.HIGH;
        Screen.SetResolution(1920, 1200, fullscreen);
    }

    public void SetMediumQuality()
    {
        curState = ResState.MEDIUM;
        Screen.SetResolution(1680, 1050, fullscreen);
    }

    public void SetLowQuality()
    {
        curState = ResState.LOW;
        Screen.SetResolution(1440, 900, fullscreen);
    }

    public void ToggleFullscreen()
    {
        fullscreen = !fullscreen;
        switch(curState)
        {
            case ResState.LOW: SetLowQuality(); break;
            case ResState.MEDIUM: SetMediumQuality(); break;
            default: SetHighQuality(); break;
        }
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
