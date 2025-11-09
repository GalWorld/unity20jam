using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class ShaderToggleSmooth : MonoBehaviour
{
    [Header("Transición")]
    [Min(0.0001f)] public float speed = 1f;
    [Range(0f, 1f)] public float currentValue = 0f;

    bool goingUp = false;
    string floatProperty = "_Dissolver";
    bool goingDown = false;

    Renderer rend;
    MaterialPropertyBlock mpb;

    void Awake()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        if (mpb == null) mpb = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        currentValue = 0f;
        goingUp = true;
        goingDown = false;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    void Update()
    {
        if (!goingUp && !goingDown) return;

        float dt = Application.isPlaying ? Time.deltaTime : (1f / 60f);
        float target = goingUp ? 1f : 0f;

        currentValue = Mathf.MoveTowards(currentValue, target, speed * dt);
        ApplyValue();

        if (Mathf.Approximately(currentValue, target))
        {
            if (goingDown)
            {
                goingDown = false;
                if (Application.isPlaying)
                    gameObject.SetActive(false);
                else
                    enabled = false;
            }
            else if (goingUp)
            {
                goingUp = false;
            }
        }
    }

    void ApplyValue()
    {
        if (rend == null) return;

        float shaderValue = 1f - currentValue;

        rend.GetPropertyBlock(mpb);
        mpb.SetFloat(floatProperty, shaderValue);
        rend.SetPropertyBlock(mpb);
    }

    [ContextMenu("Enable Smooth")]
    public void EnableSmooth()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        enabled = true;
        goingDown = false;
        goingUp = true;
    }

    [ContextMenu("Disable Smooth")]
    public void DisableSmooth()
    {
        enabled = true;
        goingUp = false;
        goingDown = true;
    }

    void OnValidate()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        if (mpb == null) mpb = new MaterialPropertyBlock();
        ApplyValue();
    }
}
