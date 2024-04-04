using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatheredSpiritMeterUI : MonoBehaviour
{
    public static GatheredSpiritMeterUI Instance { get; private set; }
    [SerializeField] public Slider health_slider;
    [SerializeField] public Image heart_icon;
    [SerializeField] public Sprite health_normal_icon;
    [SerializeField] public Sprite health_low_icon;

    private void Awake()
    {
        Instance = this;
    }
}
