using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AudioManger : MonoBehaviour
{
    public Sound[] music_sounds,sfx_sounds;
    public AudioSource music_source, sfx_source;
    public static AudioManger Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            SceneManager.sceneLoaded += UpdateAudioVolum;
        }
        else
        {
            Destroy(Instance ); 
        }
    }

    private void UpdateAudioVolum(Scene arg0, LoadSceneMode arg1)
    {
        SetMusicVolume(PlayerPrefs.GetFloat("music_volume"));
        SetVFXVolume(PlayerPrefs.GetFloat("SFX_volume"));
    }

    public void PlayMusic(string name, bool islooping = true)
    {
        Sound s = Array.Find(music_sounds, x => x.Name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            music_source.loop = islooping;
            music_source.clip = s.clip;
            music_source.Play();
        }
    }
    public void PlaySFX(string name, bool islooping = false)
    {
        Sound s = Array.Find(sfx_sounds, x => x.Name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfx_source.loop = islooping;
            sfx_source.PlayOneShot(s.clip);
        }
    }

    public void ToggleMusic()
    {
        music_source.mute= !music_source.mute;
    }
    public void ToggleSFX()
    {
        sfx_source.mute = !sfx_source.mute;
    }

    public void SetMusicVolume(float volume=0)
    {
        music_source.volume = volume;
        PlayerPrefs.SetFloat("music_volume", volume);
    }
    public void SetVFXVolume(float volume)
    {
        sfx_source.volume = volume;
        PlayerPrefs.SetFloat("SFX_volume", volume);
    }
    
    public void StopMusic()
    { 
        music_source.Stop(); 
    }
    public void StopVFX()
    {
        sfx_source.Stop();
    }
}

 