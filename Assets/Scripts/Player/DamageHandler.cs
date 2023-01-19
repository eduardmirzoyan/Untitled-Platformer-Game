using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Data")]
    [SerializeField] private bool isHurt;
    [SerializeField] private bool isInvincible;

    [Header("Settings")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float hitDuration;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private Material originalMaterial;
    private Coroutine flashRoutine;

    private void Awake()
    {
        // Get ref
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Save original material
        originalMaterial = spriteRenderer.material;
    }

    public bool IsHurt() => isHurt;

    public void Hurt()
    {
        // Do nothing
        if (isInvincible) return;

        // If not already hit
        if (!isHurt)
        {
            // Play effect
            HitEffect();

            // Debug
            if (debugMode) print("HURT!");

            // Set flag
            isHurt = true;
        }
    }

    public void SetInvincible(bool state)
    {
        isInvincible = state;
    }

    private void HitEffect()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        flashRoutine = StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Freeze time
        Time.timeScale = 0f;

        // Wait.
        yield return new WaitForSecondsRealtime(hitDuration);

        // Resume time
        Time.timeScale = 1f;

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }
}
