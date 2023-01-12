using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectiblesUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        LevelEvents.instance.onCollectCollectible += UpdateText;
    }

    private void OnDestroy()
    {
        LevelEvents.instance.onCollectCollectible -= UpdateText;
    }

    private void UpdateText()
    {
        // TODO
    }
}
