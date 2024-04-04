using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private const int low_hp = 25;
    private const int high_hp = 85;
    public int health = 25;

    private Slider health_slider;
    private Image heart_icon;
    private Sprite health_normal_icon;
    private Sprite health_low_icon;

    void Start()
    {
        health_slider = GatheredHealthUI.Instance.health_slider;
        heart_icon = GatheredHealthUI.Instance.heart_icon;
        health_normal_icon = GatheredHealthUI.Instance.health_normal_icon;
        health_low_icon = GatheredHealthUI.Instance.health_low_icon;

        health_slider.onValueChanged.AddListener(Update_Health);
        health_slider.value = health;
    }

    public void Update_Health(float new_hp)
    {
        health = Mathf.FloorToInt(new_hp);
        health_slider.value = new_hp;

        if (new_hp <= low_hp)
        {
            heart_icon.sprite = health_low_icon;
        }
        else
        {
            heart_icon.sprite = health_normal_icon;
        }
    }
}
