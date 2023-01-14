using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameOverUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CinemachineVirtualCamera cinemachine;
    [SerializeField] private FadeUI fadeUI;

    [Header("Settings")]
    [SerializeField] private float waitTime = 1f;

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
        // Stop camera from following player
        cinemachine.Follow = null;

        // Block interaction
        canvasGroup.interactable = true;    
        canvasGroup.blocksRaycasts = true;

        // Dim screen
        StartCoroutine(DimScreen(waitTime));
    }

    private IEnumerator DimScreen(float duration)
    {
        // Set inital value
        canvasGroup.alpha = 0f;

        float elapsed = 0;
        while (elapsed < duration)
        {
            // Lerp alpha
            canvasGroup.alpha = elapsed / duration;

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final value
        canvasGroup.alpha = 1f;

        // Now show Window
        fadeUI.FadeIn();
    }
}
