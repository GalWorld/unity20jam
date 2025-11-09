using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class VisualAudioController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float master = 0f;

    public VisualEffect vfx;

    private float last = -1f;
    private Coroutine fadeRoutine;

    private void Start()
    {

        master = 0f;

        ShaderController.instance.SetMorph(0);
        ShaderController.instance.SetDissolve(1);
        MusicManager.instance.SetVolumeSong(0);


        StartCoroutine(FadeOn());
    }

    private void Update()
    {
        if (Mathf.Approximately(last, master)) return;

        last = master;

        MusicManager.instance.SetVolumeSong(master);
        ShaderController.instance.SetMorph(master);
        ShaderController.instance.SetDissolve(1f - master);
    }
    private IEnumerator FadeOn()
    {
        master = 0f;

        if (vfx != null)
            vfx.SendEvent("OnDissolve");

        MusicManager.instance.Play();

        for (int i = 0; i <= 10; i++)
        {
            master = i * 0.1f;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void TriggerOff()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOff());
    }

    private IEnumerator FadeOff()
    {
        for (int i = 10; i >= 0; i--)
        {
            master = i * 0.1f;
            yield return new WaitForSeconds(0.15f);
        }


        if (vfx != null)
            vfx.SendEvent("OnDissolve");


        MusicManager.instance.Stop();


        ShaderController.instance.SetMorph(0);
        ShaderController.instance.SetDissolve(1);
    }
}
