using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;

    [Header("Quality Settings")]
    [SerializeField] private Button[] textureQualityButtons;
    [SerializeField] private Button[] shadowQualityButtons;
    [SerializeField] private Button[] vsyncButtons;
    [SerializeField] private Button[] displayModeButtons;
    [SerializeField] private Button[] resolutionButtons;

    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;

    private Resolution[] resolutions;
    private int currentResolutionIndex;
    private bool settingsChanged = false;

    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string TEXTURE_QUALITY_KEY = "TextureQuality";
    private const string SHADOW_QUALITY_KEY = "ShadowQuality";
    private const string VSYNC_KEY = "VSync";
    private const string DISPLAY_MODE_KEY = "DisplayMode";
    private const string RESOLUTION_KEY = "Resolution";

    private void Awake()
    {
        resolutions = Screen.resolutions;
        LoadSettings();
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Audio Sliders
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

        // Apply and Reset buttons
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSettings);

        // Initialize all settings UI
        SetModeOptions();
        SetVsyncOptions();
        SetShadowsOptions();
        SetTextureOptions();
        SetResolutionOptions();
    }

    private void SetResolutionOptions()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            int index = i; // Local copy for closure
            if (i < resolutionButtons.Length && resolutionButtons[i] != null)
            {
                resolutionButtons[i].GetComponentInChildren<Text>().text =
                    $"{resolutions[i].width}x{resolutions[i].height}";

                resolutionButtons[i].onClick.AddListener(() =>
                {
                    currentResolutionIndex = index;
                    settingsChanged = true;
                });
            }
        }
    }

    private void SetModeOptions()
    {
        if (displayModeButtons != null && displayModeButtons.Length >= 2)
        {
            displayModeButtons[0].onClick.AddListener(() =>
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                PlayerPrefs.SetInt(DISPLAY_MODE_KEY, 0);
                settingsChanged = true;
            });

            displayModeButtons[1].onClick.AddListener(() =>
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                PlayerPrefs.SetInt(DISPLAY_MODE_KEY, 1);
                settingsChanged = true;
            });
        }
    }

    private void SetVsyncOptions()
    {
        if (vsyncButtons != null && vsyncButtons.Length >= 2)
        {
            vsyncButtons[0].onClick.AddListener(() =>
            {
                QualitySettings.vSyncCount = 1;
                PlayerPrefs.SetInt(VSYNC_KEY, 1);
                settingsChanged = true;
            });

            vsyncButtons[1].onClick.AddListener(() =>
            {
                QualitySettings.vSyncCount = 0;
                PlayerPrefs.SetInt(VSYNC_KEY, 0);
                settingsChanged = true;
            });
        }
    }

    private void SetShadowsOptions()
    {
        if (shadowQualityButtons != null && shadowQualityButtons.Length >= 3)
        {
            shadowQualityButtons[0].onClick.AddListener(() =>
            {
                SetShadowQuality(0);
                PlayerPrefs.SetInt(SHADOW_QUALITY_KEY, 0);
                settingsChanged = true;
            });

            shadowQualityButtons[1].onClick.AddListener(() =>
            {
                SetShadowQuality(1);
                PlayerPrefs.SetInt(SHADOW_QUALITY_KEY, 1);
                settingsChanged = true;
            });

            shadowQualityButtons[2].onClick.AddListener(() =>
            {
                SetShadowQuality(2);
                PlayerPrefs.SetInt(SHADOW_QUALITY_KEY, 2);
                settingsChanged = true;
            });
        }
    }

    private void SetTextureOptions()
    {
        if (textureQualityButtons != null && textureQualityButtons.Length >= 3)
        {
            textureQualityButtons[0].onClick.AddListener(() =>
            {
                SetTextureQuality(4);
                PlayerPrefs.SetInt(TEXTURE_QUALITY_KEY, 4);
                settingsChanged = true;
            });

            textureQualityButtons[1].onClick.AddListener(() =>
            {
                SetTextureQuality(2);
                PlayerPrefs.SetInt(TEXTURE_QUALITY_KEY, 2);
                settingsChanged = true;
            });

            textureQualityButtons[2].onClick.AddListener(() =>
            {
                SetTextureQuality(0);
                PlayerPrefs.SetInt(TEXTURE_QUALITY_KEY, 0);
                settingsChanged = true;
            });
        }
    }

    public void SetShadowQuality(int level)
    {
        switch (level)
        {
            case 0: // Off
                QualitySettings.shadowCascades = 0;
                QualitySettings.shadowDistance = 0f;
                QualitySettings.shadows = ShadowQuality.Disable;
                break;

            case 1: // Medium
                QualitySettings.shadowCascades = 2;
                QualitySettings.shadowDistance = 200f;
                QualitySettings.shadows = ShadowQuality.HardOnly;
                break;

            case 2: // High
                QualitySettings.shadowCascades = 3;
                QualitySettings.shadowDistance = 300f;
                QualitySettings.shadows = ShadowQuality.HardOnly;
                break;
        }
    }

    public void SetTextureQuality(int level)
    {
        QualitySettings.globalTextureMipmapLimit = level;
    }

    public void SetMasterVolume(float volume)
    {
        if(audioMixer is not null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat(MASTER_VOL_KEY, volume);
            settingsChanged = true;
        }
    }

    public void ApplySettings()
    {
        // Apply resolution
        Screen.SetResolution(
            resolutions[currentResolutionIndex].width,
            resolutions[currentResolutionIndex].height,
            Screen.fullScreenMode
        );

        PlayerPrefs.SetInt(RESOLUTION_KEY, currentResolutionIndex);
        PlayerPrefs.Save();
        settingsChanged = false;
        ToggleSettingsPanel();
    }

    public void ResetSettings()
    {
        // Reset to default settings
        SetMasterVolume(1f);

        if (masterVolumeSlider != null) masterVolumeSlider.value = 1f;

        SetTextureQuality(0);
        SetShadowQuality(2);
        QualitySettings.vSyncCount = 1;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        // Reset resolution to native
        currentResolutionIndex = resolutions.Length - 1;
        Screen.SetResolution(
            resolutions[currentResolutionIndex].width,
            resolutions[currentResolutionIndex].height,
            Screen.fullScreenMode
        );

        // Save defaults
        PlayerPrefs.SetInt(TEXTURE_QUALITY_KEY, 0);
        PlayerPrefs.SetInt(SHADOW_QUALITY_KEY, 2);
        PlayerPrefs.SetInt(VSYNC_KEY, 1);
        PlayerPrefs.SetInt(DISPLAY_MODE_KEY, 0);
        PlayerPrefs.SetInt(RESOLUTION_KEY, currentResolutionIndex);
        PlayerPrefs.SetFloat(MASTER_VOL_KEY, 1f);
        PlayerPrefs.Save();

        settingsChanged = false;
        ToggleSettingsPanel();
    }

    private void LoadSettings()
    {
        // Load audio settings
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f);
        SetMasterVolume(masterVol);

        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVol;

        // Load quality settings
        int textureQuality = PlayerPrefs.GetInt(TEXTURE_QUALITY_KEY, 0);
        int shadowQuality = PlayerPrefs.GetInt(SHADOW_QUALITY_KEY, 2);
        int vsync = PlayerPrefs.GetInt(VSYNC_KEY, 1);
        int displayMode = PlayerPrefs.GetInt(DISPLAY_MODE_KEY, 0);
        currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_KEY, resolutions.Length - 1);

        SetTextureQuality(textureQuality);
        SetShadowQuality(shadowQuality);
        QualitySettings.vSyncCount = vsync;
        Screen.fullScreenMode = (FullScreenMode)displayMode;
        Screen.SetResolution(
            resolutions[currentResolutionIndex].width,
            resolutions[currentResolutionIndex].height,
            Screen.fullScreenMode
        );
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);

            if (settingsPanel.activeSelf)
            {
                LoadSettings(); // Refresh settings when panel is opened
            }
        }
    }
}