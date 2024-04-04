using KBCore.Refs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MenuSection : MonoBehaviour
{
    public string menu_name;

    public virtual void Register()
    {
        foreach (MenuFunction function in GetComponents(typeof(MenuFunction)))
        {
            function.Function_Loaded();
        }
    }

    /* Frontend Functions For Calling Back To The Menu Manager */
    public virtual void Back_Menu_Stack()
    {
        MenuManager.Instance.BackMenu_Notification();
    }

    public virtual void CloseMenu()
    {
        MenuManager.Instance.CloseMenu_Notification(this, true);
    }

    public virtual void ClearMenu()
    {
        MenuManager.Instance.CloseMenu_Notification(this, false);
        MenuManager.Instance.Clear_Menu_Stack_Notification();
    }

    public virtual void OpenMenu()
    {
        MenuManager.Instance.OpenMenu_Notification(this);
    }

    /* Backend Functions For Actually Performing Actions Requested By The Menu Manager */
    public virtual void Backend_Close_Menu()
    {
        // Basic code for closing the menu
        gameObject.SetActive(false);
    }

    public virtual void Backend_Open_Menu()
    {
        // Basic code for opening the menu
        gameObject.SetActive(true);
    }

    /* Start The Menu Functions */
    public virtual void StartMenu()
    {
        MenuManager.Instance.RegisterMenu(this);
    }

    private void OnValidate()
    {
        this.ValidateRefs();
    }
}
