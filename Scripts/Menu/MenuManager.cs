using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    public List<MenuSection> menus = new List<MenuSection>();
    public MenuSection opened_menu { get; private set; } = null;
    private Stack<MenuSection> closed_menu_stack = new Stack<MenuSection>();

    public Dictionary<string, Action> event_bus = new Dictionary<string, Action>();


    public void Start()
    {
        Instance = this;
        foreach (MenuSection m in FindObjectsByType<MenuSection>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            m.StartMenu();
        }
    }

    // Clears the reference to the previous opened menu and closes that menu.
    // Then opens the new menu and calls the menu's backend open function.
    public void OpenMenu_Notification(MenuSection new_menu)
    {
        if (opened_menu != null)
        {
            opened_menu.CloseMenu();
        }

        opened_menu = new_menu;
        new_menu.Backend_Open_Menu();
    }


    // Clears the reference to the previous opened menu and closes the 'closed_menu'.
    // Then pushes the closed menu to the stack.
    public void CloseMenu_Notification(MenuSection closed_menu, bool push_to_stack)
    {
        opened_menu = null;

        closed_menu.Backend_Close_Menu();
        if (push_to_stack)
        {
            closed_menu_stack.Push(closed_menu);
        }
    }

    // Clears the stack of closed menus.
    public void Clear_Menu_Stack_Notification()
    {
        closed_menu_stack.Clear();
    }

    // Tries to pop a menu off the stack of closed menus.
    // If this fails, we try to just close the previous opened menu.
    // If it does not fail, we open the popped menu (which closes the current menu)
    public void BackMenu_Notification()
    {
        MenuSection previous_menu;
        if (!closed_menu_stack.TryPop(out previous_menu))
        {
            if (opened_menu == null)
            {
                Debug.LogWarning("No opened menu but trying to go back!");
                return;
            }

            opened_menu.CloseMenu();
            return;
        }

        previous_menu.OpenMenu();
    }

    public void RegisterMenu(MenuSection new_menu)
    {
        menus.Add(new_menu);
        new_menu.Register();
    }

    public void RegisterEvent(string bus_name, Action bus_event) {
        event_bus.Add(bus_name, bus_event);
    }

    public void FireEvent(string bus_name)
    {
        Action bus_event;
        if (event_bus.TryGetValue(bus_name, out bus_event))
        {
            bus_event();
            return;
        }

        Debug.LogWarning($"Event missing from bus '{bus_name}'!");
    }
}
