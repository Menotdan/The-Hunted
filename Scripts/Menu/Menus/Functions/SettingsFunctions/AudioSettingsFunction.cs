using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsFunction : SettingsFunction
{
    [SerializeField] private Slider master_volume_slider;
    // [SerializeField] private Slider sfx_volume_slider;

    [SerializeField] private TextMeshProUGUI master_volume_percent_text;
    // [SerializeField] private TextMeshProUGUI sfx_volume_percent_text;


    public override void Function_Loaded()
    {
        master_volume_slider.maxValue = 1f;
        master_volume_slider.minValue = 0f;

        // Update the game settings when the slider changes.
        base.Function_Loaded();
        master_volume_slider.onValueChanged.AddListener(GameSettingsManager.Instance.Master_Volume_Update);
    }

    public override void Settings_Updated()
    {
        base.Settings_Updated();
        float new_volume = GameSettingsManager.game_volume;
        master_volume_slider.value = new_volume;

        float volume_percent = Mathf.FloorToInt(new_volume * 100);
        master_volume_percent_text.text = $"{volume_percent}%";
    }
}
