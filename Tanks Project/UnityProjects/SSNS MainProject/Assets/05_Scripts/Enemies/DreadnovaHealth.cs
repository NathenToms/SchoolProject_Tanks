using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DreadnovaHealth : MonoBehaviour
{
    [SerializeField] Slider hpSlider;

    public void SetMaxHealth(int maxHP)
    {
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
    }

    public void SetHealth(int hp)
    {
        hpSlider.value = hp;
    }
}
