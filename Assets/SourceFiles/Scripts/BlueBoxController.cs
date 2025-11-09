using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueBoxController : MonoBehaviour
{
    public Renderer rendBoxBlue;

    public float minIntensity = 20f;
    public float maxIntensity = 400f;
    public float speed = 4f;
    private Material mat;
    public Light pointLightBoxBlue;

    [Header("Spawn")]
    [SerializeField] private float spawnDelay = 1f;
    public List<GameObject> spawnPrefabs = new List<GameObject>(); 

    public Transform BoxBlue;
    private float _heightOffset = 2.5f;

    private void Start()
    {
        mat = rendBoxBlue.material;
        pointLightBoxBlue.color = Color.yellow;
        LightChangeColor();
    }

    void LightChangeColor()
    {
        pointLightBoxBlue.color = Color.yellow;
        float t = Mathf.PingPong(Time.time * speed, 1f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        pointLightBoxBlue.intensity = intensity;

        // Start color/spawn routine and reset flag
        StartCoroutine(ChangingColor());
    }

    IEnumerator ChangingColor()
    {
        // Wait before deciding which prefab to spawn
        yield return new WaitForSeconds(spawnDelay);

        // Safety: if list is empty, just restore color and exit
        if (spawnPrefabs == null || spawnPrefabs.Count == 0)
        {
            StartCoroutine(OriginalColor());
            yield break;
        }

        // Pick a random index based on the list size
        int idx = Random.Range(0, spawnPrefabs.Count);

        // Compute spawn position once
        Vector3 offset = new Vector3(
            BoxBlue.transform.position.x,
            BoxBlue.transform.position.y + _heightOffset,
            BoxBlue.transform.position.z
        );

        // You can replace this with your own mapping if you need specific colors per prefab.
        if (idx < spawnPrefabs.Count / 2)
            pointLightBoxBlue.color = Color.red;
        else
            pointLightBoxBlue.color = Color.green;

        // Spawn the selected prefab
        GameObject prefab = spawnPrefabs[idx];
        if (prefab != null)
        {
            Instantiate(prefab, offset, Quaternion.identity);
        }

        // Restore color after a while and re-arm the trigger
        StartCoroutine(OriginalColor());
    }

    IEnumerator OriginalColor()
    {
        yield return new WaitForSeconds(2f);
        pointLightBoxBlue.color = Color.yellow;
    }
}
