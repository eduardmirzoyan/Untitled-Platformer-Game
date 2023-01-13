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

    [Header("Data")]
    [SerializeField] private float startTime;
    [SerializeField] private bool started;

    [Header("Settings")]
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;

    [Header("Debug")]
    [SerializeField] private bool debugMode;

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLockEntrance += StartTimer;
        LevelEvents.instance.onPlayerDeath += StopTimer;
        LevelEvents.instance.onLevelExit += StopTimer;

        minutesText.color = inactiveColor;
        secondsText.color = inactiveColor;
        milliText.color = inactiveColor;
    }

    private void OnDestroy()
    {
        LevelEvents.instance.onLockEntrance -= StartTimer;
        LevelEvents.instance.onPlayerDeath -= StopTimer;
        LevelEvents.instance.onLevelExit -= StopTimer;
    }

    private void Update()
    {
        if (started)
        {
            float time = Time.time - startTime;
            
            // Debug
            if (debugMode) print(time);

            int minutes = (int) (time / 60);
            minutesText.text = minutes + "'";

            int seconds = (int) (time % 60);
            secondsText.text = seconds + "''";

            float milliSecondsFloat = ((time % 60) - seconds) * 100;
            int milliSeconds = (int) Mathf.Round(milliSecondsFloat);

            string text = (milliSeconds < 10) ? "0" + milliSeconds : "" + milliSeconds;
            milliText.text = text + "'''";
        }
        
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

    private void StopTimer(Transform transform)
    {
        StopTimer();
    }
}
