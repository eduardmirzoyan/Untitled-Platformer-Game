using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        LevelEvents.instance.onPlayerDeath += Display;
    }

    private void OnDestroy()
    {
        LevelEvents.instance.onPlayerDeath -= Display;
    }

    private void Display()
    {
        // Display UI
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
