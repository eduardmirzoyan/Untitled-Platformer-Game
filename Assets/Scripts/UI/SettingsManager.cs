using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;

    private Resolution[] resolutions;
    private bool isOpen;

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
        // Cache possible resolutions based on hardware
        resolutions = Screen.resolutions;

        // Clear any options
        resolutionsDropdown.ClearOptions();

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

        // Load any previous settings
        LoadSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            // Close settings
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

        // Save pref
        PlayerPrefs.SetFloat("MasterVolume", volume);

        // Debug
        Debug.Log("Master volume set to: " + volume);
    }

    public void SetMusicVolume(float volume)
    {
        // Set mixer volume 
        audioMixer.SetFloat("MusicVolume", volume);

        // Save pref
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // Debug
        Debug.Log("Music volume set to: " + volume);
    }

    public void SetSFXVolume(float volume)
    {
        // Set mixer volume 
        audioMixer.SetFloat("SFXVolume", volume);

        // Save pref
        PlayerPrefs.SetFloat("SFXVolume", volume);

        // Debug
        Debug.Log("SFX volume set to: " + volume);
    }

    public void SetUIVolume(float volume)
    {
        // Set mixer volume 
        audioMixer.SetFloat("UIVolume", volume);

        // Save pref
        PlayerPrefs.SetFloat("UIVolume", volume);

        // Debug
        Debug.Log("UI volume set to: " + volume);
    }

    public void SetQuality(int qualityIndex)
    {
        // Not used yet...

        QualitySettings.SetQualityLevel(qualityIndex);

        // Debug
        Debug.Log("Quality set to: " + qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        // Get resolution from our dropdown
        Resolution resolution = resolutions[resolutionIndex];

        // Update resolution
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Save pref
        PlayerPrefs.SetInt("Resolution", resolutionIndex);

        // Debug
        Debug.Log("Resolution set to: " + resolution.width + " x " + resolution.height);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // Set fullscreen
        Screen.fullScreen = isFullscreen;

        // Save pref
        int state = isFullscreen ? 1 : 0;
        PlayerPrefs.SetInt("Fullscreen", state);

        // Debug
        Debug.Log("Fullscreen set to: " + isFullscreen.ToString());
    }
}
