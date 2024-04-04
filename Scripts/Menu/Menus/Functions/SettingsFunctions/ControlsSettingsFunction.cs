using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsSettingsFunction : SettingsFunction
{
    [SerializeField] private Button reset_controls_button;
    [SerializeField] private GameObject keybinds_parent_object;
    [SerializeField] private Slider sensitivity_slider;
    [SerializeField] private TextMeshProUGUI sensitivity_percent_text;

    public override void Function_Loaded()
    {
        MenuManager.Instance.RegisterEvent("update_controls", Update_Controls);
        MenuManager.Instance.RegisterEvent("reset_all_controls", Reset_All_Controls);

        sensitivity_slider.minValue = GameSettingsManager.sensitivity_min;
        sensitivity_slider.maxValue = GameSettingsManager.sensitivity_max;

        base.Function_Loaded();
        reset_controls_button.onClick.AddListener(Reset_All_Controls);
        sensitivity_slider.onValueChanged.AddListener(GameSettingsManager.Instance.Sensitivity_Update);
    }

    public override void Settings_Updated()
    {
        base.Settings_Updated();
        float new_sensitivity = GameSettingsManager.look_sensitivity;
        sensitivity_slider.value = new_sensitivity;

        float min_sensitivity = GameSettingsManager.sensitivity_min;
        float max_sensitivity = GameSettingsManager.sensitivity_max;

        int sensitivity_percent = Mathf.RoundToInt(((new_sensitivity - min_sensitivity) / max_sensitivity) * 100);
        sensitivity_percent_text.text = $"{sensitivity_percent}%";
    }

    public void Update_Controls()
    {
        RebindableKey[] keybinds = keybinds_parent_object.GetComponentsInChildren<RebindableKey>();
        foreach (RebindableKey key in keybinds)
        {
            key.Refresh_Bindings();
        }
    }

    public void Reset_All_Controls()
    {
        RebindableKey[] keybinds = keybinds_parent_object.GetComponentsInChildren<RebindableKey>();
        foreach (RebindableKey key in keybinds)
        {
            key.Reset_Control();
        }
    }
}
