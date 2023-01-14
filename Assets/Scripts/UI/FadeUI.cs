using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float fadeInOffset = 1f;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float fadeOutOffset = 1f;

    private Coroutine routine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        // Fade in UI
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeInOverTime(fadeInDuration));
    }

    public void FadeOut()
    {
        // Fade out UI
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeOutOverTime(fadeOutDuration));
    }

    private IEnumerator FadeInOverTime(float duration)
    {
        Vector3 startPos = transform.localPosition + Vector3.up * fadeInOffset;
        Vector3 endPos = transform.localPosition;
        canvasGroup.alpha = 0f;

        float elapsed = 0;
        while (elapsed < duration)
        {
            // Lerp alpha
            canvasGroup.alpha = elapsed / duration;

            // Lerp position
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final values
        transform.localPosition = endPos;
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutOverTime(float duration)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = transform.localPosition + Vector3.up * fadeOutOffset;
        canvasGroup.alpha = 1f;

        float elapsed = 0;
        while (elapsed < duration)
        {
            // Lerp alpha
            canvasGroup.alpha = 1 - elapsed / duration;

            // Lerp position
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final values
        transform.localPosition = endPos;
        canvasGroup.alpha = 0f;
    }
}
