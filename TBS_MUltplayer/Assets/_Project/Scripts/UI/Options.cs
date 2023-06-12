using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] GameObject vfx_slider;
    [SerializeField] GameObject music_slider;
    [SerializeField] GameObject quailty_drop_down;
    private void Start()
    {
        music_slider = transform.GetChild(0).gameObject;
        quailty_drop_down = transform.GetChild(1).gameObject;
        vfx_slider = transform.GetChild(3).gameObject;

        music_slider.GetComponent<Slider>().SetValueWithoutNotify(AudioManger.Instance.music_source.volume);
        vfx_slider.GetComponent<Slider>().SetValueWithoutNotify(AudioManger.Instance.sfx_source.volume);
        quailty_drop_down.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(QualitySettings.GetQualityLevel());
    }

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
