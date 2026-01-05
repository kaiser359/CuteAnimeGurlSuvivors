using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LoadPrefs
{
    public bool CanUse { get; }

    private readonly MenuController menuController;

    private readonly TMP_Text volumeTextValue;
    private readonly Slider volumeSlider;

    private readonly Slider brightnessSlider;
    private readonly TMP_Text brightnessTextValue;

    private readonly TMP_Dropdown qualityDropdown;

    private readonly Toggle fullScreenToggle;

    private readonly TMP_Text SensitvityTextValue;
    private readonly Slider SensitvitySlider;

    private readonly Toggle invertYToggle;

    public LoadPrefs(
        bool canUse,
        MenuController menuController,
        TMP_Text volumeTextValue,
        Slider volumeSlider,
        Slider brightnessSlider,
        TMP_Text brightnessTextValue,
        TMP_Dropdown qualityDropdown,
        Toggle fullScreenToggle,
        TMP_Text SensitvityTextValue,
        Slider SensitvitySlider,
        Toggle invertYToggle)
    {
        CanUse = canUse;
        this.menuController = menuController;
        this.volumeTextValue = volumeTextValue;
        this.volumeSlider = volumeSlider;
        this.brightnessSlider = brightnessSlider;
        this.brightnessTextValue = brightnessTextValue;
        this.qualityDropdown = qualityDropdown;
        this.fullScreenToggle = fullScreenToggle;
        this.SensitvityTextValue = SensitvityTextValue;
        this.SensitvitySlider = SensitvitySlider;
        this.invertYToggle = invertYToggle;
    }

    // Call this from a MonoBehaviour (e.g. a manager) when ready
    public void Load()
    {
        if (!CanUse) return;

        if (menuController == null)
            return; // dependent behaviour requires menuController; caller may handle logging

        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float localVolume = PlayerPrefs.GetFloat("masterVolume");
            if (volumeTextValue != null) volumeTextValue.text = localVolume.ToString("0.0");
            if (volumeSlider != null) volumeSlider.value = localVolume;
            AudioListener.volume = localVolume;
        }
        else
        {
            menuController.ResetButton("Audio");
        }

        if (PlayerPrefs.HasKey("masterQuality") && qualityDropdown != null)
        {
            int localQuality = PlayerPrefs.GetInt("masterQuality");
            qualityDropdown.value = localQuality;
            QualitySettings.SetQualityLevel(localQuality);
        }

        if (PlayerPrefs.HasKey("masterFullscreen") && fullScreenToggle != null)
        {
            int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");
            bool isFull = localFullscreen == 1;
            Screen.fullScreen = isFull;
            fullScreenToggle.isOn = isFull;
        }

        if (PlayerPrefs.HasKey("masterBrightness"))
        {
            float localBrightness = PlayerPrefs.GetFloat("masterBrightness");
            if (brightnessTextValue != null) brightnessTextValue.text = localBrightness.ToString("0.0");
            if (brightnessSlider != null) brightnessSlider.value = localBrightness;
        }

        if (PlayerPrefs.HasKey("masterSensitvity"))
        {
            float localSensitivity = PlayerPrefs.GetFloat("masterSensitvity");
            if (SensitvityTextValue != null) SensitvityTextValue.text = localSensitivity.ToString("0");
            if (SensitvitySlider != null) SensitvitySlider.value = localSensitivity;
            menuController.mainSensitivity = Mathf.RoundToInt(localSensitivity);
        }

        if (PlayerPrefs.HasKey("masterInvertY") && invertYToggle != null)
        {
            invertYToggle.isOn = (PlayerPrefs.GetInt("masterInvertY") == 1);
        }
    }
}