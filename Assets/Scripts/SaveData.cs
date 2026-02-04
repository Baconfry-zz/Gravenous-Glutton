using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float stomachContents;
    public float intestineContents;
    public float coomContents;
    public float coomStorage;
    public int digestionTimer;
    public int hungerTimer;
    public float trainingModifier;
    public float intestineMultiplier;
    public int munchiesConsumed;
    public int weedStock;
    public int money;
    public int cumulativeEarnings;
    public float flowRate;
    public int disposalTimer;
    public float dailyCalories;
    public float wombContents;
    public int fetusCount;
    public int pregnancyDays;
    public int actualDays;
    public int currentTime;
    public bool tookCaffeine;
    public bool isStreaming;
    public bool jiggledDuringStream;
    public bool tookLaxative;
    public bool usedPlug;
    public int daysUntilNextStream;

    public int sleepCountdown;
    public bool isAsleep;
    public bool[] achievements;
    public bool alwaysUseEatingAnimation;
    public bool tattooToggledOn;
    public bool xRayToggledOn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
