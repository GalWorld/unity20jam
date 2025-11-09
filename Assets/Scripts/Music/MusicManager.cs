using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using System.Collections;
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource song;
    [MinMaxSlider(-3f, 3f)]
    [SerializeField] private Vector2 pitchRange = Vector2.up;

    [Header("Events")]
    [SerializeField] private UnityEvent onSongFinished;

    private double startTime;
    private bool hasStarted;
    private bool finishedInvoked;
    private bool pausedManually;
    private Coroutine smoothRewindRoutine, smoothPitchRoutine;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (song == null || song.clip == null) return;

        if (!hasStarted && AudioSettings.dspTime >= startTime && (song.isPlaying || song.time > 0f))
            hasStarted = true;

        if (hasStarted
            && !finishedInvoked
            && !pausedManually
            && !song.loop
            && !song.isPlaying
            && song.time > 0f)
        {
            finishedInvoked = true;
            onSongFinished?.Invoke();
        }
    }

    public void Play()
    {
        if (!song || !song.clip)
        {
            Debug.LogError("Falta la cancion o el clip en el Music Manager");
            return;
        }

        hasStarted = false;
        finishedInvoked = false;
        pausedManually = false;
        startTime = AudioSettings.dspTime + 0.1f;
        song.PlayScheduled(startTime);
    }

    public void Stop()
    {
        if (!song) return;
        song.Stop();
        hasStarted = false;
        pausedManually = false;
        finishedInvoked = false;
        song.time = 0f;
    }

    public void Pause()
    {
        if (!song) return;
        if (song.isPlaying)
        {
            song.Pause();
            pausedManually = true;
        }
    }

    public void Resume()
    {
        if (!song) return;
        if (pausedManually)
        {
            song.UnPause();
            pausedManually = false;
        }
    }

    public void TogglePause()
    {
        if (!song) return;
        if (pausedManually) Resume();
        else Pause();
    }

    public AudioSource GetSong() => song;

    public float GetCurrentTimeSong()
    {
        return (float)(AudioSettings.dspTime - startTime);
    }

    public float GetDuration() => song && song.clip ? song.clip.length : 0f;
    public float GetPitch() => song && song.clip ? song.pitch : 0f;

    public float SetVolumeSong(float vol)
    {
        if (!song) return 0f;
        float trueVol = Mathf.Clamp01(vol);
        song.volume = trueVol;
        return song.volume;
    }

    public void AddPitchSong(float diff, float smoothDuration)
    {
        if (!song) return;

        float targetPitch = Mathf.Clamp(song.pitch + diff, pitchRange.x, pitchRange.y);

        if (smoothPitchRoutine != null)
            StopCoroutine(smoothPitchRoutine);

        smoothPitchRoutine = StartCoroutine(SmoothPitch(targetPitch, smoothDuration));
    }
    public void Rewind(float seconds, float smoothDuration)
    {
        if (!song || !song.clip) return;

        float targetTime = Mathf.Clamp(song.time + seconds, 0f, song.clip.length);

        if (smoothRewindRoutine != null)
            StopCoroutine(smoothRewindRoutine);

        smoothRewindRoutine = StartCoroutine(SmoothRewind(targetTime, smoothDuration));
    }

    private void ResyncStartTimeToSongTime()
    {
        startTime = AudioSettings.dspTime - song.time;
    }

    private IEnumerator SmoothPitch(float targetPitch, float duration)
    {
        float startPitch = song.pitch;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Lerp(startPitch, targetPitch, t / duration);
            song.pitch = lerp;
            yield return null;
        }

        song.pitch = targetPitch;
    }

    private IEnumerator SmoothRewind(float targetTime, float duration)
    {
        float originalVolume = song.volume;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            song.volume = Mathf.Lerp(originalVolume, 0f, t / duration);
            yield return null;
        }

        song.time = targetTime;
        finishedInvoked = false;
        ResyncStartTimeToSongTime();

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            song.volume = Mathf.Lerp(0f, originalVolume, t / duration);
            yield return null;
        }

        song.volume = originalVolume;
    }

}
