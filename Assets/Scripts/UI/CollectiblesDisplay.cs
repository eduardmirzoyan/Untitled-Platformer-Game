using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(FadeUI))]
public class CollectiblesDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private FadeUI fadeUI;

    [Header("Data")]
    [SerializeField] private int totalCollectibles;
    [SerializeField] private int currentCollected;

    [Header("Settings")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color completedColor;

    private void Awake()
    {
        fadeUI = GetComponent<FadeUI>();
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLockEntrance += Show;
        LevelEvents.instance.onPlayerDeath += Hide;
        LevelEvents.instance.onLevelExit += Hide;

        LevelEvents.instance.onLevelSetup += SetupDisplay;
        LevelEvents.instance.onCollect += UpdateText;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLockEntrance -= Show;
        LevelEvents.instance.onPlayerDeath -= Hide;
        LevelEvents.instance.onLevelExit -= Hide;

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

    private void Show()
    {
        // Show UI
        fadeUI.FadeIn();
    }

    private void Hide()
    {
        // Hide UI
        fadeUI.FadeOut();
    }

    private void Hide(Transform transform)
    {
        Hide();
    }
}
