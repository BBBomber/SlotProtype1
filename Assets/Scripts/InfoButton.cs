using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    public Button infoButton;
   
    void Start()
    {
        
        infoButton = GetComponent<Button>();
        infoButton.onClick.AddListener(SwitchOff);
    }

    private void SwitchOff()
    {
        FindObjectOfType<ImageCycler>().InfoUI();
    }

  
}
