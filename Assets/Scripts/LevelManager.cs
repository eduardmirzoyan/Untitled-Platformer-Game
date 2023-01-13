using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<CollectibleHandler> collectibles;
    [SerializeField] private PlayerController player;
    [SerializeField] private ExitDoorHandler exitDoor;

    [Header("Settings")]
    [SerializeField] private Vector2Int mapSize;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    public static LevelManager instance;

    private void Awake()
    {
        // Singleton logic
        if (LevelManager.instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        collectibles = new List<CollectibleHandler>();
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onCollect += DecrementNumCollectibles;

        // Look for important game objects
        FindPlayer();
        FindCollectibles();
        FindExit();

        // Open scene
        TransitionManager.instance.OpenScene(player.transform.position);

        // Start level after scene is opened
        StartCoroutine(DelayedStart(TransitionManager.instance.GetTransitionTime()));
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onCollect -= DecrementNumCollectibles;
    }

    private IEnumerator DelayedStart(float duration)
    {
        // Wait 1 frame
        yield return new WaitForEndOfFrame();

        // Trigger event
        LevelEvents.instance.TriggerOnLevelSetup(mapSize, collectibles);

        // Wait a few
        yield return new WaitForSeconds(duration);

        // Trigger event
        LevelEvents.instance.TriggerOnLevelEnter(player.transform);
    }

    public Transform GetPlayerTransform() => player.transform;

    public void ExitLevel()
    {
        // Trigger event
        LevelEvents.instance.TriggerOnLevelExit(player.transform);
    }

    private void DecrementNumCollectibles(CollectibleHandler collectible)
    {
        if (debugMode) print("Collectible count decreased!");

        // Remove from list
        collectibles.Remove(collectible);

        // If you reach 0
        if (collectibles.Count == 0)
        {
            // Open exit
            // Trigger event
            LevelEvents.instance.TriggerOnUnlockExit(exitDoor.transform);
        }
    }

    private void FindPlayer()
    {
        // Find player
        var player = GameObject.FindObjectOfType<PlayerController>();

        // Cache ref
        this.player = player;
    }

    private void FindCollectibles()
    {
        // Find all collectibles in this scene
        var collectibles = GameObject.FindObjectsOfType<CollectibleHandler>();

        // Save lists
        this.collectibles.AddRange(collectibles);
        
        // Debug
        if (debugMode) print("Found " + this.collectibles.Count + " collectibles.");
    }

    private void FindExit()
    {
        // Find exit
        var exitDoor = GameObject.FindObjectOfType<ExitDoorHandler>();

        // Cache ref
        this.exitDoor = exitDoor;
    }
}
