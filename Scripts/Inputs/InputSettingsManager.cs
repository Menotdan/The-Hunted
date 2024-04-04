using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSettingsManager : MonoBehaviour
{
    public static PlayerInput input_management;

    public static void register_input_action(System.Action<InputAction.CallbackContext> caller, string action_name)
    {
        InputAction action = input_management.currentActionMap.FindAction(action_name);
        action.performed += caller;
        action.canceled += caller;
    }

    public void Awake()
    {
        if (input_management == null)
        {
            input_management = GetComponent<PlayerInput>();
        }
    }

    public void Load_Controls()
    {
        string input_prefs = PlayerPrefs.GetString(GameSettingsManager.controls_key, "");
        input_management.actions.LoadBindingOverridesFromJson(input_prefs);
        MenuManager.Instance.FireEvent("update_controls");
    }

    void Start()
    {
        Load_Controls();
    }
}
