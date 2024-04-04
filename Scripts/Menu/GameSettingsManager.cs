using Assets.Scripts.Extras;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;

    public bool settings_loaded = false;

    public static float game_volume;
    public static float game_brightness;
    public static float look_sensitivity;
    public static int game_resolution;
    public static int game_quality;
    public static bool game_fullscreen;
    public static List<Resolution> resolution_list;
    public static List<QualityOption> quality_list;
    public static List<string> resolution_name_list;
    public static List<string> quality_name_list;

    [SerializeField] private VolumeProfile brightness_volume;
    private ColorAdjustments brightness_controller;

    /* Events */
    public static Action settings_update;

    /* Constants */
    private const string volume_key = "VolumeLevel";
    private const string brightness_key = "Brightness";
    private const string resolution_key = "Resolution";
    private const string sensitivity_key = "Sensitivity";
    private const string quality_key = "QualityLevel";
    public const string controls_key = "ControlsOptions";

    public const float brightness_max = 2.0f;
    public const float brightness_min = 0f;
    public const float sensitivity_max = 2.0f;
    public const float sensitivity_min = 0f;

    public const float brightness_default = 1.0f;
    public const float sensitivity_default = 1.0f;
    public const float game_volume_default = 1.0f;
    public const int resolution_default = 0;
    public const int quality_default = 0;
    public const bool fullscreen_default = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Ensure_Settings_Setup();
        }
        else
        {
            Debug.LogWarning("Instance of singleton duplicated!");
        }
    }

    public void Ensure_Settings_Setup()
    {
        if (settings_loaded)
        {
            return;
        }

        /* Setup Brightness Controls */
        foreach (VolumeComponent component in brightness_volume.components)
        {
            if (component.name == "ColorAdjustments")
            {
                brightness_controller = (ColorAdjustments)component;
                break;
            }
        }

        Load_Video_Options();
        Load_Settings();
        settings_loaded = true;
    }

    private void Settings_Updated()
    {
        if (settings_update != null)
        {
            settings_update();
        }
    }

    void Load_Video_Options()
    {
        /* Load Potential Video Options */
        resolution_name_list = new List<string>();
        quality_name_list = new List<string>();

        resolution_list = new List<Resolution>();
        quality_list = new List<QualityOption>();

        int quality_index = 0;
        foreach (string n in QualitySettings.names)
        {
            quality_list.Add(new QualityOption(n, quality_index));
            quality_name_list.Add(n);
            quality_index++;
        }

        foreach (Resolution r in Screen.resolutions)
        {
            resolution_list.Add(r);
            resolution_name_list.Add(r.ToString());
        }
    }

    /* --- Update Functions --- */
    public void Master_Volume_Update(float val)
    {
        game_volume = val;
        AudioListener.volume = val;

        Settings_Updated();
    }

    public void SFX_Volume_Update(float val)
    {
        Settings_Updated();
        return;
    }

    public void Brightness_Update(float val)
    {
        if (val < brightness_min)
        {
            val = brightness_min;
        }

        if (val > brightness_max)
        {
            val = brightness_max;
        }

        game_brightness = val;
        brightness_controller.postExposure.value = game_brightness - 1f;

        Settings_Updated();
    }

    public void Resolution_Update(int new_selection)
    {
        if (new_selection - 1 > resolution_list.Count)
        {
            PlayerPrefs.DeleteKey(resolution_key);
            game_resolution = resolution_default;
            Screen.SetResolution(resolution_list[resolution_default].width, resolution_list[resolution_default].height, game_fullscreen);
        }

        game_resolution = new_selection;
        Screen.SetResolution(resolution_list[new_selection].width, resolution_list[new_selection].height, game_fullscreen);

        Settings_Updated();
    }

    public void Sensitivity_Update(float new_value)
    {
        look_sensitivity = new_value;
        if (new_value < sensitivity_min)
        {
            new_value = sensitivity_min;
        }

        if (new_value > sensitivity_max)
        {
            new_value = sensitivity_max;
        }

        Settings_Updated();
    }

    public void Quality_Update(int new_selection)
    {
        if (new_selection - 1 > quality_list.Count)
        {
            PlayerPrefs.DeleteKey(quality_key);
            game_quality = quality_default;
            QualitySettings.SetQualityLevel(game_quality);
        }

        game_quality = new_selection;
        QualitySettings.SetQualityLevel(game_quality);

        Settings_Updated();
    }

    /* Utility Settings Functions */
    void Load_Settings()
    {
        float volume = PlayerPrefs.GetFloat(volume_key, game_volume_default);
        Master_Volume_Update(volume);

        float brightness = PlayerPrefs.GetFloat(brightness_key, brightness_default);
        Brightness_Update(brightness);

        int resolution = PlayerPrefs.GetInt(resolution_key, resolution_default);
        Debug.Log("Resolution: " + resolution);
        Resolution_Update(resolution);

        int quality = PlayerPrefs.GetInt(quality_key, quality_default);
        Quality_Update(quality);

        float sensitivity = PlayerPrefs.GetFloat(sensitivity_key, sensitivity_default);
        Sensitivity_Update(sensitivity);

        Settings_Updated();
    }

    public void Apply_Settings()
    {
        PlayerPrefs.SetFloat(volume_key, game_volume);
        PlayerPrefs.SetFloat(brightness_key, game_brightness);
        PlayerPrefs.SetFloat(sensitivity_key, look_sensitivity);
        PlayerPrefs.SetInt(quality_key, game_quality);
        PlayerPrefs.SetInt(resolution_key, game_resolution);
    }

    public void Reset_Settings()
    {
        // Clear all keys to revert to defaults defined in this class.
        PlayerPrefs.DeleteKey(volume_key);
        PlayerPrefs.DeleteKey(brightness_key);
        PlayerPrefs.DeleteKey(quality_key);
        PlayerPrefs.DeleteKey(resolution_key);
        PlayerPrefs.DeleteKey(sensitivity_key);

        MenuManager.Instance.FireEvent("reset_all_controls");
        Load_Settings();
    }
}
