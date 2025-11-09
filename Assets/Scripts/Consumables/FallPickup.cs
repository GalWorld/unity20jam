using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Pickup))]
public class FallPickup : MonoBehaviour
{
    [Range(0.2f, 1f)]
    [SerializeField] private float wait = 0.5f;
    private Pickup pickup;
    private Collider col;

    void Start()
    {
        pickup = GetComponent<Pickup>();
        if (pickup != null) pickup.enabled = false;

        col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
    }

    IEnumerator BeginPickup()
    {
        yield return new WaitForSeconds(wait);
        col.isTrigger = true;
        pickup.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(BeginPickup());
    }
}
