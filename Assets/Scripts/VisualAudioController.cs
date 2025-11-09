using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class VisualAudioController : MonoBehaviour
{
    [Header("Control principal")]
    [Range(0f, 1f)]
    public float master = 0f;

    public VisualEffect vfx;

    [Header("Curva de evolución del master")]
    [Tooltip("Si está activo, el master se calculará según el tiempo de la canción y esta curva.")]
    public bool driveMasterFromMusicTime = true;

    [Tooltip("Curva en función del tiempo normalizado de la canción (0 = inicio, 1 = fin). " +
             "El valor evaluado se usa como master.")]
    public AnimationCurve masterOverTime = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float last = -1f;
    private Coroutine fadeRoutine;

    private void Start()
    {
        master = 0f;

        ShaderController.instance.SetMorph(0);
        ShaderController.instance.SetDissolve(1);
        MusicManager.instance.SetVolumeSong(0);

        if (!driveMasterFromMusicTime)
            StartCoroutine(FadeOn());
        else
            MusicManager.instance.Play();
    }

    private void Update()
    {
        if (driveMasterFromMusicTime)
        {
            float dur = MusicManager.instance.GetDuration();
            if (dur > 0f)
            {
                float t = Mathf.Clamp01(MusicManager.instance.GetCurrentTimeSong() / dur);

                float target = Mathf.Clamp01(masterOverTime.Evaluate(t));
                master = target;
            }
        }

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

        if (driveMasterFromMusicTime)
        {
            master = 0f;

            if (vfx != null)
                vfx.SendEvent("OnDissolve");

            MusicManager.instance.Stop();
            ShaderController.instance.SetMorph(0);
            ShaderController.instance.SetDissolve(1);
        }
        else
        {
            fadeRoutine = StartCoroutine(FadeOff());
        }
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
