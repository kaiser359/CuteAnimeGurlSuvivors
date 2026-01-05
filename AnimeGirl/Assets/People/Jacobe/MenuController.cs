using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController
{
    public Button LoadGameDialog_YesBtn;

    // Volume Setting
    private readonly TMP_Text volumeTextValue;
    private readonly Slider volumeSlider;
    private readonly float defaultVolume = 1.0f;

    // Gameplay Settings
    private readonly TMP_Text SensitvityTextValue;
    private readonly Slider SensitvitySlider;
    private readonly int defaultSensitvity = 4;
    public int mainSensitivity = 4;

    // Toggle settings
    private readonly Toggle invertYToggle;

    // Graphics Settings
    private readonly Slider brightnessSlider;
    private readonly TMP_Text brightnessTextValue;
    private readonly float defaultBrightness = 1;

    private readonly TMP_Dropdown qualityDropdown;
    private readonly Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    // Confirmation
    private readonly GameObject comfirmationPrompt;
    private float _confirmationTimer;
    private bool _confirmationActive;

    // Worlds To Load
    public string _newGameWorld;
    private string WorldToLoad;
    private readonly GameObject noSavedGameDialog;

    // Resolution Dropdowns
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    public float volume;

    public chooseChar chooseChar;

    public Button LoadGameBtn;

    // Constructor - supply dependencies from a MonoBehaviour or factory
    public MenuController(
        TMP_Text volumeTextValue,
        Slider volumeSlider,
        TMP_Text sensitText,
        Slider sensitSlider,
        Toggle invertYToggle,
        Slider brightnessSlider,
        TMP_Text brightnessTextValue,
        TMP_Dropdown qualityDropdown,
        Toggle fullScreenToggle,
        GameObject comfirmationPrompt,
        GameObject noSavedGameDialog,
        TMP_Dropdown resolutionDropdown,
        chooseChar chooseChar)
    {
        this.volumeTextValue = volumeTextValue;
        this.volumeSlider = volumeSlider;
        this.SensitvityTextValue = sensitText;
        this.SensitvitySlider = sensitSlider;
        this.invertYToggle = invertYToggle;
        this.brightnessSlider = brightnessSlider;
        this.brightnessTextValue = brightnessTextValue;
        this.qualityDropdown = qualityDropdown;
        this.fullScreenToggle = fullScreenToggle;
        this.comfirmationPrompt = comfirmationPrompt;
        this.noSavedGameDialog = noSavedGameDialog;
        this.resolutionDropdown = resolutionDropdown;
        this.chooseChar = chooseChar;
    }

    // Call from a MonoBehaviour Start to initialize UI state
    public void Initialize()
    {
        resolutions = Screen.resolutions;
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        if (volumeSlider != null)
            volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    // Call each frame from a MonoBehaviour Update to keep UI synced
    public void Tick(float deltaTime)
    {
        // handle confirmation box timing
        if (_confirmationActive)
        {
            _confirmationTimer -= deltaTime;
            if (_confirmationTimer <= 0f)
            {
                if (comfirmationPrompt != null)
                    comfirmationPrompt.SetActive(false);
                _confirmationActive = false;
            }
        }

        // update volume UI and persist
        PlayerPrefs.SetFloat("volume", volume);
        if (volumeTextValue != null)
            volumeTextValue.text = Mathf.Round(volume * 100) + "%";
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length) return;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void NewGameDialogYes()
    {
        if (chooseChar != null && chooseChar.chosen)
        {
            _newGameWorld = "SampleScene 2";
        }
        else
        {
            _newGameWorld = "SampleScene";
        }
        SceneManager.LoadScene(_newGameWorld);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedWorld"))
        {
            WorldToLoad = PlayerPrefs.GetString("SavedWorld");
            SceneManager.LoadScene(WorldToLoad);
        }
        else
        {
            if (noSavedGameDialog != null)
                noSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float volume1)
    {
        volume = volume1;
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        ShowConfirmation(2f);
    }

    public void SetSensitvity(float sensitvity)
    {
        mainSensitivity = Mathf.RoundToInt(sensitvity);
        if (SensitvityTextValue != null)
            SensitvityTextValue.text = Mathf.Round(sensitvity * 10) + "%";
    }

    public void GameplayApply()
    {
        if (invertYToggle != null)
        {
            PlayerPrefs.SetInt("masterInvertY", invertYToggle.isOn ? 1 : 0);
        }

        PlayerPrefs.SetFloat("masterSensitvity", mainSensitivity);
        ShowConfirmation(2f);
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        if (brightnessTextValue != null)
            brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        ShowConfirmation(2f);
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Graphics")
        {
            if (brightnessSlider != null) brightnessSlider.value = defaultBrightness;
            if (brightnessTextValue != null) brightnessTextValue.text = defaultBrightness.ToString("0.0");

            if (qualityDropdown != null) qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            if (fullScreenToggle != null) fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            if (resolutionDropdown != null && resolutions != null) resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            if (volumeSlider != null) volumeSlider.value = defaultVolume;
            if (volumeTextValue != null) volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            if (SensitvityTextValue != null) SensitvityTextValue.text = defaultSensitvity.ToString("0");
            if (SensitvitySlider != null) SensitvitySlider.value = defaultSensitvity;
            mainSensitivity = defaultSensitvity;
            if (invertYToggle != null) invertYToggle.isOn = false; 
            GameplayApply();
        }
    }

    // Replaces coroutine -- caller must call Tick(deltaTime) each frame
    public void ShowConfirmation(float durationSeconds)
    {
        if (comfirmationPrompt != null) comfirmationPrompt.SetActive(true);
        _confirmationTimer = durationSeconds;
        _confirmationActive = true;
    }
}