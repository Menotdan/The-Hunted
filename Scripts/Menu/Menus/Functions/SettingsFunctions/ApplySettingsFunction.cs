using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ApplySettingsFunction : SettingsFunction
{
    [SerializeField] private Button apply_settings_button;
    [SerializeField] private Button reset_settings_button;

    public override void Function_Loaded()
    {
        base.Function_Loaded();

        apply_settings_button.onClick.AddListener(GameSettingsManager.Instance.Apply_Settings);
        reset_settings_button.onClick.AddListener(GameSettingsManager.Instance.Reset_Settings);
    }
}
