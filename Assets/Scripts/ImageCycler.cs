using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Globalization;
using TMPro;
using System.CodeDom;
//using System.Linq;

public class ImageCycler : MonoBehaviour
{
    public Image[] imageComponents; // Reference to the image components in the scene
    public Sprite[] imageArray; // Array of images/symbols to cycle through
    public float cycleTime = 3.0f; // Total time for the cycling animation
    private bool spinning = false; // Flag to indicate if spinning is in progress 
    private Dictionary<Sprite, int> SpriteIndex = new Dictionary<Sprite, int>(); //dict from sprite to int for checking payouts
    private Dictionary<List<int>, double> WinConPayoutIndex = new Dictionary<List<int>, double>();// dict from a list of win conditions to their respective payout multipliers


    private List<int>[] CurrentWinLists = new List<int>[10]; // array of max 3 lists which might be created after a spin stops to be used as keys in the above dict.

    private int[][] winMatrix = new int[5][];//create an empty win matrix of 5 column arrays populate after spinning has stopped
    public double betAmount = 10;
    public double totalCredits = 100;
    private double[] payMultipliers = new double[10];
    
    // accessing elements will be done in a transposed manner. matrix[columnIndex][rowIndex] will give the value in the specified column and row, 
    public TMP_Text Credits;
    public TMP_Text LastWin;
    public TMP_Text BetAmount;
    

    //insert coins text animation
    public TMP_Text InsertMoreCoins;
   
    //button/slider refs
    public Slider BetSlider; 
    public Button spinButton;
    public Button infoButton;
    private bool buttonClicked = false;

    //audio stuff
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    private AudioSource audioSource1;
    private AudioSource audioSource2;

    //mask object
    public GameObject ImageMask;

    //amount of paylines being bet on
    [SerializeField] private int PaylinesAmount;
   

    private void Awake()
    {
        PaylinesAmount = 1;
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource1.clip = audioClip1;

        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.clip = audioClip2;

        //assign int to each image
        SpriteIndex.Add(imageArray[0], 1);
        SpriteIndex.Add(imageArray[1], 2);
        SpriteIndex.Add(imageArray[2], 3);
        SpriteIndex.Add(imageArray[3], 4);
        SpriteIndex.Add(imageArray[4], 5);
        SpriteIndex.Add(imageArray[5], 6);
        SpriteIndex.Add(imageArray[6], 7);

        //dict of winning combinations to their payout multipliers
        WinConPayoutIndex.Add(new List<int> { 1, 1, 1 }, 0.35);
        WinConPayoutIndex.Add(new List<int> { 2, 2, 2 }, 0.7);
        WinConPayoutIndex.Add(new List<int> { 3, 3, 3 }, 1.05);
        WinConPayoutIndex.Add(new List<int> { 4, 4, 4 }, 1.4);
        WinConPayoutIndex.Add(new List<int> { 5, 5, 5 }, 1.75);
        WinConPayoutIndex.Add(new List<int> { 6, 6, 6 }, 2.1);
        WinConPayoutIndex.Add(new List<int> { 7, 7, 7 }, 2.45);
        WinConPayoutIndex.Add(new List<int> { 1, 1, 1, 1 }, 1.1);
        WinConPayoutIndex.Add(new List<int> { 2, 2, 2, 2 }, 2.2);
        WinConPayoutIndex.Add(new List<int> { 3, 3, 3, 3 }, 3.3);
        WinConPayoutIndex.Add(new List<int> { 4, 4, 4, 4 }, 4.4);
        WinConPayoutIndex.Add(new List<int> { 5, 5, 5, 5 }, 5.5);
        WinConPayoutIndex.Add(new List<int> { 6, 6, 6, 6 }, 6.6);
        WinConPayoutIndex.Add(new List<int> { 7, 7, 7, 7 }, 7.7);
        WinConPayoutIndex.Add(new List<int> { 1, 1, 1, 1, 1 }, 1.5);
        WinConPayoutIndex.Add(new List<int> { 2, 2, 2, 2, 2 }, 3);
        WinConPayoutIndex.Add(new List<int> { 3, 3, 3, 3, 3 }, 4.5);
        WinConPayoutIndex.Add(new List<int> { 4, 4, 4, 4, 4 }, 6);
        WinConPayoutIndex.Add(new List<int> { 5, 5, 5, 5, 5 }, 7.5);
        WinConPayoutIndex.Add(new List<int> { 6, 6, 6, 6, 6 }, 9);
        WinConPayoutIndex.Add(new List<int> { 7, 7, 7, 7, 7 }, 10.5);

        //setting text
        Credits.text = "Credits: " + totalCredits;
        LastWin.text = "Win Amount: " + 0;
        BetAmount.text = "Bet Amount: " + betAmount;
        InsertMoreCoins.text = "Insert More Coins..";
        
        //for insert more coins anim
        InsertMoreCoins.alpha = 0;
        
        InsertMoreCoins.enabled = false;
        

    }
    private void Start()
    {
        

        


        // Set initial images/symbol for image components
        for (int i = 0; i < imageComponents.Length; i++)
        {
            imageComponents[i].sprite = GetRandomImage();
        }
        //check if player has the money to play
        BrokeOrNot();
    }

    public void StoppedSliding(float value)
    {
        
        betAmount = value;
        BetAmount.text = "Bet Amount: " + betAmount;
        BrokeOrNot();
    }

    public void BetAmountUpdated(int value)
    {
        PaylinesAmount = value;
        Debug.Log(PaylinesAmount);
    }

    //gets called when player clicks on spin button
    public void StartSpinning()
    {
        if (!spinning)
        {
            Button spinButton = FindObjectOfType<SpinButton>().GetComponent<Button>();
            
            //clean unneccesary visual and audio effects
            audioSource1.Stop();
            audioSource2.Stop();

            //disable buttons
            spinButton.interactable = false;
            BetSlider.interactable = false;
            infoButton.interactable = false;

            //subtract bet amount from creds and set text
            totalCredits = totalCredits - betAmount;
            Credits.text = "Credits: " + totalCredits;

            spinning = true;
            SpinImages();
        }
    }

    private void SpinImages()
    {
        
        ImageMask.SetActive(true);
        

        




        // Update the images for each component 
        for (int i = 0; i < imageComponents.Length; i++)
        {

            imageComponents[i].sprite = GetRandomImage();

        }
        
        //call endspin function in x time
        Invoke("EndSpin", 3f);
       


        




    }

    private Sprite GetRandomImage()//set random image fn
    {
        int randomIndex = Random.Range(0, imageArray.Length);
        return imageArray[randomIndex];
    }

    private void SetWinMatrix()
    {
        //after spin ends, 
        for (int i = 0; i < 4; i++)
        {
            winMatrix[i] = new int[3];
        }


        winMatrix[0] = new int[] { SpriteIndex[imageComponents[0].sprite], SpriteIndex[imageComponents[1].sprite], SpriteIndex[imageComponents[2].sprite] };
        winMatrix[1] = new int[] { SpriteIndex[imageComponents[3].sprite], SpriteIndex[imageComponents[4].sprite], SpriteIndex[imageComponents[5].sprite] };
        winMatrix[2] = new int[] { SpriteIndex[imageComponents[6].sprite], SpriteIndex[imageComponents[7].sprite], SpriteIndex[imageComponents[8].sprite] };
        winMatrix[3] = new int[] { SpriteIndex[imageComponents[9].sprite], SpriteIndex[imageComponents[10].sprite], SpriteIndex[imageComponents[11].sprite] };
        winMatrix[4] = new int[] { SpriteIndex[imageComponents[12].sprite], SpriteIndex[imageComponents[13].sprite], SpriteIndex[imageComponents[14].sprite] };





        PaylineWinChecker();

        //create win lists
        /*
        WinListChecker(winMatrix[0][0], 0);
        WinListChecker(winMatrix[0][1], 1);
        WinListChecker(winMatrix[0][2], 2);
        */


        

    }

    private void PaylineWinChecker()
    {
        Payline1(winMatrix[0][1], 0);
        if (PaylinesAmount > 1)
        {
            Payline2(winMatrix[0][0], 1);

            if(PaylinesAmount > 2)
            {
                Payline3(winMatrix[0][2], 2);

                if(PaylinesAmount > 3)
                {
                    Payline4(winMatrix[0][0], 3);

                    if(PaylinesAmount > 4)
                    {
                        Payline5(winMatrix[0][2], 4);

                        if(PaylinesAmount > 5)
                        {
                            Payline6(winMatrix[0][1], 5);

                            if(PaylinesAmount > 6)
                            {
                                Payline7(winMatrix[0][1], 6);

                                if(PaylinesAmount > 7)
                                {
                                    Payline8(winMatrix[0][0], 7);

                                    if(PaylinesAmount > 8)
                                    {
                                        Payline9(winMatrix[0][2], 8);

                                        if(PaylinesAmount > 9)
                                        {
                                            Payline10(winMatrix[0][0], 9);
                                        }
                                    }
                                }
                            }
                        }


                    }
                }

                
            }
        }

        CheckLists();
    }

    private void Payline1(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][1] == startPoint)
        {
            listName.Add(winMatrix[1][1]);
            if (winMatrix[2][1] == startPoint)
            {
                listName.Add(winMatrix[2][1]);
                if (winMatrix[3][1] == startPoint)
                {
                    listName.Add(winMatrix[3][1]);
                    if (winMatrix[4][1] == startPoint)
                    {
                        listName.Add(winMatrix[4][1]);
                        
                    }
                }
            }   
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline2(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][0] == startPoint)
        {
            listName.Add(winMatrix[1][0]);
            if (winMatrix[2][0] == startPoint)
            {
                listName.Add(winMatrix[2][0]);
                if (winMatrix[3][0] == startPoint)
                {
                    listName.Add(winMatrix[3][0]);
                    if (winMatrix[4][0] == startPoint)
                    {
                        listName.Add(winMatrix[4][0]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline3(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][2] == startPoint)
        {
            listName.Add(winMatrix[1][2]);
            if (winMatrix[2][2] == startPoint)
            {
                listName.Add(winMatrix[2][2]);
                if (winMatrix[3][2] == startPoint)
                {
                    listName.Add(winMatrix[3][2]);
                    if (winMatrix[4][2] == startPoint)
                    {
                        listName.Add(winMatrix[4][2]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline4(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][1] == startPoint)
        {
            listName.Add(winMatrix[1][1]);
            if (winMatrix[2][2] == startPoint)
            {
                listName.Add(winMatrix[2][2]);
                if (winMatrix[3][1] == startPoint)
                {
                    listName.Add(winMatrix[3][1]);
                    if (winMatrix[4][0] == startPoint)
                    {
                        listName.Add(winMatrix[4][0]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;

    }

    private void Payline5(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][1] == startPoint)
        {
            listName.Add(winMatrix[1][1]);
            if (winMatrix[2][0] == startPoint)
            {
                listName.Add(winMatrix[2][0]);
                if (winMatrix[3][1] == startPoint)
                {
                    listName.Add(winMatrix[3][1]);
                    if (winMatrix[4][2] == startPoint)
                    {
                        listName.Add(winMatrix[4][2]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline6(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][0] == startPoint)
        {
            listName.Add(winMatrix[1][0]);
            if (winMatrix[2][0] == startPoint)
            {
                listName.Add(winMatrix[2][0]);
                if (winMatrix[3][0] == startPoint)
                {
                    listName.Add(winMatrix[3][0]);
                    if (winMatrix[4][1] == startPoint)
                    {
                        listName.Add(winMatrix[4][1]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline7(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][2] == startPoint)
        {
            listName.Add(winMatrix[1][2]);
            if (winMatrix[2][2] == startPoint)
            {
                listName.Add(winMatrix[2][2]);
                if (winMatrix[3][2] == startPoint)
                {
                    listName.Add(winMatrix[3][2]);
                    if (winMatrix[4][1] == startPoint)
                    {
                        listName.Add(winMatrix[4][1]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline8(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][0] == startPoint)
        {
            listName.Add(winMatrix[1][0]);
            if (winMatrix[2][1] == startPoint)
            {
                listName.Add(winMatrix[2][1]);
                if (winMatrix[3][2] == startPoint)
                {
                    listName.Add(winMatrix[3][2]);
                    if (winMatrix[4][2] == startPoint)
                    {
                        listName.Add(winMatrix[4][2]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline9(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][2] == startPoint)
        {
            listName.Add(winMatrix[1][2]);
            if (winMatrix[2][1] == startPoint)
            {
                listName.Add(winMatrix[2][1]);
                if (winMatrix[3][0] == startPoint)
                {
                    listName.Add(winMatrix[3][0]);
                    if (winMatrix[4][0] == startPoint)
                    {
                        listName.Add(winMatrix[4][0]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void Payline10(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        if (winMatrix[1][2] == startPoint)
        {
            listName.Add(winMatrix[1][2]);
            if (winMatrix[2][1] == startPoint)
            {
                listName.Add(winMatrix[2][1]);
                if (winMatrix[3][0] == startPoint)
                {
                    listName.Add(winMatrix[3][0]);
                    if (winMatrix[4][1] == startPoint)
                    {
                        listName.Add(winMatrix[4][1]);

                    }
                }
            }
        }
        CurrentWinLists[pos] = listName;
    }

    private void CheckLists()
    {
        var listToCheck1 = CurrentWinLists[0];
        var result1 = CrossCheckList(listToCheck1);
        payMultipliers[0] = result1;
        var listToCheck2 = CurrentWinLists[1];
        var result2 = CrossCheckList(listToCheck2);
        payMultipliers[1] = result2;
        var listToCheck3 = CurrentWinLists[2];
        var result3 = CrossCheckList(listToCheck3);
        payMultipliers[2] = result3;
        var listToCheck4 = CurrentWinLists[3];
        var result4 = CrossCheckList(listToCheck4);
        payMultipliers[3] = result4;
        var listToCheck5 = CurrentWinLists[4];
        var result5 = CrossCheckList(listToCheck5);
        payMultipliers[4] = result5;
        var listToCheck6 = CurrentWinLists[5];
        var result6 = CrossCheckList(listToCheck6);
        payMultipliers[5] = result6;
        var listToCheck7 = CurrentWinLists[6];
        var result7 = CrossCheckList(listToCheck7);
        payMultipliers[6] = result7;
        var listToCheck8 = CurrentWinLists[7];
        var result8 = CrossCheckList(listToCheck8);
        payMultipliers[7] = result8;
        var listToCheck9 = CurrentWinLists[8];
        var result9 = CrossCheckList(listToCheck9);
        payMultipliers[8] = result9;
        var listToCheck10 = CurrentWinLists[9];
        var result10 = CrossCheckList(listToCheck10);
        payMultipliers[9] = result10;

        totalCredits = totalCredits + CalculateTotal(payMultipliers);


        BrokeOrNot(); //check if dude is broke or not

        //ui setup afte game ends
        Credits.text = "Credits: " + totalCredits;
        LastWin.text = "Win Amount: " + CalculateTotal(payMultipliers);
    }


    private void WinListChecker(int startPoint, int pos)
    {
        List<int> listName = new List<int>();
        listName.Add(startPoint);
        Debug.Log(startPoint);
        for (int i = 1; i < 5; i++)//loop through the 4 reamining columns, 
        {
            if (i != listName.Count)// we just set both list name count and i to 1, everytime we find the same symbol in the next column, it gets added to the list and i iterates, so if it does not get added, outside loop breaks.
            {
                break;
            }
            for (int x = 0; x < 3; x++)//should loop through all elements of the column it is in to look for a value similar to the startpoint value, if it finds it then break and go to next column, otherwise the out loop will break on the next iteration
            {
                if (winMatrix[i][x] == startPoint)
                {
                    listName.Add(winMatrix[i][x]);
                    Debug.Log(winMatrix[i][x]);
                    break;

                }

            }

        }
        CurrentWinLists[pos] = listName;
        Debug.Log("WinList Added " + string.Join(" , ", CurrentWinLists[pos]));


    }

    private double CalculateTotal(double[] multipliers)
    {
        double total = 0.0;

        // Iterate through the multipliers array and multiply each element by the betAmount
        for (int i = 0; i < multipliers.Length; i++)
        {
            total += multipliers[i] * betAmount;
        }

        return total;
    }

    
    //checks if there is a payout multiplier for  combination, if there is it returns the multiplier, otherwise 0
    private double CrossCheckList(List<int> list)
    {
        foreach (var entry in WinConPayoutIndex)
        {
            if (ListEquals(entry.Key, list))
            {

                return entry.Value;
            }
        }
        return 0;
    }
    //checks if a list matches another list
    private bool ListEquals(List<int> list1, List<int> list2)
    {
        if (list1.Count != list2.Count)
            return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
                return false;
        }

        return true;
    }

    //when given an array of doubles, it returns the biggest double from it. 
    private double GetMaxFloat(double[] array)
    {
        double max = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
            }
        }
        //play sound based on if win or not
        if (max > 0)
        {
            audioSource1.Play();
        }
        if (max == 0)
        {
            audioSource2.Play();
        }
        return max;



    }

    private void BrokeOrNot() //set spin button interaction 
    {
        Button spinButton = FindObjectOfType<SpinButton>().GetComponent<Button>();
        if (betAmount > totalCredits)
        {

            spinButton.interactable = false;
            
            InsertMoreCoins.enabled = true;
            
            
        }
        else
        {

            spinButton.interactable = true;
            
            InsertMoreCoins.enabled = false;
        }
    }

    public void InfoUI()
    {
        if (buttonClicked)
        {
            buttonClicked = false;

            BetSlider.interactable = true;

            if(betAmount < totalCredits)
            {
                spinButton.interactable = true;
            }
           
        }
        else
        {
            spinButton.interactable = false;
            BetSlider.interactable = false;

            buttonClicked = true;
        }

    }

    private void EndSpin()
    {
        //enable buttons and do cleanup 
        Button spinButton = FindObjectOfType<SpinButton>().GetComponent<Button>();
        spinButton.interactable = true; 
        BetSlider.interactable = true; 
        infoButton.interactable = true;
        spinning = false;
        ImageMask.SetActive(false);
        //call function which creates win matrix
        SetWinMatrix();
    }





}

   