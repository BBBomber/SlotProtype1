using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    public Button infoButton;

    

    public Sprite exit;
    public Sprite initial;
   
    void Start()
    {
        infoButton.image.sprite = initial;
        infoButton = GetComponent<Button>();
        infoButton.onClick.AddListener(SwitchOff);
    }

    private void SwitchOff()
    {
        FindObjectOfType<ImageCycler>().InfoUI();

        if(infoButton.image.sprite == initial)
        {
            infoButton.image.sprite = exit;
        }
        else if (infoButton.image.sprite == exit)
        {
            infoButton.image.sprite = initial;
        }
        
    }

    
  
}
