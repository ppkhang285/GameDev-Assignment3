using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBarSlider;
    public Text healthBarValueText;
    public int maxHealth;
    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        healthBarValueText.text = currentHealth.ToString()+"/"+maxHealth.ToString();
        healthBarSlider.value = currentHealth;
        healthBarSlider.maxValue = maxHealth;

    }
}
