using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectiblesDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Data")]
    [SerializeField] private int totalCollectibles;
    [SerializeField] private int currentCollected;

    [Header("Settings")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color completedColor;

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelSetup += SetupDisplay;
        LevelEvents.instance.onCollect += UpdateText;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLevelSetup -= SetupDisplay;
        LevelEvents.instance.onCollect -= UpdateText;
    }

    private void SetupDisplay(Vector2Int mapSize, List<CollectibleHandler> collectibles)
    {
        this.totalCollectibles = collectibles.Count;
        this.currentCollected = 0;

        // Set text
        text.text = currentCollected + "/" + totalCollectibles;
        text.color = defaultColor;
    }

    private void UpdateText(CollectibleHandler collectible)
    {
        // Increment
        currentCollected++;

        // Update text
        text.text = currentCollected + "/" + totalCollectibles;
        text.color = currentCollected >= totalCollectibles ? completedColor : defaultColor;
    }
}
