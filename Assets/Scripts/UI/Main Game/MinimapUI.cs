using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FadeUI))]
public class MinimapUI : MonoBehaviour
{
    [SerializeField] private FadeUI fadeUI;

    private void Awake()
    {
        fadeUI = GetComponent<FadeUI>();
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLockEntrance += Show;
        LevelEvents.instance.onPlayerDeath += Hide;
        LevelEvents.instance.onLevelExit += Hide;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLockEntrance -= Show;
        LevelEvents.instance.onPlayerDeath -= Hide;
        LevelEvents.instance.onLevelExit -= Hide;
    }

    private void Show()
    {
        // Show UI
        fadeUI.FadeIn();
    }

    private void Hide()
    {
        // Hide UI
        fadeUI.FadeOut();
    }

    private void Hide(Transform transform)
    {
        Hide();
    }
}
