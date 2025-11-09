using UnityEngine;
using UnityEngine.UI;

public class TestSliderTime : MonoBehaviour
{
    [SerializeField] private Image imageFill;

    private void Start()
    {
        if (imageFill == null)
        {
            Debug.LogError("TestSliderTime: No se asignó la imagen de fill.");
            return;
        }
    }

    private void Update()
    {
        if (MusicManager.instance == null || imageFill == null) return;

        float duration = MusicManager.instance.GetDuration();
        float currentTime = MusicManager.instance.GetCurrentTimeSong();

        if (duration <= 0f) return;

        float fillValue = Mathf.Clamp01(currentTime / duration);

        imageFill.fillAmount = fillValue;
    }
}
