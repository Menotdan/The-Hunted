using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindableKey : MonoBehaviour
{
    [SerializeField] public InputActionReference action;
    [SerializeField] private GameObject waiting_for_new_key;
    [SerializeField] private GameObject set_input_button;
    [SerializeField] private TMP_Text keybind_display;
    [SerializeField] private PlayerInput input_management;

    [SerializeField] private Button reset_control_button;
    [SerializeField] private Button bind_control_button;

    private InputActionRebindingExtensions.RebindingOperation rebinding_operation = null;

    public void Awake()
    {
        reset_control_button.onClick.AddListener(Reset_Control);
        bind_control_button.onClick.AddListener(Start_Rebind);
    }

    public void Start_Rebind()
    {
        waiting_for_new_key.SetActive(true);
        set_input_button.SetActive(false);

        input_management.SwitchCurrentActionMap("Menu");
        rebinding_operation = action.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation => End_Rebind())
            .OnCancel(operation => Cancel_Rebind())
            .Start();
    }

    public void Save_Binding_Changes()
    {
        string overrides = input_management.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(GameSettingsManager.controls_key, overrides);
    }

    void End_Rebind()
    {
        int binding_index = action.action.GetBindingIndexForControl(action.action.controls[0]);
        keybind_display.text = InputControlPath.ToHumanReadableString(action.action.bindings[binding_index].effectivePath, 
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        Save_Binding_Changes();

        rebinding_operation.Dispose();
        rebinding_operation = null;

        waiting_for_new_key.SetActive(false);
        set_input_button.SetActive(true);

        input_management.SwitchCurrentActionMap("PlayerInput");
    }

    void Cancel_Rebind()
    {
        rebinding_operation.Dispose();
        rebinding_operation = null;

        waiting_for_new_key.SetActive(false);
        set_input_button.SetActive(true);

        input_management.SwitchCurrentActionMap("PlayerInput");
    }

    public void Refresh_Bindings()
    {
        string control_path = null;
        string device_layout = null;


        int binding_index = action.action.GetBindingIndexForControl(action.action.controls[0]);
        string display_name = action.action.GetBindingDisplayString(binding_index, out device_layout, out control_path, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);

        keybind_display.text = display_name;
    }

    public void Reset_Control()
    {
        int binding_index = action.action.GetBindingIndexForControl(action.action.controls[0]);
        action.action.RemoveBindingOverride(binding_index);
        Save_Binding_Changes();

        keybind_display.text = InputControlPath.ToHumanReadableString(action.action.bindings[binding_index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}
