using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip _buttonPressClip;
    public void PlayButtonDownAudioClip()
    {
        _audioSource.volume = Constants.SoundSliderValue;
        _audioSource.PlayOneShot(_buttonPressClip);
    }
}
