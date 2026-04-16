using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float stomachContents;
    public float intestineContents;
    public float coomContents;
    public float gasContents;
    public float coomStorage;
    public float liquidContents;
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
    public int bankedCalories;
    public float wombContents;
    public int fetusCount;
    public int fertilityBonus;
    public int pregnancyDays;
    public int actualDays;
    public int lastSeenEmptyBelly;
    public int currentTime;
    public bool tookCaffeine;
    public bool isNauseous;
    public bool isStreaming;
    //public bool jiggledDuringStream;
    public bool tookLaxative;
    public bool usedPlug;
    public int daysUntilNextStream;

    public int sleepCountdown;
    public bool isAsleep;
    public bool[] achievements;
    public bool[] toggledStates = new bool[7];
    //0: BGM
    //1: SFX
    //2: voice
    //3: stomach xray
    //4: womb xray
    //5: sideview enabled
    //6: womb tattoo
    public int[] preyHealth;
    public bool alwaysUseEatingAnimation;
    //public bool tattooToggledOn;
    //public bool xRayToggledOn;
    public bool ampmMode;
    public bool nakedMode;
    public bool transparentSideview;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
