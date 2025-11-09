using UnityEngine;

public class DurationConsumable : MonoBehaviour
{
    [Range(0,1f)]
    public float smooth = 0.3f;
    public float rewindAmount = 5;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            MusicManager.instance.Rewind(rewindAmount, smooth);
        }
    }
}
