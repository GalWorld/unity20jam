using UnityEngine;
using UnityEngine.UI;

public class TestSliderTime : MonoBehaviour
{
    public Slider songSlider, pitchSlider;
    void Start()
    {
        songSlider.maxValue = MusicManager.instance.GetDuration();
    }

    void Update()
    {
        songSlider.value = MusicManager.instance.GetCurrentTimeSong();
        pitchSlider.value = MusicManager.instance.GetPitch();
    }
}