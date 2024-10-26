using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{

    public class Healthbarv2 : MonoBehaviour
    {
    // Start is called before the first frame update
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpspeed = 0.05f;
    
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(healthSlider.value != health) 
        {
            healthSlider.value = health;
        }
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            takeDamage(10);
        }

        if(healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpspeed);
        }
    }

    void takeDamage(float damage)
    {
        health -= damage;
    }
    }
}