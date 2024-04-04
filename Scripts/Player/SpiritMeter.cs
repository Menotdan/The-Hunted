using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritMeter : MonoBehaviour
{
    private const int low_hp = 25;
    private const int high_hp = 85;
    public int health = 1;

    private Slider spirit_slider;
    private Image spirit_icon;
    private Sprite spirit_normal_icon;
    private Sprite spirit_high_icon;

    void Start()
    {
        spirit_slider = GatheredSpiritMeterUI.Instance.health_slider;
        spirit_icon = GatheredSpiritMeterUI.Instance.heart_icon;
        spirit_normal_icon = GatheredSpiritMeterUI.Instance.health_normal_icon;
        spirit_high_icon = GatheredSpiritMeterUI.Instance.health_low_icon;

        spirit_slider.onValueChanged.AddListener(Update_Health);
        spirit_slider.value = health;
    }

    public void Update_Health(float new_hp)
    {
        health = Mathf.FloorToInt(new_hp);
        spirit_slider.value = new_hp;

        if (new_hp >= high_hp)
        {
            spirit_icon.sprite = spirit_high_icon;
        }
        else
        {
            spirit_icon.sprite = spirit_normal_icon;
        }
    }
}
