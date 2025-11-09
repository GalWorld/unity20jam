using UnityEngine;

public class ShaderController : MonoBehaviour
{
    public static ShaderController instance;

    [Range(1, 0)] [SerializeField] private float dissolve;
    [Range(0, 1)] [SerializeField] private float morph;

    [SerializeField] private float smoothSpeed = 5f;
    private float currentDissolve;
    private float currentMorph;

    private void OnValidate()
    {
        Shader.SetGlobalFloat("_DissolveAmount", dissolve);
        Shader.SetGlobalFloat("_MorphAmount", morph);
    }

    private void Awake()
    {
        instance = this;
        currentDissolve = dissolve;
        currentMorph = dissolve;
    }

    private void Update()
    {
        currentDissolve = Mathf.Lerp(currentDissolve, dissolve, Time.deltaTime * smoothSpeed);
        currentMorph = Mathf.Lerp(currentMorph, morph, Time.deltaTime * smoothSpeed);

        ApplyShaderValues();
    }

    private void ApplyShaderValues()
    {
        Shader.SetGlobalFloat("_DissolveAmount", currentDissolve);
        Shader.SetGlobalFloat("_MorphAmount", currentMorph);
    }

    public void SetMorph(float val)
    {
        morph = Mathf.Clamp01(val);
    }

    public void SetDissolve(float val)
    {
        dissolve = Mathf.Clamp01(val);
    }
}
