using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingManagerUI : MonoBehaviour
{
    [SerializeField] private ScrollingUI[] scrollingUIs;
    [SerializeField] private float newFactor = 2f;

    private void Awake()
    {
        scrollingUIs = GetComponentsInChildren<ScrollingUI>();
    }

    public void AccelerateUI()
    {
        // Loop through all UI
        foreach (var scrollingUI in scrollingUIs)
        {
            // Set new speed
            scrollingUI.SetSpeed(newFactor);
        }
    }
}
