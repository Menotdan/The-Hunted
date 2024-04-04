using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuControl : MenuSection
{
    [SerializeField] private Button audio_panel_button;
    [SerializeField] private Button controls_panel_button;
    [SerializeField] private Button graphics_panel_button;
    [SerializeField] private Button back_button;

    [SerializeField] private TextMeshProUGUI audio_panel_text;
    [SerializeField] private TextMeshProUGUI controls_panel_text;
    [SerializeField] private TextMeshProUGUI graphics_panel_text;

    [SerializeField, Anywhere] private GameObject audio_panel;
    [SerializeField, Anywhere] private GameObject controls_panel;
    [SerializeField, Anywhere] private GameObject graphics_panel;

    [SerializeField] private Color default_panel_color;
    [SerializeField] private Color hightlighted_panel_color;

    private void Open_Audio_Panel()
    {
        audio_panel.SetActive(true);
        controls_panel.SetActive(false);
        graphics_panel.SetActive(false);

        audio_panel_text.color = hightlighted_panel_color;
        controls_panel_text.color = default_panel_color;
        graphics_panel_text.color = default_panel_color;
    }

    private void Open_Controls_Panel()
    {
        controls_panel.SetActive(true);
        audio_panel.SetActive(false);
        graphics_panel.SetActive(false);

        controls_panel_text.color = hightlighted_panel_color;
        audio_panel_text.color = default_panel_color;
        graphics_panel_text.color = default_panel_color;
    }

    private void Open_Graphics_Panel()
    {
        graphics_panel.SetActive(true);
        controls_panel.SetActive(false);
        audio_panel.SetActive(false);

        graphics_panel_text.color = hightlighted_panel_color;
        controls_panel_text.color = default_panel_color;
        audio_panel_text.color = default_panel_color;
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

        audio_panel_button.onClick.AddListener(Open_Audio_Panel);
        controls_panel_button.onClick.AddListener(Open_Controls_Panel);
        graphics_panel_button.onClick.AddListener(Open_Graphics_Panel);

        back_button.onClick.AddListener(MenuManager.Instance.BackMenu_Notification);
    }
}