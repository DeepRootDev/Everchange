using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle muteToggle;

    [SerializeField] private string volumeParameter;

    private void Start()
    {
        audioMixer.GetFloat(volumeParameter, out var initialDb);
        var initialToggle = initialDb < -79.99f;
        muteToggle.isOn = initialToggle;

        var initialVolumeInput = DBToVolumeInput(initialDb);
        volumeSlider.value = initialVolumeInput;
        
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(SetMute);
    }

    private void SetVolume(float volumeInput)
    {
        
        var dbValue = VolumeInputToDb(volumeInput);
        audioMixer.SetFloat(volumeParameter, dbValue);
    }

    private void SetMute(bool mute)
    {
        var dbValue = mute ? -80.0f : VolumeInputToDb(volumeSlider.value);
        audioMixer.SetFloat(volumeParameter, dbValue);
    }
    
    // NOTE(marvin): Decibel is a logarithmic scale, but the UI slider is linear input, so we "logify" the input.
    // "Volume input" refers to the UI slider input. "Db" refers to the decibels understood by the Audio Mixer.
    // These two functions should be mathematical inverses of each other.

    private static float VolumeInputToDb(float input)
    {
        var result = 80.0f*(Mathf.Log10(input) - 1.0f);
        return result;
    }

    private static float DBToVolumeInput(float db)
    {
        var result = Mathf.Pow(10.0f, db / 80f + 1.0f);
        return result;
    }
}