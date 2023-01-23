using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField, ReadOnly] private Animator animator;
    [SerializeField, ReadOnly] private TextMeshProUGUI[] scoreTexts;

    [Header("Settings")]
    [SerializeField] private float delayDuration = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Show updated scores
        UpdateScores();
    }

    public void StartLevel(int buildIndex)
    {
        // Debug
        print("Starting Level " + buildIndex);

        // Start level
        StartCoroutine(DelayedStart(delayDuration, buildIndex));
    }

    private IEnumerator DelayedStart(float duration, int buildIndex)
    {
        // Wait
        yield return new WaitForSeconds(duration);

        // Load scene
        TransitionManager.instance.LoadSelectedScene(buildIndex, Vector3.zero);
    }

    public void UpdateScores()
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
