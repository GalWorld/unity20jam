using UnityEngine;

public class BoxSideLights : MonoBehaviour
{
    public Renderer rendBoxSide;
    public Color baseEmissionColor;
    public float minIntensity = -10f;
    public float maxIntensity = -6f;
    public float speed = 4f;
    private Material mat;
   
    
    void Start()
    {
        mat = rendBoxSide.material;
        mat.EnableKeyword("_EMISSION");
        
    }

   void Update()
    {
        EmissionLight();
    }

   void EmissionLight()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        mat.SetColor("_EmissionColor", baseEmissionColor * Mathf.Pow(2.0f, intensity));
    }


}
