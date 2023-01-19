using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI minutesText;
    [SerializeField] private TextMeshProUGUI secondsText;
    [SerializeField] private TextMeshProUGUI milliText;
    [SerializeField] private TextMeshProUGUI newRecordText;

    [Header("Data")]
    [SerializeField] private float startTime;
    [SerializeField] private bool started;

    [Header("Settings")]
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;

    [Header("Debug")]
    [SerializeField] private bool debugMode;

    private float time;

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLockEntrance += StartTimer;
        LevelEvents.instance.onPlayerDeath += StopTimer;
        LevelEvents.instance.onLevelExit += StopTimerOnWin;

        minutesText.color = inactiveColor;
        secondsText.color = inactiveColor;
        milliText.color = inactiveColor;

        // Start time at 0
        time = 0f;
    }

    private void OnDestroy()
    {
        LevelEvents.instance.onLockEntrance -= StartTimer;
        LevelEvents.instance.onPlayerDeath -= StopTimer;
        LevelEvents.instance.onLevelExit -= StopTimerOnWin;
    }

    private void Update()
    {
        if (started)
        {
            time = Time.time - startTime;
            
            // Debug
            if (debugMode) print(time);

            // Display
            DisplayTime(time);
        }
        
    }

    private void DisplayTime(float time)
    {
        int minutes = (int)(time / 60);
        minutesText.text = minutes + "'";

        int seconds = (int)(time % 60);
        secondsText.text = seconds + "''";

        float milliSecondsFloat = ((time % 60) - seconds) * 100;
        int milliSeconds = (int)Mathf.Round(milliSecondsFloat);

        string text = (milliSeconds < 10) ? "0" + milliSeconds : "" + milliSeconds;
        milliText.text = text + "'''";
    }

    private void StartTimer()
    {
        // Set start time
        startTime = Time.time;

        // Enable flag
        started = true;

        // Activate color
        minutesText.color = activeColor;
        secondsText.color = activeColor;
        milliText.color = activeColor;
    }

    private void StopTimer()
    {
        // Disable flag
        started = false;

        // Turn off color
        minutesText.color = inactiveColor;
        secondsText.color = inactiveColor;
        milliText.color = inactiveColor;
    }

    private void StopTimerOnWin(Transform transform)
    {
        // Stop the timer
        StopTimer();

        // Save highscore
        SaveTime();
    }

    private void SaveTime()
    {
        // Get any saved time on this level
        float savedTime = PlayerPrefs.GetFloat("Best Time " + TransitionManager.instance.GetSceneIndex(), float.MaxValue);
        
        // Check if this time is better
        if (time < savedTime)
        {
            // Debug
            print("New record saved!");

            // Overwrite save for this level
            PlayerPrefs.SetFloat("Best Time " + TransitionManager.instance.GetSceneIndex(), time);

            // Show feedback
            newRecordText.enabled = true;
            newRecordText.color = Color.yellow;

            minutesText.color = Color.yellow;
            secondsText.color = Color.yellow;
            milliText.color = Color.yellow;
        }
    }

    public static string GetTimeAsString(float time)
    {
        string result = "";

        int minutes = (int)(time / 60);
        result += minutes + "' ";

        int seconds = (int)(time % 60);
        result += seconds + "'' ";

        float milliSecondsFloat = ((time % 60) - seconds) * 100;
        int milliSeconds = (int)Mathf.Round(milliSecondsFloat);

        string text = (milliSeconds < 10) ? "0" + milliSeconds : "" + milliSeconds;
        result += text + "'''";

        return result;
    }
}
