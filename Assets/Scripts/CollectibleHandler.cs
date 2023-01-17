using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CollectibleHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Light2D lightSource;

    [Header("Data")]
    [SerializeField] private bool isCollected;

    [Header("Settings")]
    [SerializeField] private float despawnDuration = 1f;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        lightSource = GetComponentInChildren<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If a player collected this
        if (!isCollected && other.tag == "Player")
        {
            Collect();
        }
    }

    private void Collect()
    {
        // Debug
        if (debugMode) print("Collected!");

        // Play animation
        animationHandler.ChangeAnimation("Pick up");

        // Play sound
        AudioManager.instance.Play("Collect");

        // Trigger event
        LevelEvents.instance.TriggerOnCollect(this);

        // Destroy this collectible
        StartCoroutine(ReduceLightOverTime(despawnDuration));

        // Change state
        isCollected = true;
    }

    private IEnumerator ReduceLightOverTime(float duration)
    {
        float startRadius = lightSource.pointLightOuterRadius;
        float endRadius = 0f;

        float elapsed = 0;
        while (elapsed < duration)
        {
            // Lerp radius
            lightSource.pointLightOuterRadius = Mathf.Lerp(startRadius, endRadius, elapsed / duration);

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destory object
        Destroy(gameObject);
    }
}
