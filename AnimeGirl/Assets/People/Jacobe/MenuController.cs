using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Xml.Linq;

public class MenuController : MonoBehaviour
{
    public Button LoadGameDialog_YesBtn;

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text SensitvityTextValue = null;
    [SerializeField] private Slider SensitvitySlider = null;
    [SerializeField] private int defaultSensitvity = 4;
    public int mainSensitivity = 4;

    [Header("Toggle settings")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 1;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;


    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("Worlds To Load")]
    public string _newGameWorld;
    private string WorldToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    public float volume;

    public chooseChar chooseChar;
    //public TMP_Text volumeText;

    private void Start()
    {
        resolutions = Screen.resolutions;
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

        volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    private void Update()
    {
        PlayerPrefs.SetFloat("volume", volume);
        volumeTextValue.text = Mathf.Round(volume * 100) + "%";
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void NewGameDialogYes()
    {if (chooseChar.chosen)
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
            //PlayerPrefs.SetString("SavedLevel", whateveryourlevelis)
            SceneManager.LoadScene(WorldToLoad);
        }
        else
        {
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
        StartCoroutine(ConfirmationBox());
    }

    public void SetSensitvity(float sensitvity)
    {
        mainSensitivity = Mathf.RoundToInt(sensitvity);
        SensitvityTextValue.text = Mathf.Round(sensitvity * 10) + "%";
    }

    public void GameplayApply()
    {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
            //invertY
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
            //Not invert
        }

        PlayerPrefs.SetFloat("masterSensitvity", mainSensitivity);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
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
        // change your brightness with post processing

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {

        if (MenuType == "Graphics")
        {
            //reset brightness value
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            SensitvityTextValue.text = defaultSensitvity.ToString("0");
            SensitvitySlider.value = defaultSensitvity;
            mainSensitivity = defaultSensitvity;
            invertYToggle.isOn = false;
            GameplayApply();
        }
    }

    public Button LoadGameBtn;


    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}