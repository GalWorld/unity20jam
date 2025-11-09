using System.Collections.Generic;
using UnityEngine;

public class DissolveManager : MonoBehaviour
{
    [Header("Comportamiento")]
    [Range(0f, 1f)] public float disableThreshold = 0.98f;

    [Header("Optimización")]
    public bool scanOnStart = true;

    readonly List<GameObject> _targets = new();
    bool _currentlyDisabled;

    void Start()
    {
        if (scanOnStart) ScanScene();
        bool shouldDisable = Shader.GetGlobalFloat("_DissolveAmount") >= disableThreshold;
        ApplyState(shouldDisable);
    }

    void Update()
    {
        bool shouldDisable = Shader.GetGlobalFloat("_DissolveAmount") >= disableThreshold;
        if (shouldDisable != _currentlyDisabled)
            ApplyState(shouldDisable);
    }

    void ApplyState(bool shouldDisable)
    {
        _currentlyDisabled = shouldDisable;
        bool targetActive = !shouldDisable;

        for (int i = 0; i < _targets.Count; i++)
        {
            var go = _targets[i];
            if (!go) continue;

            if (go.activeSelf != targetActive)
                go.SetActive(targetActive);
        }
    }

    [ContextMenu("Scan Scene Now")]
    public void ScanScene()
    {
        _targets.Clear();

        var objs = Object.FindObjectsByType<Pickup>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var obj in objs)
        {
            if (!obj) continue;
            RegisterRenderer(obj.gameObject);
        }

        ApplyState(_currentlyDisabled);
    }

    public void RegisterRenderer(GameObject go)
    {
        if (!go) return;
        if (!_targets.Contains(go))
            _targets.Add(go);
    }
}
