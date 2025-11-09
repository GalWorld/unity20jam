using UnityEngine;

public class PitchConsumable : MonoBehaviour
{
    [Range(0, 1f)]
    public float smooth = 0.3f;
    public float pitchAmount = 0.2f;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            MusicManager.instance.AddPitchSong(pitchAmount, smooth);
        }
    }
}
