using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class PaylinesDropdown : MonoBehaviour
{
    [SerializeField] private int PaylinesAmount;



    public void Dropdown(int index)
    {
        switch (index)
        {
            case 0: PaylinesAmount = 1; break;
            case 1: PaylinesAmount = 2; break;
            case 2: PaylinesAmount = 3; break;
            case 3: PaylinesAmount = 4; break;
            case 4: PaylinesAmount = 5; break;
            case 5: PaylinesAmount = 6; break;
            case 6: PaylinesAmount = 7; break;
            case 7: PaylinesAmount = 8; break;
            case 8: PaylinesAmount = 9; break;
            case 9: PaylinesAmount = 10; break;

        }


        UpdateBetAmount(PaylinesAmount);
    }

    private void UpdateBetAmount(int value)
    {
        FindObjectOfType<ImageCycler>().BetAmountUpdated(value);
    }


}            
    
   
        
            
          
    

        

        
   

