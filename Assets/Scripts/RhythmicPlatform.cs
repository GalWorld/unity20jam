using UnityEngine;

public class RhythmicPlatform: MonoBehaviour
{
    public enum SyncMode { BPM, Auto }
    

    [Header("Movement")]
    public float amplitude = 2f;                 
    public float smoothness = 8f;
    public float speed = 1;
    public Vector3 direction = Vector3.right;
    public AnimationCurve wave = AnimationCurve.Linear(0, 0, 1, 1);

    public SyncMode mode = SyncMode.Auto;

    [Header("For BPM Mode")]
    public float bpm = 169f;
    [Range(1, 8)] public int subdivision = 1;    
    public float songOffsetSec = 0f;             
    public bool useSamplesForTime = true;        

    [Header("For AUTO Mode")]
    public float minInterval = 0.2f;             
    public float sensitivity = 1.6f;             
    public int energyWindow = 1024;              
    public float pulseWidth = 0.10f;

    private AudioSource audioSource;
    private Vector3 initialPos;
    private float beatInterval;
    private float lastAutoBeatTime = -999f;
    private float autoPulseTimer = 0f;
    private float[] sampleBuf;

    void Start()
    {
        audioSource = MusicManager.instance.GetSong();
        if (!audioSource.clip)
        {
            Debug.LogError("[MoveWithBeatPro] Asigna un AudioSource con clip.");
            enabled = false; return;
        }

        initialPos = transform.position;
        beatInterval = 60f / Mathf.Max(1f, bpm) / Mathf.Max(1, subdivision);

        sampleBuf = new float[Mathf.Max(256, energyWindow)];
    }

    void Update()
    {
        if (!audioSource.isPlaying) return;

        double songTime = GetSongTime();
        speed = audioSource.volume;

        float pulse = 0f;
        if (mode == SyncMode.BPM)
        {
            float interval = Mathf.Max(0.0001f, beatInterval);
            float phase = (float)((songTime - songOffsetSec) % interval) / interval;
            if (phase < 0f) phase += 1f;

            float s = Mathf.Sin(phase * Mathf.PI * 2f) * 0.5f + 0.5f;
            pulse = wave.Evaluate(s);
        }
        else
        {
            audioSource.GetOutputData(sampleBuf, 0);

            float rms = 0f;
            for (int i = 0; i < energyWindow && i < sampleBuf.Length; i++)
                rms += sampleBuf[i] * sampleBuf[i];
            rms = Mathf.Sqrt(rms / Mathf.Max(1, Mathf.Min(energyWindow, sampleBuf.Length)));

            float threshold = EnergyTracker.Update(rms) * sensitivity;

            if (rms > threshold && (Time.time - lastAutoBeatTime) > minInterval)
            {
                lastAutoBeatTime = Time.time;
                autoPulseTimer = pulseWidth;
            }

            autoPulseTimer = Mathf.Max(0f, autoPulseTimer - Time.deltaTime);
            pulse = Mathf.Clamp01(autoPulseTimer / Mathf.Max(0.0001f, pulseWidth));
        }

        float t = (pulse * 2f) - 1f;
        float offset = t * amplitude * audioSource.pitch * speed;
        Vector3 target = initialPos + offset * direction;

        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * smoothness);
    }

    float GetSongTime()
    {
        if (useSamplesForTime && audioSource.clip)
        {
            return audioSource.timeSamples / audioSource.clip.frequency;
        }
        else
        {
            return MusicManager.instance.GetCurrentTimeSong();
        }
    }

    static class EnergyTracker
    {
        static float ema = 0f;
        const float alpha = 0.07f;
        public static float Update(float x)
        {
            ema = (1f - alpha) * ema + alpha * x;
            return Mathf.Max(1e-6f, ema);
        }
    }
}
