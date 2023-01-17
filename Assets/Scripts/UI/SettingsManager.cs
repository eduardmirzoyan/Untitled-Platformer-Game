using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

    [Header("Settings")]
    [SerializeField] private KeyCode closeHotkey = KeyCode.Escape;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private Resolution[] resolutions;
    private bool isOpen;

    public static SettingsManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        canvasGroup = GetComponentInChildren<CanvasGroup>(); 
    }

    private void LoadSettings()
    {
        // Load any existing prefs

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        }

        if (PlayerPrefs.HasKey("UIVolume"))
        {
            SetUIVolume(PlayerPrefs.GetFloat("UIVolume"));
        }

        if (PlayerPrefs.HasKey("Resolution"))
        {
            SetResolution(PlayerPrefs.GetInt("Resolution"));
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            SetFullscreen(PlayerPrefs.GetInt("Fullscreen") == 1);
        }
    }

    private void Start()
    {
        // Clear any previous options in dropdown
        resolutionsDropdown.ClearOptions();

        // Cache possible resolutions based on hardware
        resolutions = Screen.resolutions;

        // Format resolutions
        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].height == Screen.currentResolution.height && resolutions[i].width == Screen.currentResolution.width)
            {
                currentResIndex = i;
            }
        }

        // Update possible resolutions
        resolutionsDropdown.AddOptions(options);
        // Mark your current resolution
        resolutionsDropdown.value = currentResIndex;
        // Update the options
        resolutionsDropdown.RefreshShownValue();

        // Load any presistent settings
        LoadSettings();
    }

    private void Update()
    {
        // If hotkey is pressed while settings is open
        if (Input.GetKeyDown(closeHotkey) && isOpen)
        {
            // Close settings UI
            Close();
        }
    }

    public void Open()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isOpen = true;
    }

    public void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        isOpen = false;
    }

    /// ~~~~~ Functionality Here ~~~~~

    public void SetMasterVolume(float volume)
    {
        // Set mixer volume 
        audioMixer.SetFloat("MasterVolume", volume);

        // Set slider
        masterVolumeSlider.value = volume;

        // Save pref
        PlayerPrefs.SetFloat("MasterVolume", volume);

        // Debug
        if (debugMode) Debug.Log("Master volume set to: " + volume);
    }

    public void SetMusicVolume(float volume)
    {
        // Set mixer volume 
        audioMixer.SetFloat("MusicVolume", volume);

        // Set slider
        musicVolumeSlider.value = volume;

        // Save pref
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // Debug
        if (debugMode) Debug.Log("Music volume set to: " + volume);
    }

    public void SetSFXVolume(float volume)
    {
        // Set mixer volume 
        audioMixer.SetFloat("SFXVolume", volume);

        // Set slider
        sfxVolumeSlider.value = volume;

        // Save pref
        PlayerPrefs.SetFloat("SFXVolume", volume);

        // Debug
        if (debugMode) Debug.Log("SFX volume set to: " + volume);
    }

    public void SetUIVolume(float volume)
    {
        // Set mixer volume 
        audioMixer.SetFloat("UIVolume", volume);

        // Set slider
        uiVolumeSlider.value = volume;

        // Save pref
        PlayerPrefs.SetFloat("UIVolume", volume);

        // Debug
        if (debugMode) Debug.Log("UI volume set to: " + volume);
    }

    public void SetQuality(int qualityIndex)
    {
        // Not used yet...

        QualitySettings.SetQualityLevel(qualityIndex);

        // Debug
        if (debugMode) Debug.Log("Quality set to: " + qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        // Get resolution from our dropdown
        Resolution resolution = resolutions[resolutionIndex];

        // Update resolution
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Set dropdown value
        resolutionsDropdown.value = resolutionIndex;

        // Save pref
        PlayerPrefs.SetInt("Resolution", resolutionIndex);

        // Debug
        if (debugMode) Debug.Log("Resolution set to: " + resolution.width + " x " + resolution.height);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // Set fullscreen
        Screen.fullScreen = isFullscreen;

        // Set state
        fullscreenToggle.isOn = isFullscreen;

        // Save pref
        int state = isFullscreen ? 1 : 0;
        PlayerPrefs.SetInt("Fullscreen", state);

        // Debug
        if (debugMode) Debug.Log("Fullscreen set to: " + isFullscreen.ToString());
    }
}
