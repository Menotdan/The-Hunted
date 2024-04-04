using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Item_List : MonoBehaviour
{
    public static SO_Item_List Instance;
    public List<SO_Item> items_list = new List<SO_Item>();

    public void Awake()
    {
        Instance = this;
    }
}
