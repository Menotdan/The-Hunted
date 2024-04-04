using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuControl : MenuSection
{
    [SerializeField] private bool is_main_menu = false;
    [SerializeField, Anywhere] private Button resume_button;
    [SerializeField, Anywhere] private Button exit_game_button;
    [SerializeField, Anywhere] private Button options_menu_button;
    [SerializeField, Anywhere] private MenuSection options_menu;

    public void Open_Options_Menu()
    {
        options_menu.OpenMenu();
    }

    public void Open_Pause_Menu(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }

        if (MenuManager.Instance.opened_menu == this)
        {
            CloseMenu();
        } else
        {
            OpenMenu();
        }
    }

    public override void OpenMenu()
    {
        base.OpenMenu();
        MenuState.is_paused = true;
    }

    public override void CloseMenu()
    {
        base.CloseMenu();
        MenuState.is_paused = false;
    }

    public override void StartMenu()
    {
        base.StartMenu();
        options_menu_button.onClick.AddListener(Open_Options_Menu);

        if (!is_main_menu)
        {
            resume_button.onClick.AddListener(CloseMenu);
            InputSettingsManager.register_input_action(Open_Pause_Menu, "Menu");
        } else
        {
            // Register play game option on the main menu.
        }
    }
}
