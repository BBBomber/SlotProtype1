using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Globalization;
using TMPro;
//using System.Linq;

public class ImageCycler : MonoBehaviour
{
    public Image[] imageComponents; // Reference to the image components in the scene
    public Sprite[] imageArray; // Array of images/symbols to cycle through
    public float cycleTime = 3.0f; // Total time for the cycling animation
    private bool spinning = false; // Flag to indicate if spinning is in progress 
    private Dictionary<Sprite, int> SpriteIndex = new Dictionary<Sprite, int>(); //dict from sprite to int for checking payouts
    private Dictionary<List<int>, double> WinConPayoutIndex = new Dictionary<List<int>, double>();// dict from a list of win conditions to their respective payout multipliers


    private List<int>[] CurrentWinLists = new List<int>[3]; // array of max 3 lists which might be created after a spin stops to be used as keys in the above dict.

    private int[][] winMatrix = new int[5][];//create an empty win matrix of 5 column arrays populate after spinning has stopped
    public double betAmount = 10;
    public double totalCredits = 100;
    private double[] payMultipliers = new double[3];
    // accessing elements will be done in a transposed manner. matrix[columnIndex][rowIndex] will give the value in the specified column and row, 
    public TMP_Text Credits;
    public TMP_Text LastWin;
    public TMP_Text BetAmount;
    

    //insert coins text animation
    public TMP_Text InsertMoreCoins;
   

    public Slider BetSlider; //slider ref
    public Button spinButton;
    public Button infoButton;
    private bool buttonClicked = false;

    //audio stuff
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    private AudioSource audioSource1;
    private AudioSource audioSource2;

    private void Awake()
    {
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource1.clip = audioClip1;

        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.clip = audioClip2;

        SpriteIndex.Add(imageArray[0], 1);
        SpriteIndex.Add(imageArray[1], 2);
        SpriteIndex.Add(imageArray[2], 3);
        SpriteIndex.Add(imageArray[3], 4);
        SpriteIndex.Add(imageArray[4], 5);
        SpriteIndex.Add(imageArray[5], 6);
        SpriteIndex.Add(imageArray[6], 7);

        //temp
        WinConPayoutIndex.Add(new List<int> { 1, 1, 1 }, 0.35);
        WinConPayoutIndex.Add(new List<int> { 2, 2, 2 }, 0.7);
        WinConPayoutIndex.Add(new List<int> { 3, 3, 3 }, 1.05);
        WinConPayoutIndex.Add(new List<int> { 4, 4, 4 }, 1.4);
        WinConPayoutIndex.Add(new List<int> { 5, 5, 5 }, 1.75);
        WinConPayoutIndex.Add(new List<int> { 6, 6, 6 }, 2.1);
        WinConPayoutIndex.Add(new List<int> { 7, 7, 7 }, 2.45);
        WinConPayoutIndex.Add(new List<int> { 1, 1, 1, 1 }, 1.1);
        WinConPayoutIndex.Add(new List<int> { 2, 2, 2, 2 }, 2.2);
        WinConPayoutIndex.Add(new List<int> { 3, 3, 3, 2 }, 3.3);
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


        Credits.text = "Credits: " + totalCredits;
        LastWin.text = "Win Amount: " + 0;
        BetAmount.text = "Bet Amount: " + betAmount;
       
        InsertMoreCoins.text = "Insert More Coins..";
        
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
        BrokeOrNot();
    }

    public void StoppedSliding(float value)
    {
        
        betAmount = value;
        BetAmount.text = "Bet Amount: " + betAmount;
        BrokeOrNot();
    }

    public void StartSpinning()
    {
        if (!spinning)
        {
            
            audioSource1.Stop();
            audioSource2.Stop();
            totalCredits = totalCredits - betAmount;
            Credits.text = "Credits: " + totalCredits;
            spinning = true;
            StartCoroutine(SpinImages());
        }
    }

    private System.Collections.IEnumerator SpinImages()
    {
        Button spinButton = FindObjectOfType<SpinButton>().GetComponent<Button>();
        spinButton.interactable = false; // Disable the spin button
        BetSlider.interactable = false; //disable slider
        infoButton.interactable = false;
        float startTime = Time.time;


        while (Time.time - startTime < cycleTime)
        {

            // Update the images for each component based on the progress
            for (int i = 0; i < imageComponents.Length; i++)
            {

                imageComponents[i].sprite = GetRandomImage();

            }

            yield return new WaitForSeconds(0.1f);
        }



        spinButton.interactable = true; // Enable the spin button
        BetSlider.interactable = true; // Enable Slider
        infoButton.interactable = true;
        spinning = false;
        SetWinMatrix();




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







        //create win lists
        WinListChecker(winMatrix[0][0], 0);
        WinListChecker(winMatrix[0][1], 1);
        WinListChecker(winMatrix[0][2], 2);


        var listToCheck1 = CurrentWinLists[0];
        var result1 = CrossCheckList(listToCheck1);
        payMultipliers[0] = result1;
        var listToCheck2 = CurrentWinLists[1];
        var result2 = CrossCheckList(listToCheck2);        
        payMultipliers[1] = result2;
        var listToCheck3 = CurrentWinLists[2];
        var result3 = CrossCheckList(listToCheck3);        
        payMultipliers[2] = result3;

        totalCredits =  totalCredits + (betAmount *  GetMaxFloat(payMultipliers));
       

        BrokeOrNot(); //check if dude is broke or not
        
        //ui setup afte game ends
        Credits.text = "Credits: " + totalCredits;
        LastWin.text = "Win Amount: " + betAmount * GetMaxFloat(payMultipliers);

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

    private void DebugWinMatrix()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int x = 0; x < 3; x++)
            {
                Debug.Log(winMatrix[i][x]);
            }
        }
    }

    private double CrossCheckList(List<int> list)
    {
        foreach (var entry in WinConPayoutIndex)
        {
            if (ListEquals(entry.Key, list))
            {

                return entry.Value;
            }
        }
        return -1;
    }

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







}

   