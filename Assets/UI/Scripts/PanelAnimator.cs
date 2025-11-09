using System.Runtime.Serialization;
using UnityEngine;

public class PanelAnimator : MonoBehaviour
{
    [SerializeField] private UIDOTweenAnimator panelAnimator;

    public void ShowPanel() => panelAnimator.PlayEnter();
    public void HidePanel() => panelAnimator.PlayExit();
    private void OnEnable() {
        panelAnimator.PlayEnter();
    }

    public void GoToScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}