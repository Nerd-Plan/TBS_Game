using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class Options : MonoBehaviour
{
    
   public void SetMusicVolume(float volume)
   {
        AudioManger.Instance.SetMusicVolume(volume);
   }
    public void SetSFXVolume(float volume)
    {
        AudioManger.Instance.SetVFXVolume(volume);
    }

    public void SetQuailty(int quailty_index)
    {
        QualitySettings.SetQualityLevel(quailty_index);
    }

}
