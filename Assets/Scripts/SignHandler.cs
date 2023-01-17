using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignHandler : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI instructionsText;

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField][TextArea(20, 5)] private string text;

    private Coroutine routine;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        instructionsText = GetComponentInChildren<TextMeshProUGUI>();

        // Set text
        instructionsText.text = text;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Show UI
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeInstructions(0f, 1f, fadeInDuration));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Hide UI
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeInstructions(1f, 0f, fadeInDuration));
    }

    private IEnumerator FadeInstructions(float startAlpha, float endAlpha, float duration)
    {
        // Set start value
        canvasGroup.alpha = startAlpha;

        float elapsed = 0;
        while (elapsed < duration)
        {
            // Lerp alpha
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set end value
        canvasGroup.alpha = endAlpha;
    }
}
