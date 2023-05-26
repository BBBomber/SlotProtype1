using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetSlider : MonoBehaviour
{

    public Slider slider;
    
    private void Start()
    {
        
        slider.onValueChanged.AddListener(UpdateBetAmount);
    }

    
    
    private void UpdateBetAmount(float value)
    {
        FindObjectOfType<ImageCycler>().StoppedSliding(value);
    }
}

