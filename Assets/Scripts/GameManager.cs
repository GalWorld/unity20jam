using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private float dissolveDuration = 1.2f; 

    private bool _ending;

    private void OnEnable() 
    {
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        if (_ending) return;
        StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        _ending = true;

        // Start from current global dissolve value (0..1)
        float start = Shader.GetGlobalFloat("_DissolveAmount");
        float t = 0f;

        while (t < dissolveDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(start, 1f, Mathf.Clamp01(t / dissolveDuration));
            ShaderController.instance.SetDissolve(v);
            yield return null;
        }

        ShaderController.instance.SetDissolve(1f);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(true);

        Time.timeScale = 0f; 
    }
}
