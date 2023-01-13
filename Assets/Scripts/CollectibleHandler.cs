using UnityEngine;

public class CollectibleHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Data")]
    [SerializeField] private bool isCollected;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
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

        // Trigger event
        LevelEvents.instance.TriggerOnCollect(this);

        // Destroy this collectible
        Destroy(gameObject, 0.5f);

        // Change state
        isCollected = true;
    }
}
