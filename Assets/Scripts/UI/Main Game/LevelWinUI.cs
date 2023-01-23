using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWinUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private FadeUI fadeUI;

    [Header("Settings")]
    [SerializeField] private float waitTime = 1f;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        fadeUI = GetComponentInChildren<FadeUI>();
    }

    private void Start()
    {
        LevelEvents.instance.onLevelExit += Display;
    }

    private void OnDestroy()
    {
        LevelEvents.instance.onLevelExit -= Display;
    }

    private void Display(Transform transform)
    {
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

    public void NextLevel() => TransitionManager.instance.LoadNextScene(Vector3.zero);
    public void Restart() => PauseManager.instance.Restart();
    public void MainMenu() => PauseManager.instance.MainMenu();
}
