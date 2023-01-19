using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Show updated scores
        UpdateScores();
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void StartLevel(int buildIndex)
    {
        // Debug
        print("Starting Level " + buildIndex);

        // Load scene
        TransitionManager.instance.LoadSelectedScene(buildIndex, Vector3.zero);
    }

    private void UpdateScores()
    {
        // Loop through each
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            // Check for saved score
            float time = PlayerPrefs.GetFloat("Best Time " + (i + 1), float.MaxValue);

            // Check if exits
            if (time == float.MaxValue)
            {
                // Set default value
                scoreTexts[i].text = "--";
            }
            else
            {
                // Set high score
                scoreTexts[i].text = TimerDisplay.GetTimeAsString(time);
            }
        }
    }
}
