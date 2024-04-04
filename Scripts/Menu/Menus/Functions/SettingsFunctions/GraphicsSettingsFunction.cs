using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsFunction : SettingsFunction
{
    [SerializeField] private TMP_Dropdown resolution_dropdown;
    [SerializeField] private TMP_Dropdown quality_dropdown;
    [SerializeField] private Slider brightness_slider;
    [SerializeField] private TextMeshProUGUI brightness_percent_text;
    [SerializeField] private Toggle fullscreen_toggle;

    public override void Function_Loaded()
    {
        resolution_dropdown.ClearOptions();
        quality_dropdown.ClearOptions();

        resolution_dropdown.AddOptions(GameSettingsManager.resolution_name_list);
        resolution_dropdown.RefreshShownValue();

        quality_dropdown.AddOptions(GameSettingsManager.quality_name_list);
        quality_dropdown.RefreshShownValue();

        brightness_slider.minValue = GameSettingsManager.brightness_min;
        brightness_slider.maxValue = GameSettingsManager.brightness_max;

        // Run base code here because it calls Settings_Updated() which relies
        // On us having configured the values.

        base.Function_Loaded();
        resolution_dropdown.onValueChanged.AddListener(GameSettingsManager.Instance.Resolution_Update);
        quality_dropdown.onValueChanged.AddListener(GameSettingsManager.Instance.Quality_Update);
        brightness_slider.onValueChanged.AddListener(GameSettingsManager.Instance.Brightness_Update);
    }

    public override void Settings_Updated()
    {
        base.Settings_Updated();
        float new_brightness = GameSettingsManager.game_brightness;
        float min_brightness = GameSettingsManager.brightness_min;
        float max_brightness = GameSettingsManager.brightness_max;

        float brightness_percent = Mathf.FloorToInt(((new_brightness - min_brightness) / (max_brightness - min_brightness)) * 100);

        brightness_slider.value = new_brightness;
        brightness_percent_text.text = $"{brightness_percent}%";

        int new_resolution = GameSettingsManager.game_resolution;
        int new_quality = GameSettingsManager.game_quality;

        resolution_dropdown.value = new_resolution;
        quality_dropdown.value = new_quality;

        resolution_dropdown.RefreshShownValue();
        quality_dropdown.RefreshShownValue();
    }
}
