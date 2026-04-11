using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MainLoop : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Text foodText;
    [SerializeField] private Text bellyText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text caloriesText;
    [SerializeField] private Text hungerText;
    [SerializeField] private Text achievementText;
    [SerializeField] private Text medicineText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Text donationsText;
    [SerializeField] private Text[] contentsText;
    [SerializeField] private Text deleteSaveText;

    [SerializeField] private Text achievementBoard;
    [SerializeField] private Text achievementRewards;

    [SerializeField] private AudioPlayer musicPlayer;
    [SerializeField] private AudioPlayer stuffedMoansPlayer;
    [SerializeField] private AudioPlayer gulpPlayer;
    [SerializeField] private AudioPlayer gurglePlayer;
    [SerializeField] private AudioClip hungrySound;
    [SerializeField] private AudioClip lastBirthSound;
    [SerializeField] private AudioClip hiccupSound;
    [SerializeField] private AudioClip[] burpSounds;
    [SerializeField] private AudioPlayer streamDigestionSounds;
    [SerializeField] private AudioPlayer smokePlayer;
    [SerializeField] private AudioPlayer achievementPlayer;
    [SerializeField] private AudioPlayer gaspPlayer;

    [SerializeField] private SpriteRenderer overrideFace;
    [SerializeField] private SpriteRenderer sexFace;
    [SerializeField] private SpriteRenderer roomBG;
    [SerializeField] private SpriteRenderer streamFoodIcon;
    [SerializeField] private SpriteRenderer mouthSprite;
    [SerializeField] private SpriteRenderer hiccupMouth;

    [SerializeField] private GameObject minigameUI;
    [SerializeField] private GameObject statsView;
    [SerializeField] private TimedSlider sexMinigame;
    [SerializeField] private CommentGenerator commentGenerator;
    [SerializeField] private Cursor cursor;
    [SerializeField] private Sprite[] draggableFood;

    public int[] preyHealth = new int[] {0, 0, 0}; //save
    [SerializeField] private Transform[] preyMaxHealthBars = new Transform[3];
    [SerializeField] private Transform[] preyHealthBars = new Transform[3];
    [SerializeField] private PreySpawner preySpawner;
    private int preyOutside;
    private int preyInside;

    [SerializeField] private Transform stomachCapacityBar;
    [SerializeField] private Transform stomachContentsBar;
    [SerializeField] private Transform gasContentsBar;
    [SerializeField] private Transform intestineCapacityBar;
    [SerializeField] private Transform intestineContentsBar;
    [SerializeField] private Transform wombContentsBar;
    [SerializeField] private Transform coomContentsBar;
    [SerializeField] private Transform wombCapacityBar;
    [SerializeField] private Transform coomStorageBar;
    [SerializeField] private Transform xRayWomb;
    [SerializeField] private Transform coomWomb;
    [SerializeField] private Transform sideview;
    [SerializeField] private DigitCounter wombSprites;
    [SerializeField] private SpriteRenderer coomWombSprite;
    private Vector3 xRayStartPosition;

    [SerializeField] private GameObject weedButton;
    [SerializeField] private GameObject foodButton;
    [SerializeField] private GameObject sexButton;
    [SerializeField] private GameObject chatButton;
    [SerializeField] private GameObject tattooToggle;
    [SerializeField] private GameObject xRayToggle;
    [SerializeField] private GameObject sideviewToggle;
    [SerializeField] private GameObject deleteSaveButton;
    [SerializeField] private GameObject autoSexButton;
    [SerializeField] private ToggleButton alwaysEatButton;
    [SerializeField] private ToggleButton achievementButton;
    [SerializeField] private ToggleButton pillButton;
    [SerializeField] private ToggleButton recordButton;
    [SerializeField] private GameObject[] touchColliders;
    [SerializeField] private DigitCounter faces;
    [SerializeField] private DigitCounter weedStockCounter;
    [SerializeField] private DigitCounter wombTattoo;
    [SerializeField] private DigitCounter nopan;
    [SerializeField] private DigitCounter skipTime;
    [SerializeField] private DigitCounter sideviewTop;
    [SerializeField] private DigitCounter sideviewBottom;
    [SerializeField] SpriteRenderer sideviewBase;

    [SerializeField] private string[] messageList;
    private bool[] eligibleMessages;
    private bool[] sentMessages;
    private bool doingSpecialMessage = false;
    private bool givingBirth = false;
    public bool[] achievements = new bool[12];
    [SerializeField] private GameObject[] medicineButtons = new GameObject[7];
    [SerializeField] private GameObject[] plates = new GameObject[50];
    [SerializeField] private int[] medicinePrices = new int[7];
    [SerializeField] private Text medicinePricesText;

    public string clickedButtonName = "よㅛ油";

    /*
     * 0.59, 1.07 | 23: 0.59, 1.20 | 24: 0.59, 1.38 | 25: 0.62, 1.50 | 26: 0.65, 1.57 | 27: 0.74, 1.64
     * 0.42, 0.70 | 21: 0.42, 0.83 | 22: 0.42, 1.01 | 23: 0.42, 1.17 | 24: 0.42, 1.24 | 25: 0.58, 1.27 | 26: 0.67, 1.32 | 27: 0.73, 1.37
     * -1/0, 0/1 : -2/0, -1/1
     * 
     * for implementation: slightly randomize food intake?
     * 
     * */
    float holdFaceDuration = 0f;
    float barRatio = 0.45f;
    public bool isPlayingJiggleAnim = false;
    public bool largeBreastMode = false;
    bool maintainEmptyBellyMessage = false;

    float stomachCapacity = 1.0f;
    public float stomachContents = 0f; //save
    public float gasContents = 0f; //SAVE
    public float liquidContents = 0f;
    public int digestionTimer = 0; //save
    public int hungerTimer = 0; //save
    public float coomContents = 0f; //save
    public float coomStorage = 2f; //save

    public float trainingModifier = 1.0f; //save
    public float hungerModifier = 1.0f;
    public float intestineMultiplier = 1.5f; //save

    public int munchiesConsumed = 0; //save
    public int weedStock = 0; //save
    int storedSoda = 0;
    int inertSoda = 0; //save
    int sodaInIntestines = 0;

    public int money = 0;
    public int cumulativeEarnings = 0;

    bool flowEnabled = false;
    public float flowRate = 0.3f; //save
    float intestineCapacity = 1.0f;
    public float intestineContents = 0f; //save
    public int disposalTimer = 0; //save
    int disposalReq = 4; //save

    public float dailyCalories = 1000f; //save
    public int bankedCalories = 0;

    public float wombContents = 0f; //save
    public int fetusCount; //save
    public int fertilityBonus; //save
    int maxFertilityBonus = 3;
    public int pregnancyDays = 0; //save
    public int actualDays = 0; //save
    public int lastSeenEmptyBelly = 0; //save
    string foodDescription = "";

    public int currentTime = 8; //save
    public int sleepCountdown = 0; //save
    public bool isAsleep = false; //save
    bool isBouncing = false;
    bool babiesKicking = false;
    bool sawNauseaMessage = false;
    bool didElbows = false;
    public bool tookCaffeine = false; //save
    public bool isNauseous = false; //save
    public bool isStreaming = false; //save
    public bool alwaysUseEatingAnimation = false; //save
    public bool tattooToggledOn = false; //save
    public bool xRayToggledOn = false;
    public bool tookLaxative = false; //save
    public bool usedPlug = false; //save
    public int daysUntilNextStream = 0; //save
    public bool playingDigestionSounds = false;
    public bool ampmMode = false;
    public bool nopanMode = false;
    public bool transparentSideview = true;
    Color oldColor = new Color(1f, 1f, 1f, 0.1019608f);
    bool sodaMode = false;

    float displayedStomachContents;
    float displayedIntestineContents;
    float displayedGasContents;
    float displayedWombContents;
    float displayedCoomContents;
    int displayedCalories;
    int streamEarnings;
    int displayedEarnings;

    int imageIndex = 0;
    int startingSize = 0;
    int lastJiggledSize = 5;
    [SerializeField] Sprite[] characterSpritesBtm = new Sprite[14];
    [SerializeField] Sprite[] characterSpritesTop = new Sprite[14];

    public void SaveGame()
    {
        SaveData saveData = new SaveData();

        saveData.stomachContents = stomachContents;
        saveData.coomContents = coomContents;
        saveData.coomStorage = coomStorage;
        saveData.liquidContents = liquidContents;
        saveData.intestineContents = intestineContents;
        saveData.gasContents = gasContents;
        saveData.digestionTimer = digestionTimer;
        saveData.hungerTimer = hungerTimer;
        saveData.trainingModifier = trainingModifier;
        saveData.intestineMultiplier = intestineMultiplier;
        saveData.munchiesConsumed = munchiesConsumed;
        saveData.weedStock = weedStock;
        saveData.money = money;
        saveData.cumulativeEarnings = cumulativeEarnings;
        saveData.flowRate = flowRate;
        saveData.disposalTimer = disposalTimer;
        saveData.dailyCalories = dailyCalories;
        saveData.bankedCalories = bankedCalories;
        saveData.wombContents = wombContents;
        saveData.fetusCount = fetusCount;
        saveData.fertilityBonus = fertilityBonus;
        saveData.pregnancyDays = pregnancyDays;
        saveData.actualDays = actualDays;
        saveData.lastSeenEmptyBelly = lastSeenEmptyBelly;
        saveData.currentTime = currentTime;
        saveData.tookCaffeine = tookCaffeine;
        saveData.isNauseous = isNauseous;
        saveData.isStreaming = isStreaming;
        saveData.tookLaxative = tookLaxative;
        saveData.usedPlug = usedPlug;
        saveData.daysUntilNextStream = daysUntilNextStream;
        saveData.sleepCountdown = sleepCountdown;
        saveData.isAsleep = isAsleep;
        saveData.achievements = achievements;
        saveData.preyHealth = preyHealth;
        saveData.alwaysUseEatingAnimation = alwaysUseEatingAnimation;
        saveData.tattooToggledOn = tattooToggle.GetComponent<ToggleButton>().isActive;
        saveData.xRayToggledOn = xRayToggle.GetComponent<ToggleButton>().isActive;
        saveData.ampmMode = ampmMode;
        saveData.nopanMode = nopanMode;
        saveData.transparentSideview = transparentSideview;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savedGame.json", json);
        //Debug.Log(Application.persistentDataPath);
        //Debug.Log(json);
    }

    public void SaveOnlySettings()
    {
        SaveData oldSaveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + "/savedGame.json"));
        oldSaveData.alwaysUseEatingAnimation = alwaysUseEatingAnimation;
        oldSaveData.tattooToggledOn = tattooToggle.GetComponent<ToggleButton>().isActive;
        oldSaveData.xRayToggledOn = xRayToggle.GetComponent<ToggleButton>().isActive;
        oldSaveData.ampmMode = ampmMode;
        oldSaveData.nopanMode = nopanMode;
        oldSaveData.transparentSideview = transparentSideview;

        string json = JsonUtility.ToJson(oldSaveData);
        File.WriteAllText(Application.persistentDataPath + "/savedGame.json", json);
    }

    public void LoadBlankSaveData()
    {
        stomachContents = 0f;
        intestineContents = 0f;
        coomContents = 0f;
        gasContents = 0f;
        coomStorage = 2f;
        liquidContents = 0f;
        digestionTimer = 0;
        hungerTimer = 0;
        trainingModifier = 1f;
        intestineMultiplier = 1.5f;
        munchiesConsumed = 0;
        weedStock = 0;
        money = 0;
        cumulativeEarnings = 0;
        flowRate = 0.3f;
        disposalTimer = 0;
        dailyCalories = 1000f;
        bankedCalories = 0;
        wombContents = 0f;
        fetusCount = 0;
        fertilityBonus = 0;
        pregnancyDays = 0;
        actualDays = 0;
        lastSeenEmptyBelly = 0;
        currentTime = 8;
        tookCaffeine = false;
        isNauseous = false;
        isStreaming = false;
        tookLaxative = false;
        usedPlug = false;
        daysUntilNextStream = 0;
        sleepCountdown = 0;
        isAsleep = false;
        achievements = new bool[12];
        preyHealth = new int[] {0, 0, 0};
        //alwaysUseEatingAnimation = false;
        tattooToggledOn = false;
        xRayToggledOn = false;
        //ampmMode = false;
        nopanMode = false;
        transparentSideview = true;
    }

    public void WipeSaveData()
    {
        SaveData saveData = new SaveData();

        saveData.stomachContents = 0f;
        saveData.intestineContents = 0f;
        saveData.coomContents = 0f;
        saveData.coomStorage = 2f;
        saveData.liquidContents = 0f;
        saveData.gasContents = 0f;
        saveData.digestionTimer = 0;
        saveData.hungerTimer = 0;
        saveData.trainingModifier = 1f;
        saveData.intestineMultiplier = 1.5f;
        saveData.munchiesConsumed = 0;
        saveData.weedStock = 0;
        saveData.money = 0;
        saveData.cumulativeEarnings = 0;
        saveData.flowRate = 0.3f;
        saveData.disposalTimer = 0;
        saveData.dailyCalories = 1000f;
        saveData.bankedCalories = 0;
        saveData.wombContents = 0f;
        saveData.fetusCount = 0;
        saveData.fertilityBonus = 0;
        saveData.pregnancyDays = 0;
        saveData.actualDays = 0;
        saveData.lastSeenEmptyBelly = 0;
        saveData.currentTime = 8;
        saveData.tookCaffeine = false;
        saveData.isNauseous = false;
        saveData.isStreaming = false;
        saveData.tookLaxative = false;
        saveData.usedPlug = false;
        saveData.daysUntilNextStream = 0;
        saveData.sleepCountdown = 0;
        saveData.isAsleep = false;
        saveData.achievements = new bool[12];
        saveData.preyHealth = new int[] { 0, 0, 0 };
        saveData.alwaysUseEatingAnimation = false;
        saveData.tattooToggledOn = false;
        saveData.xRayToggledOn = false;
        saveData.ampmMode = false;
        saveData.nopanMode = false;
        saveData.transparentSideview = true;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savedGame.json", json);
    }

    IEnumerator Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //weedStockCounter = weedButton.transform.Find("counter").GetComponent<DigitCounter>();
        //fetusCount = 0;
        //if (Random.Range(0, 10) > 7) fetusCount++;
        
        if (File.Exists(Application.persistentDataPath + "/savedGame.json") && Settings.SaveEnabled)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(Application.persistentDataPath + "/savedGame.json"), this);
            }
            catch { }
        }
        //deleteSaveButton.SetActive(Settings.SaveEnabled);
        if (achievements.Length < 12)
        {
            bool[] updatedAchievements = new bool[12];
            for (int i = 0; i < achievements.Length; i++)
            {
                updatedAchievements[i] = achievements[i];
            }
            achievements = updatedAchievements;
        }
        UpdatePreyHealthbars();
        for (int i = 0; i < preyHealth.Length; i++)
        {
            if (preyHealth[i] > 0) preyInside++;
        }
        preySpawner.UpdateValues(preyOutside + preyInside, achievements[8] ? 3 : 2);
        eligibleMessages = new bool[messageList.Length];
        sentMessages = new bool[messageList.Length];
        xRayStartPosition = xRayWomb.localPosition;
        yield return null;
        alwaysEatButton.ForceState(alwaysUseEatingAnimation);
        autoSexButton.SetActive(achievements[4]);
        xRayToggle.GetComponent<ToggleButton>().ForceState(achievements[5] && xRayToggledOn);
        xRayToggle.SetActive(achievements[5]);
        tattooToggle.GetComponent<ToggleButton>().ForceState(achievements[4] && tattooToggledOn);
        tattooToggle.SetActive(achievements[4]);
        sideviewTop.GetComponent<SpriteRenderer>().color = transparentSideview ? oldColor : Color.white;
        sideviewBottom.GetComponent<SpriteRenderer>().color = transparentSideview ? oldColor : Color.white;
        nopan.GetComponent<SpriteRenderer>().enabled = nopanMode;
        PrintAchievementBoard();
        StartCoroutine(MainRoutine());
    }

    void UpdateEligibleMessages(int foodEaten, bool[] seenInteractions)
    {
        foodEaten += preyInside * 10;
        eligibleMessages[0] = imageIndex < 7;
        eligibleMessages[1] = imageIndex < 7;
        eligibleMessages[2] = true;
        eligibleMessages[3] = imageIndex < 7;
        eligibleMessages[4] = true;
        eligibleMessages[5] = true;
        eligibleMessages[6] = true;
        eligibleMessages[7] = true;
        eligibleMessages[8] = imageIndex > 4 && imageIndex < 10;
        eligibleMessages[9] = imageIndex > 4 && foodEaten > 3;
        eligibleMessages[10] = foodEaten > 10;
        eligibleMessages[11] = imageIndex > 8;
        eligibleMessages[12] = true;
        eligibleMessages[13] = foodEaten > 1 && !sodaMode;
        eligibleMessages[14] = imageIndex > 5 && foodEaten > 3;
        eligibleMessages[15] = true;
        eligibleMessages[16] = imageIndex > 5;
        eligibleMessages[17] = foodEaten > 6;
        eligibleMessages[18] = true;
        eligibleMessages[19] = imageIndex > 5;
        eligibleMessages[20] = foodEaten > 6;
        eligibleMessages[21] = foodEaten > 15;
        eligibleMessages[22] = foodEaten > 3;
        eligibleMessages[23] = imageIndex > 4 && foodEaten > 0;
        eligibleMessages[24] = foodEaten > 3;
        eligibleMessages[25] = foodEaten > 6;
        eligibleMessages[26] = foodEaten > 3;
        eligibleMessages[27] = imageIndex > 8;
        eligibleMessages[28] = foodEaten > 9;
        eligibleMessages[29] = imageIndex > 4 && imageIndex < 13;
        eligibleMessages[30] = foodEaten > 15;
        eligibleMessages[31] = imageIndex > 10;
        eligibleMessages[32] = imageIndex > 4 && foodEaten > 0;
        eligibleMessages[33] = imageIndex > 4 && foodEaten > 0;
        eligibleMessages[34] = imageIndex > 20 && foodEaten > 3;
        eligibleMessages[35] = imageIndex > 18;
        eligibleMessages[36] = true;
        eligibleMessages[37] = foodEaten > 15;
        eligibleMessages[38] = true;
        eligibleMessages[39] = imageIndex > 5 && foodEaten > 3;
        eligibleMessages[40] = foodEaten > 9;
        eligibleMessages[41] = imageIndex > 5;
        eligibleMessages[42] = imageIndex > 5 && foodEaten > 3;
        eligibleMessages[43] = foodEaten > 15;
        eligibleMessages[44] = foodEaten > 9;
        eligibleMessages[45] = imageIndex > 5;
        eligibleMessages[46] = imageIndex > 4 && foodEaten > 3;
        eligibleMessages[47] = fetusCount > 2 && seenInteractions[4];
        eligibleMessages[48] = startingSize > 7 && foodEaten > 9;
        eligibleMessages[49] = seenInteractions[4] && foodEaten >= (fetusCount + 1) * 5 && fetusCount > 0;
        messageList[49] = "Olviden comer para " + (fetusCount + 1) + ", esta comiendo para " + ((fetusCount + 1) * 10);
        eligibleMessages[50] = foodEaten > 6 && foodEaten < 10;
        eligibleMessages[51] = imageIndex > 12;
        eligibleMessages[52] = foodEaten > 15;
        eligibleMessages[53] = fetusCount > 1 && seenInteractions[4] && foodEaten > 15;
        eligibleMessages[54] = wombContents > 4f && cumulativeEarnings > 0;
        eligibleMessages[55] = foodEaten > 9;
        eligibleMessages[56] = foodEaten > 6;
        eligibleMessages[57] = imageIndex > 8 && foodEaten > 3;
        eligibleMessages[58] = true;
        eligibleMessages[59] = true;
        eligibleMessages[60] = foodEaten < 6;
        eligibleMessages[61] = foodEaten > 3 && imageIndex > 11;
        eligibleMessages[62] = foodEaten > 9;
        eligibleMessages[63] = foodEaten > 5;
        eligibleMessages[64] = true;
        eligibleMessages[65] = imageIndex > 20;
        eligibleMessages[66] = true;
        eligibleMessages[67] = fetusCount > 3 && pregnancyDays >= 20 && seenInteractions[4];
        eligibleMessages[68] = foodEaten > 3;
        eligibleMessages[69] = imageIndex > 16 && foodEaten > 9;
        eligibleMessages[70] = foodEaten > 9;
        eligibleMessages[71] = foodEaten < 6;
        eligibleMessages[72] = imageIndex > 10;
        eligibleMessages[73] = currentTime < 4;
        eligibleMessages[74] = currentTime > 7 && currentTime < 12 && foodEaten < 15;
        eligibleMessages[75] = foodEaten > 9;
        eligibleMessages[76] = true;
        eligibleMessages[77] = false;
        eligibleMessages[78] = imageIndex > 4;
        eligibleMessages[79] = foodEaten > 30;
        eligibleMessages[80] = imageIndex > 20;
        eligibleMessages[81] = imageIndex > 16;
        eligibleMessages[82] = imageIndex > 20;
        eligibleMessages[83] = pregnancyDays >= 20;
        eligibleMessages[84] = imageIndex > 10;
        eligibleMessages[85] = imageIndex > 16;
        eligibleMessages[86] = tattooToggle.GetComponent<ToggleButton>().isActive && imageIndex > 20;
        eligibleMessages[87] = foodEaten > 5;
        eligibleMessages[88] = imageIndex > 4;
        eligibleMessages[89] = imageIndex > 4 && imageIndex < 10;
        eligibleMessages[90] = foodEaten > 20;
        eligibleMessages[91] = foodEaten > 9 && imageIndex > 6 && imageIndex < 16;
        eligibleMessages[92] = foodEaten > 5;
        eligibleMessages[93] = pregnancyDays >= 20 && (seenInteractions[4] || seenInteractions[5]);
        eligibleMessages[94] = imageIndex > 12;
        eligibleMessages[95] = foodEaten > 20;
        eligibleMessages[96] = pregnancyDays >= 20;
        eligibleMessages[97] = true;
        eligibleMessages[98] = (seenInteractions[4] || seenInteractions[5]) && fetusCount > 0;
        eligibleMessages[99] = foodEaten > 9;
        eligibleMessages[100] = foodEaten > 20;
        eligibleMessages[101] = foodEaten > 5;
        eligibleMessages[102] = imageIndex > 19;
        eligibleMessages[103] = imageIndex > 7 && pregnancyDays >= 20;
        eligibleMessages[104] = imageIndex > 9 && imageIndex < 15;
        eligibleMessages[105] = imageIndex > 5 && imageIndex < 15;
        eligibleMessages[106] = foodEaten > 20;
        eligibleMessages[107] = foodEaten > 20;
        eligibleMessages[108] = fetusCount > 1 && foodEaten > 15;
        eligibleMessages[109] = fetusCount == 1 && foodEaten > 15;
        eligibleMessages[110] = false;
        eligibleMessages[111] = false;
        eligibleMessages[112] = largeBreastMode;
        eligibleMessages[113] = imageIndex > 11;
        eligibleMessages[114] = false;
        eligibleMessages[115] = doingSpecialMessage && preyInside == 1;
        eligibleMessages[116] = doingSpecialMessage && preyInside == 1;
        eligibleMessages[117] = doingSpecialMessage && preyInside == 1;
        eligibleMessages[118] = doingSpecialMessage && preyInside == 1;
        eligibleMessages[119] = doingSpecialMessage && preyInside > 1;
        eligibleMessages[120] = doingSpecialMessage && preyInside > 1;
        eligibleMessages[121] = doingSpecialMessage && preyInside > 1 && fetusCount > 1 && pregnancyDays >= 20 && seenInteractions[4];
        eligibleMessages[122] = doingSpecialMessage && preyInside > 1 && fetusCount > 1 && pregnancyDays >= 20 && seenInteractions[4];
        eligibleMessages[123] = doingSpecialMessage && preyInside == 1;
        eligibleMessages[124] = doingSpecialMessage && preyInside == 2;
    }

    string GetBellyDescriptor()
    {
        string bellyDescriptor = "round";
        if (imageIndex > 7) bellyDescriptor = "huge";
        if (imageIndex > 13) bellyDescriptor = "enormous";
        if (imageIndex > 18) bellyDescriptor = "gigantic";
        return bellyDescriptor;
    }

    int BellyToFaceIndex(bool isJiggling)
    {
        int reactionFaceIndex;
        float foodContents = Mathf.Round((stomachContents + intestineContents + gasContents) * 1000) / 1000;
        switch (Mathf.Floor(foodContents))
        {
            case 0:
                reactionFaceIndex = isJiggling ? 0 : (imageIndex < 6 ? 7 : 2);
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                reactionFaceIndex = isJiggling ? (imageIndex < 6 ? 2 : 11) : (imageIndex < 6 ? 7 : 2);
                break;
            case 5:
            case 6:
            case 7:
            case 8:
                reactionFaceIndex = isJiggling ? 3 : 8;
                break;
            default:
                reactionFaceIndex = isJiggling ? 9 : 3;
                break;
        }
        if (isAsleep) reactionFaceIndex = 0;
        if (babiesKicking) reactionFaceIndex = 11;
        return reactionFaceIndex;
    }

    public IEnumerator SuckItIn()
    {
        if (imageIndex < 2) yield break;
        float frameDelay = 0.03f;
        bool isTopHeavy = stomachContents + gasContents > intestineContents + wombContents + coomContents;
        int trueImageIndex;
        if (imageIndex <= 20)
        {
            trueImageIndex = imageIndex;
        }
        else
        {
            trueImageIndex = (int)(20 + (stomachContents + intestineContents + gasContents + wombContents + coomContents + 0.001f - 20) / 2);
        }//(int)(stomachContents + gasContents + intestineContents + wombContents + coomContents);
        faces.SetCounterTo(10);
        if (!isTopHeavy)
        {
            SetBellySprites(true, imageIndex);
            UpdateWombTattoo(true);
            yield return new WaitForSeconds(frameDelay);
        }
        Vector3 startPosForSuck = xRayWomb.localPosition;
        SetBellySprites(true, (int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 1));
        wombTattoo.SetCounterTo((int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 1));
        nopan.SetCounterTo((int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 1));
        xRayWomb.localPosition = startPosForSuck + new Vector3(0f, 0.1f * (int)(Mathf.Max(4f, wombContents) + coomContents) / 19, 0f) + new Vector3(0.04f, 0.06f, 0f);
        coomWomb.localPosition = xRayWomb.localPosition;
        yield return new WaitForSeconds(frameDelay);
        SetBellySprites(true, (int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 2));
        wombTattoo.SetCounterTo((int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 2));
        nopan.SetCounterTo((int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 2));
        xRayWomb.localPosition = startPosForSuck + new Vector3(0f, 0.1f * (int)(Mathf.Max(4f, wombContents) + coomContents) / 19, 0f) + new Vector3(0.08f, 0.12f, 0f);
        coomWomb.localPosition = xRayWomb.localPosition;
        yield return new WaitForSeconds(frameDelay);
        while (Input.GetMouseButton(0))
        {
            yield return null;
        }
        faces.SetCounterTo(gasContents > 0f ? 10 : BellyToFaceIndex(true));
        if (stomachContents + intestineContents > 3f) gurglePlayer.PlayRandom();
        if ((int)(stomachContents + intestineContents) >= 5 && gasContents == 0f) stuffedMoansPlayer.PlayRandom();
        SetBellySprites(true, (int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 1));
        wombTattoo.SetCounterTo((int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 1));
        nopan.SetCounterTo((int)Mathf.Min(characterSpritesTop.Length/2 - 1, trueImageIndex - 1));
        xRayWomb.localPosition = startPosForSuck + new Vector3(0f, 0.1f * (int)(Mathf.Max(4f, wombContents) + coomContents) / 19, 0f) + new Vector3(0.04f, 0.06f, 0f);
        coomWomb.localPosition = xRayWomb.localPosition;
        yield return new WaitForSeconds(frameDelay);
        SetBellySprites(true, imageIndex);
        wombTattoo.SetCounterTo(imageIndex);
        nopan.SetCounterTo(imageIndex);
        xRayWomb.localPosition = startPosForSuck + new Vector3(0f, 0.1f * (int)(Mathf.Max(4f, wombContents) + coomContents) / 19, 0f);// + new Vector3(-0.04f * (imageIndex - (int)(Mathf.Max(4f, wombContents) + coomContents)), -0.04f * (imageIndex - (int)(Mathf.Max(4f, wombContents) + coomContents)), 0f);
        coomWomb.localPosition = xRayWomb.localPosition;
        yield return new WaitForSeconds(frameDelay * 3);
        SetBellySprites(false, imageIndex);
        UpdateWombTattoo(false);
        yield return new WaitForSeconds(frameDelay * 8);
        SetBellySprites(true, imageIndex);
        UpdateWombTattoo(true);
        yield return new WaitForSeconds(frameDelay * 8);
        if (!isTopHeavy)
        {
            spriteRenderer.sprite = characterSpritesBtm[imageIndex];
            SetBellySprites(false, imageIndex);
            UpdateWombTattoo(false);           
        }
        yield return new WaitForSeconds(frameDelay * 8);
        if (gasContents > 0f)
        {
            StartCoroutine(gaspPlayer.PlayCustomWaitFor(burpSounds[gasContents > 0.5f ? 1 : 0], stuffedMoansPlayer.GetComponent<AudioSource>()));
            StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, burpSounds[gasContents > 0.5f ? 1 : 0].length, 0.08f));
            gasContents = 0f;
            PrintStats();
        }
        faces.SetCounterTo(BellyToFaceIndex(false));
    }

    public IEnumerator BellyJiggle(bool useDefaultBehavior)
    {
        if (isPlayingJiggleAnim) yield break;
        isPlayingJiggleAnim = true;
        //bool initialState = overrideFace.enabled;
        bool initialButtonState = achievementButton.GetComponent<Collider2D>().enabled;

        achievementButton.GetComponent<Collider2D>().enabled = false;
        recordButton.GetComponent<Collider2D>().enabled = false;

        if (!minigameUI.activeInHierarchy)
        {
            if (useDefaultBehavior)
            {
                //overrideFace.enabled = true;

                if (Mathf.Floor(Mathf.Round((stomachContents + intestineContents + gasContents) * 1000) / 1000) >= 5)
                {
                    if (!isAsleep && gasContents == 0f) stuffedMoansPlayer.PlayRandom();
                    if (stomachContents + intestineContents > 1.5f && Random.Range(0, Mathf.Max(0, 8 - imageIndex)) == 0) gurglePlayer.PlayRandom();
                }


                faces.SetCounterTo(gasContents > 0f ? 10 : BellyToFaceIndex(true));
            }
            else if (gasContents > 0f)
            {
                faces.SetCounterTo(10);
            }
            if (doingSpecialMessage) faces.SetCounterTo(12);
        }


        float baseJiggleRate = 0.1f;
        baseJiggleRate *= 1 + ((float)imageIndex / 20);

        bool topHeavy = stomachContents + gasContents > intestineContents + wombContents + coomContents;
        SetBellySprites(!topHeavy, imageIndex);
        UpdateWombTattoo(!topHeavy);
        yield return new WaitForSeconds(baseJiggleRate * 1);
        SetBellySprites(topHeavy, imageIndex);
        UpdateWombTattoo(topHeavy);
        yield return new WaitForSeconds(baseJiggleRate * 2);
        if (imageIndex >= 6)
        {
            if (isStreaming && lastJiggledSize < imageIndex)
            {
                streamEarnings += 5 * (imageIndex - lastJiggledSize);
                lastJiggledSize = imageIndex;
                foodDescription = "A few extra donations roll in as your " + GetBellyDescriptor() + " belly jiggles in front of the camera.";
                if (!sentMessages[77])
                {
                    commentGenerator.GenerateComment(messageList[77]);
                    sentMessages[77] = true;
                }
            }
            SetBellySprites(!topHeavy, imageIndex);
            UpdateWombTattoo(!topHeavy);
            yield return new WaitForSeconds(baseJiggleRate * 1);
            SetBellySprites(topHeavy, imageIndex);
            UpdateWombTattoo(topHeavy);
        }
        recordButton.GetComponent<Collider2D>().enabled = !isStreaming && !isAsleep && daysUntilNextStream <= 0;

        if (!minigameUI.activeInHierarchy && !isAsleep)
        {
            if (gasContents > 0f)
            {
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(gaspPlayer.PlayCustomWaitFor(burpSounds[gasContents > 0.5f ? 1 : 0], stuffedMoansPlayer.GetComponent<AudioSource>()));
                StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, burpSounds[gasContents > 0.5f ? 1 : 0].length + 0.1f, 0.08f));
                gasContents = 0f;
                PrintStats();
            }
            else if (useDefaultBehavior && stomachContents + intestineContents > 2f && Random.Range(0, Mathf.Max(3, 10 - imageIndex)) == 0)
            {
                StartCoroutine(gaspPlayer.PlayCustomWaitFor(hiccupSound, stuffedMoansPlayer.GetComponent<AudioSource>()));
                StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, 0.2f, 0.08f));
            }
        }
        achievementButton.GetComponent<Collider2D>().enabled = initialButtonState;

        if (useDefaultBehavior) holdFaceDuration = (gasContents > 0f ? 0.9f : 0.6f);
        isPlayingJiggleAnim = false;
        if ((babiesKicking || preyInside > 0) && !givingBirth)
        {
            if (useDefaultBehavior) yield return new WaitForSeconds(holdFaceDuration);
            faces.SetCounterTo(doingSpecialMessage ? 12 : (playingDigestionSounds ? 0 : BellyToFaceIndex(false)));
        }
    }

    public IEnumerator BellyJiggle(bool useDefaultBehavior, float speedMultiplier)
    {
        while (isPlayingJiggleAnim) yield return null;
        isPlayingJiggleAnim = true;
        //bool initialState = overrideFace.enabled;
        bool initialButtonState = achievementButton.GetComponent<Collider2D>().enabled;

        achievementButton.GetComponent<Collider2D>().enabled = false;
        recordButton.GetComponent<Collider2D>().enabled = false;

        if (!minigameUI.activeInHierarchy)
        {
            if (useDefaultBehavior)
            {
                //overrideFace.enabled = true;

                if (Mathf.Floor(Mathf.Round((stomachContents + intestineContents + gasContents) * 1000) / 1000) >= 5)
                {
                    if (!isAsleep && gasContents == 0f) stuffedMoansPlayer.PlayRandom();
                    if (stomachContents + intestineContents > 1.5f && Random.Range(0, Mathf.Max(0, 8 - imageIndex)) == 0) gurglePlayer.PlayRandom();
                }


                faces.SetCounterTo(gasContents > 0f ? 10 : BellyToFaceIndex(true));
            }
            else if (gasContents > 0f)
            {
                faces.SetCounterTo(10);
            }
            if (doingSpecialMessage) faces.SetCounterTo(12);
        }


        float baseJiggleRate = 0.1f;
        baseJiggleRate *= 1 + ((float)imageIndex / 20);

        bool topHeavy = stomachContents + gasContents > intestineContents + wombContents + coomContents;
        SetBellySprites(!topHeavy, imageIndex);
        UpdateWombTattoo(!topHeavy);
        yield return new WaitForSeconds(baseJiggleRate * speedMultiplier);
        SetBellySprites(topHeavy, imageIndex);
        UpdateWombTattoo(topHeavy);
        yield return new WaitForSeconds(baseJiggleRate * 2 * speedMultiplier);
        if (imageIndex >= 6)
        {
            if (isStreaming && lastJiggledSize < imageIndex)
            {
                streamEarnings += 5 * (imageIndex - lastJiggledSize);
                lastJiggledSize = imageIndex;
                foodDescription = "A few extra donations roll in as your " + GetBellyDescriptor() + " belly jiggles in front of the camera.";
                if (!sentMessages[77])
                {
                    commentGenerator.GenerateComment(messageList[77]);
                    sentMessages[77] = true;
                }
            }
            SetBellySprites(!topHeavy, imageIndex);
            UpdateWombTattoo(!topHeavy);
            yield return new WaitForSeconds(baseJiggleRate * speedMultiplier);
            SetBellySprites(topHeavy, imageIndex);
            UpdateWombTattoo(topHeavy);
        }
        recordButton.GetComponent<Collider2D>().enabled = !isStreaming && !isAsleep && daysUntilNextStream <= 0;

        if (!minigameUI.activeInHierarchy && !isAsleep)
        {
            if (gasContents > 0f)
            {
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(gaspPlayer.PlayCustomWaitFor(burpSounds[gasContents > 0.5f ? 1 : 0], stuffedMoansPlayer.GetComponent<AudioSource>()));
                StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, burpSounds[gasContents > 0.5f ? 1 : 0].length + 0.1f, 0.08f));
                gasContents = 0f;
                PrintStats();
            }
            else if (useDefaultBehavior && stomachContents + intestineContents > 2f && Random.Range(0, Mathf.Max(3, 10 - imageIndex)) == 0)
            {
                StartCoroutine(gaspPlayer.PlayCustomWaitFor(hiccupSound, stuffedMoansPlayer.GetComponent<AudioSource>()));
                StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, 0.2f, 0.08f));
            }
        }
        achievementButton.GetComponent<Collider2D>().enabled = initialButtonState;

        if (useDefaultBehavior) holdFaceDuration = (gasContents > 0f ? 0.9f : 0.6f);
        isPlayingJiggleAnim = false;
        if ((babiesKicking || preyInside > 0) && !givingBirth)
        {
            if (useDefaultBehavior) yield return new WaitForSeconds(holdFaceDuration);
            faces.SetCounterTo(doingSpecialMessage ? 12 : (playingDigestionSounds ? 0 : BellyToFaceIndex(false)));
        }
    }

    IEnumerator SodaBloat()
    {
        float nonWombContents = stomachContents + intestineContents + gasContents;
        if (storedSoda > 0 && stomachContents + gasContents < 14f)
        {
            faces.SetCounterTo(10);
            gulpPlayer.PlayRandom();
            yield return new WaitForSeconds(1.2f);
            gurglePlayer.PlayRandom();
            if (nonWombContents < 4f)
            {
                faces.SetCounterTo(11);
            }
            else if (nonWombContents < 8f)
            {
                faces.SetCounterTo(3);
                stuffedMoansPlayer.PlayRandom();
            }
            else
            {
                faces.SetCounterTo(9);
                stuffedMoansPlayer.PlayRandom();
            }
        }
        int inflationRounds = 0;
        while (storedSoda > 0)
        {
            storedSoda--;
            inertSoda++;
            inflationRounds++;
            if (storedSoda < 0) storedSoda = 0;
            //if ((int)Mathf.Min(characterSpritesBtm.Length - 1, Mathf.Floor(stomachContents + intestineContents + gasContents + wombContents + coomContents)) > imageIndex) gurglePlayer.PlayRandom();
            nonWombContents = stomachContents + intestineContents + gasContents;
            if (nonWombContents < 4f && nonWombContents + 0.4f > 4f)
            {
                faces.SetCounterTo(3);
                stuffedMoansPlayer.PlayRandom();
                gurglePlayer.PlayRandom();
            }
            if (nonWombContents < 6f && nonWombContents + 0.4f > 6f)
            {
                gurglePlayer.PlayRandom();
            }
            if (nonWombContents < 8f && nonWombContents + 0.4f > 8f)
            {
                faces.SetCounterTo(9);
                stuffedMoansPlayer.PlayRandom();
                gurglePlayer.PlayRandom();
            }
            if (nonWombContents < 10f && nonWombContents + 0.4f > 10f)
            {
                gurglePlayer.PlayRandom();
            }
            if (nonWombContents < 12f && nonWombContents + 0.4f > 12f)
            {
                stuffedMoansPlayer.PlayRandom();
                gurglePlayer.PlayRandom();
            }
            gasContents += 0.4f;
            PrintStats();
            yield return new WaitForSeconds(0.4f);
            if (stomachContents + gasContents >= 14f)
            {
                inertSoda += storedSoda;
                storedSoda = 0;
                StartCoroutine(gaspPlayer.PlayCustomWaitFor(burpSounds[gasContents > 0.5f ? 1 : 0], stuffedMoansPlayer.GetComponent<AudioSource>()));
                StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, burpSounds[gasContents > 0.5f ? 1 : 0].length + 0.1f, 0.08f));
                PrintStats();
            }
        }
        if (inflationRounds > 5)
        {
            List<int> indexes = new List<int>();
            if (!sentMessages[110]) indexes.Add(110);
            if (!sentMessages[111]) indexes.Add(111);

            if (indexes.Count > 0)
            {
                int selectedIndex = indexes[Random.Range(0, indexes.Count)];
                commentGenerator.GenerateComment(messageList[selectedIndex]);
                sentMessages[selectedIndex] = true;
            }
        }
        yield return null;
    }

    IEnumerator ChangeFaceDuringClip(AudioPlayer player, float duration, float delay)
    {
        while (player.GetComponent<AudioSource>().isPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(delay);
        hiccupMouth.enabled = true;
        //overrideFace.enabled = true;
        //holdFaceDuration += duration;
        yield return new WaitForSeconds(duration);
        hiccupMouth.enabled = false;
    }

    public void UpdateMedicineText()
    {
        int numberOfAchievements = 0;
        for (int i = 0; i < achievements.Length; i++)
        {
            if (achievements[i]) numberOfAchievements++;
        }
        maxFertilityBonus = numberOfAchievements / 3;
        medicineButtons[0].SetActive(intestineMultiplier < 2f);
        medicineButtons[1].SetActive(stomachContents > 0f && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier));
        medicineButtons[2].SetActive(isStreaming ? (storedSoda > 0 && stomachContents + gasContents < 14f) : (intestineContents > 0f && !tookLaxative));
        medicineButtons[3].SetActive(fetusCount > 0 && Mathf.Round(wombContents * 10000) / 10000 < Mathf.Round(pregnancyDays * (0.15f + 0.05f * fetusCount) * 10000) / 10000);
        medicineButtons[4].SetActive(hungerTimer < 10);
        medicineButtons[5].SetActive(!tookCaffeine);
        medicineButtons[6].SetActive(!usedPlug && coomContents > 0f);
        medicineButtons[7].SetActive(fetusCount == 0 && achievements[10] && pregnancyDays == 0 && fertilityBonus < maxFertilityBonus);

        medicinePrices[2] = (isStreaming ? 5 : 50);

        string updatedText = "";
        updatedText += "Relaxant: +0.1 intestine capacity (max 2.0)\n\n";
        updatedText += "Enzyme: digest some food in stomach\n\n";
        updatedText += (isStreaming ? "Mentos: reacts on contact with soda\n\n" : ("Laxative: " + (tookLaxative ? "already taken today\n\n" : "digest all food in intestines \n\n")));
        updatedText += "Folate: restore 1 day of missed growth\n\n";
        updatedText += "Ghrelin: +1 hr of natural hunger\n\n";
        updatedText += "Caffeine: sleep at 2:00\n\n";
        updatedText += "Cervical plug: " + (usedPlug ? "already used today\n\n" : "stop leakage until next 8:00\n\n");
        if (achievements[10]) updatedText += "Fertility drug: " + (fetusCount > 0 ? "use before getting pregnant" : ("+1 baby after sex (" + fertilityBonus + "/" + maxFertilityBonus + ")"));

        medicineText.text = updatedText;
        updatedText = "";

        for (int i = 0; i < medicinePrices.Length; i++)
        {
            if (i != 7 || achievements[10]) updatedText += "$" + medicinePrices[i] + "\n\n";
        }
        medicinePricesText.text = updatedText;

        for (int i = 0; i < medicinePrices.Length; i++)
        {
            medicineButtons[i].GetComponent<Collider2D>().enabled = money >= medicinePrices[i];
            medicineButtons[i].GetComponent<SpriteRenderer>().color = money >= medicinePrices[i] ? new Color(0f, 1f, 0.07058835f) : new Color(0.5f, 0.5f, 0.5f);
        }
        //string newMedicineText = "";
        moneyText.text = "$" + money;
    }

    public void UpdateAchievements(int index)
    {
        achievements[index] = true;
        string achievementMessage = "";
        string rewardMessage = "";
        switch (index)
        {
            case 0:
                achievementMessage = "Eater of Worlds: Reach a hunger multiplier of 4x.";
                rewardMessage = "Reward: receive 2 leaves per day";
                break;
            case 1:
                achievementMessage = "No Lunch Break: Spend every waking hour of the day with an overfilled stomach.";
                rewardMessage = "Reward: when near the limit, eat twice";
                break;
            case 2:
                achievementMessage = "Miss Piggy: Digest 8000 calories in one day.";
                rewardMessage = "Reward: digestion starts 1 hour sooner";
                break;
            case 3:
                achievementMessage = "Mucho Texto: Have a total of 18L or more in your stomach and intestines.";
                rewardMessage = "Reward: intestines fill up 33% quicker";
                flowRate = 0.4f;
                break;
            case 4:
                achievementMessage = "Take That, Triple Finish: Get filled with the maximum amount of baby batter.";
                rewardMessage = "Reward: womb tattoo +\nauto sex";
                wombTattoo.GetComponent<SpriteRenderer>().enabled = true;
                tattooToggle.SetActive(true);
                tattooToggle.GetComponent<ToggleButton>().ForceState(true);
                autoSexButton.SetActive(true);
                break;
            case 5:
                achievementMessage = "Opening Kickoff: Experience your first fetal movement.";
                rewardMessage = "Reward: X-ray button";
                xRayToggle.SetActive(true);
                //xRayToggle.GetComponent<ToggleButton>().ForceState(true);
                break;
            case 6:
                achievementMessage = "The Morning After: Wake up with your stomach still overfilled from last night.";
                rewardMessage = "Reward: internal stat display";
                statsView.SetActive(true);
                break;
            case 7:
                achievementMessage = "Elastigirl: Reach the highest possible stretching multiplier.";
                break;
            case 8:
                achievementMessage = "That's No Moon: Reach the largest possible belly size.";
                rewardMessage = "Reward: +1 max prey count";
                preySpawner.UpdateValues(preyOutside + preyInside, 3);
                break;
            case 9:
                achievementMessage = "Queen of Kebabs: Earn a total of $10000 through streaming.";
                rewardMessage = "Reward: stream every day";
                break;
            case 10:
                achievementMessage = "Seed of Destiny: Deliver a perfectly healthy baby.";
                rewardMessage = "Reward: fertility drug unlocked";
                break;
            case 11:
                achievementMessage = "Full Term: Reach the end of the last day of pregnancy.";
                rewardMessage = "Reward: achievement conditions revealed";
                break;
            default:
                achievementMessage = "Naughty Boy: Cause an array index out of bounds exception.";
                break;
        }

        achievementPlayer.PlayRandom();
        achievementText.text = "ACHIEVEMENT\n" + achievementMessage + "\n\n" + rewardMessage;

        //Debug.Log(achievementMessage);
        //Debug.Log(rewardMessage);
    }

    public void PrintAchievementBoard()
    {
        string achievementName = "";
        string achievementDescription = "";
        string rewardMessage = "";

        string finalString = "";
        string rewardString = "";

        for (int i = 0; i < achievements.Length; i++)
        {
            rewardMessage = "";
            switch (i)
            {
                case 0:
                    achievementName = "Eater of Worlds";
                    achievementDescription = "Reach a hunger multiplier of 4x.";
                    rewardMessage = "2 leaves / day";
                    break;
                case 1:
                    achievementName = "No Lunch Break";
                    achievementDescription = "Spend every waking hour of the day with an overfilled stomach.";
                    rewardMessage = "eat twice when near limit";
                    break;
                case 2:
                    achievementName = "Miss Piggy";
                    achievementDescription = "Digest 8000 calories in one day.";
                    rewardMessage = "-1 digestion delay";
                    break;
                case 3:
                    achievementName = "Mucho Texto";
                    achievementDescription = "Have a total of 18L or more in your stomach and intestines.";
                    rewardMessage = "+33% flow rate";
                    break;
                case 4:
                    achievementName = "Take That, Triple Finish";
                    achievementDescription = "Get filled with the maximum amount of baby batter.";
                    rewardMessage = "auto mode sex +      womb tattoo";
                    break;
                case 5:
                    achievementName = "Opening Kickoff";
                    achievementDescription = "Experience your first fetal movement.";
                    rewardMessage = "x-ray button";
                    break;
                case 6:
                    achievementName = "The Morning After";
                    achievementDescription = "Wake up with your stomach still overfilled from last night.";
                    rewardMessage = "internal stat display";
                    break;
                case 7:
                    achievementName = "Elastigirl";
                    achievementDescription = "Reach the highest possible stretching multiplier.";
                    rewardMessage = "--";
                    break;
                case 8:
                    achievementName = "That's No Moon";
                    achievementDescription = "Reach the largest possible belly size.";
                    rewardMessage = "+1 max prey count";
                    break;
                case 9:
                    achievementName = "Queen of Kebabs";
                    achievementDescription = "Earn a total of $10000 through streaming.";
                    rewardMessage = "stream every day";
                    break;
                case 10:
                    achievementName = "Seed of Destiny";
                    achievementDescription = "Give birth to a perfectly healthy baby.";
                    rewardMessage = "fertility drug unlocked";
                    break;
                case 11:
                    achievementName = "Full Term";
                    achievementDescription = "Reach the end of the last day of pregnancy.";
                    rewardMessage = "achievement conditions revealed";
                    break;
                default:
                    achievementName = "Unhandled Exception Guy";
                    achievementDescription = "Attempt to access an array index that is out of bounds.";
                    rewardMessage = "--";
                    break;
            }
            finalString += achievementName + ": " + ((achievements[i] || achievements[11]) ? achievementDescription : "??????") + "\n\n";
            rewardString += "\n" + (achievements[i] ? rewardMessage : "") + "\n";
        }
        achievementBoard.text = finalString;
        achievementRewards.text = rewardString;  
    }

    /*void UpdateWombTattoo()
    {
        wombTattoo.SetCounterTo(imageIndex + (stomachContents > intestineContents + wombContents + coomContents ? 0 : 27));
        //xRayWomb.
    }*/

    void UpdateWombTattoo(bool isTopHeavy)
    {
        wombTattoo.SetCounterTo(imageIndex + (isTopHeavy ? 0 : 28));
        nopan.SetCounterTo(imageIndex + (isTopHeavy ? 0 : 28));
        xRayWomb.localPosition = xRayStartPosition + (isTopHeavy ? new Vector3(0f, 0.1f * (int)(Mathf.Max(4f, wombContents) + coomContents) / 19, 0f) : Vector3.zero) + new Vector3(-0.02f * (imageIndex - (int)(Mathf.Max(4f, wombContents) + coomContents)), -0.04f * (imageIndex - (int)(Mathf.Max(4f, wombContents) + coomContents)), 0f);
        coomWomb.localPosition = xRayWomb.localPosition;
    }

    public IEnumerator Bounce(float bounceDuration)
    {
        //yield return null;
        if (!isBouncing)
        {
            isBouncing = true;
            float initialYPos = transform.localPosition.y;
            float timer = 0;

            while (timer < bounceDuration)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, initialYPos + 0.1f * Mathf.Sin(timer * Mathf.PI / bounceDuration), transform.localPosition.z);
                timer += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, initialYPos, transform.localPosition.z);
            isBouncing = false;
        }
        else
        {
            yield return null;
        }
    }

    string ConvertToAMPM(int time)
    {
        string convertedTime = "";
        if (time == 0)
        {
            convertedTime = "12:00 AM";
        }
        else if (time < 12)
        {
            convertedTime = time + ":00 AM";
        }
        else if (time == 12)
        {
            convertedTime = "12:00 PM";
        }
        else
        {
            convertedTime = (time - 12) + ":00 PM";
        }
        return convertedTime;
    }

    IEnumerator AnimateBirth()
    {
        if (fetusCount == 0) yield break;
        givingBirth = true;
        foodText.text = "You feel your contractions starting.";
        timeText.text = (ampmMode ? (ConvertToAMPM(currentTime)) : ((currentTime < 10 ? "0" : "") + currentTime + ":00")) + " | Day " + (fetusCount > 0 ? actualDays : "--");
        faces.SetCounterTo(5);
        float babyVolume = (wombContents - 6) / fetusCount;

        string babyDescriptor = "";

        if (babyVolume >= 1.92f) babyDescriptor = "healthy ";
        if (babyVolume >= 2.0f) babyDescriptor = "perfectly healthy ";

        gaspPlayer.PlayRandom();
        StartCoroutine(Bounce(0.1f));
        StartCoroutine(BellyJiggle(false));
        faces.SetCounterTo(4);
        yield return new WaitForSeconds(1f);
        faces.SetCounterTo(5);
        yield return new WaitForSeconds(1f);

        float amountToDecrement = 2 * wombContents / (6f + 2 * fetusCount);

        while (fetusCount > 0)
        {
            while (!Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.Space)) yield return null;
            if (!achievements[10] && babyVolume >= 2.0f) UpdateAchievements(10);
            if (fetusCount > 1)
            {
                gaspPlayer.PlayRandom();
            }
            else
            {
                gaspPlayer.PlayCustom(lastBirthSound);
            }
            StartCoroutine(Bounce(0.1f));
            StartCoroutine(BellyJiggle(false));
            faces.SetCounterTo(4);
            yield return new WaitForSeconds(1f);
            faces.SetCounterTo(5);

            fetusCount--;
            wombContents -= amountToDecrement;
            if (wombContents < 0) wombContents = 0;
            foodText.text = "You give birth to a " + babyDescriptor + "baby.";
            PrintStats();
            wombSprites.SetCounterTo(fetusCount);
        }
        wombContents = 0f;
        coomContents = 0f;
        PrintStats();
        //wombContentsBar.localScale = new Vector3(wombContents / 2, wombContentsBar.localScale.y, 1);
        yield return new WaitForSeconds(1f);
        faces.SetCounterTo(0);
        givingBirth = false;
    }

    string IntToWord(int number)
    {
        string word = "";
        switch (number)
        {
            case 0:
                word = "zero";
                break;
            case 1:
                word = "one";
                break;
            case 2:
                word = "two";
                break;
            case 3:
                word = "three";
                break;
            case 4:
                word = "four";
                break;
            case 5:
                word = "five";
                break;
            case 6:
                word = "six";
                break;
            case 7:
                word = "seven";
                break;
            default:
                word = number.ToString();
                break;
        }
        return word;
    }

    string IntToNumberofBabies(int babyCount)
    {
        string uplets = "";
        switch (babyCount)
        {
            case 0:
                uplets = "nothing";
                break;
            case 1:
                uplets = "baby";
                break;
            case 2:
                uplets = "twins";
                break;
            case 3:
                uplets = "triplets";
                break;
            case 4:
                uplets = "quadruplets";
                break;
            default:
                uplets = babyCount + " babies";
                break;
        }
        return uplets;
    }

    IEnumerator SexMinigame(bool startPregnancy)
    {
        minigameUI.SetActive(true);
        StartCoroutine(musicPlayer.ChangeTrackTo(0, 3f));
        achievementButton.GetComponent<Collider2D>().enabled = false;
        bool initialState = nopan.GetComponent<SpriteRenderer>().enabled;
        nopan.GetComponent<SpriteRenderer>().enabled = true;
        achievementButton.Brighten(false);
        yield return StartCoroutine(sexMinigame.Oscillate(startPregnancy, coomStorage));
        nopan.GetComponent<SpriteRenderer>().enabled = initialState;
        achievementButton.Brighten(true);
        achievementButton.GetComponent<Collider2D>().enabled = true;
        StartCoroutine(musicPlayer.ChangeTrackTo(1, 3f));
        if (fetusCount == 0 && sexMinigame.amountReleased > 0)
        {
            fetusCount = 1 + fertilityBonus + Mathf.Min((int)sexMinigame.amountReleased, 200) / 100;
            if (currentTime < 8) actualDays = 1;
        }
        coomContents += sexMinigame.amountReleased / 100;
        if (coomContents + wombContents > 21f) coomContents = 21f - wombContents;
        coomContents = Mathf.Round(coomContents * 10000) / 10000;
        //Debug.Log(sexMinigame.amountReleased);
        //coomStorage -= sexMinigame.amountReleased / 100;
        //coomStorage = Mathf.Round(coomStorage * 10000) / 10000;
        if (sexMinigame.amountReleased > 0) coomStorage = 0f;
        sexMinigame.amountReleased = 0;
        wombCapacityBar.localScale = new Vector3((6f + 2 * fetusCount) * barRatio, wombCapacityBar.localScale.y, 1);
        minigameUI.SetActive(false);
        //overrideFace.enabled = false;
        faces.SetCounterTo(BellyToFaceIndex(false));
        //sexButton.GetComponent<Collider2D>().enabled = (coomStorage >= 1f && !isAsleep);
        if (coomStorage >= 2f && !isAsleep && fetusCount == 0)
        {
            sexButton.GetComponent<AnimateSprite>().EnableAnimations(true);
        }
        else
        {
            sexButton.GetComponent<AnimateSprite>().EnableAnimations(false);
            sexButton.GetComponent<AnimateSprite>().SetAllColors((coomStorage >= 1f && !isAsleep) ? Color.white : new Color(1f, 1f, 1f, 0.2f));
        }
        //sexButton.GetComponent<AnimateSprite>().enabled = coomStorage >= 2f && !isAsleep && fetusCount == 0;
        //if (!sexButton.GetComponent<AnimateSprite>().enabled) sexButton.GetComponent<SpriteRenderer>().color = ((coomStorage >= 1f && !isAsleep) ? Color.white : new Color(1f, 1f, 1f, 0.2f));
        timeText.text = (ampmMode ? (ConvertToAMPM(currentTime)) : ((currentTime < 10 ? "0" : "") + currentTime + ":00")) + " | Day " + (fetusCount > 0 ? actualDays : "--");
        PrintStats();
    }

    IEnumerator Swallow(Prey prey)
    {
        recordButton.GetComponent<Collider2D>().enabled = false;
        foodDescription = prey.health > 20 ? "You manage to catch the struggling prey and start swallowing it." : "Your weakened prey is unable to resist as you start swallowing it.";
        foodText.text = foodDescription;
        prey.wasEaten = true;
        //prey.spriteRenderer.transform.localScale = new Vector3(0.7f, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
        prey.spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        prey.transform.parent = transform.Find("mouth");
        prey.transform.localPosition = new Vector3(0f, -0.3f, 0f);
        prey.spriteRenderer.transform.localPosition = new Vector3(0f, 0.4f, 0f);
        //gulpPlayer.PlayRandom();
        faces.SetCounterTo(12);
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1f + 0.04f * stomachContents);
            prey.IncrementSwallowingStage();
            gulpPlayer.PlayRandom();
            StartCoroutine(Bounce(0.3f));
        }
        yield return new WaitForSeconds(0.5f);
        float startingTime = Time.time;
        faces.SetCounterTo(13);
        while (Time.time - startingTime < 0.5f)
        {
            prey.transform.Translate(new Vector3(0f, -2f * Time.deltaTime, 0f));
            yield return null;
        }
        faces.SetCounterTo(14);
        while (Time.time - startingTime < 1f)
        {
            prey.transform.Translate(new Vector3(0f, -1f * Time.deltaTime, 0f));
            yield return null;
        }
        prey.IncrementSwallowingStage();
        stomachContents += 4f;
        faces.SetCounterTo(BellyToFaceIndex(false));

        for (int i = 0; i < preyHealth.Length; i++)
        {
            if (preyHealth[i] == 0)
            {
                preyHealth[i] = prey.health;
                break;
            }
        }
        preyOutside--;
        if (prey.health > 0) preyInside++;
        Destroy(prey.gameObject);
        System.Array.Sort(preyHealth);
        System.Array.Reverse(preyHealth);
        UpdatePreyHealthbars();
        preySpawner.UpdateValues(preyOutside + preyInside, achievements[8] ? 3 : 2);
        PrintStats();
        recordButton.GetComponent<Collider2D>().enabled = !isAsleep && daysUntilNextStream <= 0;

    }

    void DamageAllPrey()
    {
        for (int i = 0; i < preyHealth.Length; i++)
        {
            if (preyHealth[i] > 0)
            {
                preyHealth[i]--;
                dailyCalories += 100;
                if (preyHealth[i] == 0) preyInside--;
            }
        }
        UpdatePreyHealthbars();
        preySpawner.UpdateValues(preyOutside + preyInside, achievements[8] ? 3 : 2);
    }

    void UpdatePreyHealthbars()
    {
        for (int i = 0; i < preyHealth.Length; i++)
        {
            if (preyHealth[i] > 0)
            {
                /*preyMaxHealthBars[i].localPosition = new Vector3(4 * barRatio * i, preyMaxHealthBars[i].localPosition.y, preyMaxHealthBars[i].localPosition.z);
                preyHealthBars[i].localPosition = new Vector3(4 * barRatio * i, preyHealthBars[i].localPosition.y, preyHealthBars[i].localPosition.z);*/
                preyMaxHealthBars[i].localScale = new Vector3(4 * barRatio, preyMaxHealthBars[i].localScale.y, preyMaxHealthBars[i].localScale.z);
                preyHealthBars[i].localScale = new Vector3(barRatio * preyHealth[i] / 10, preyMaxHealthBars[i].localScale.y, preyMaxHealthBars[i].localScale.z);
            }
            else
            {
                preyMaxHealthBars[i].localScale = new Vector3(0f, preyMaxHealthBars[i].localScale.y, preyMaxHealthBars[i].localScale.z);
                preyHealthBars[i].localScale = new Vector3(0f, preyMaxHealthBars[i].localScale.y, preyMaxHealthBars[i].localScale.z);
            }
        }
    }

    float GetPreyVolume()
    {
        float preyVolume = 0f;
        for (int i = 0; i < preyHealth.Length; i++)
        {
            if (preyHealth[i] > 0) preyVolume += 4f;
        }
        preyVolume = Mathf.Round(preyVolume);
        return preyVolume;
    }

    int GetPreyCount()
    {
        int preyCount = 0;
        for (int i = 0; i < preyHealth.Length; i++)
        {
            if (preyHealth[i] > 0) preyCount++;
        }
        return preyCount;
    }

    IEnumerator MainRoutine()
    {
        float adjustedStomachCapacity;
        bool ateThisTurn;
        faces.SetCounterTo(BellyToFaceIndex(false));
        PrintStats();
        timeText.text = (ampmMode ? (ConvertToAMPM(currentTime)) : ((currentTime < 10 ? "0" : "") + currentTime + ":00")) + " | Day " + (fetusCount > 0 ? actualDays : "--");
        hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
        hungerText.text = "Hunger multiplier: " + hungerModifier + "x";
        moneyText.text = "$" + money;
        wombTattoo.GetComponent<SpriteRenderer>().enabled = achievements[4];
        statsView.SetActive(achievements[6]);

        RefreshTouchColliders();
        //sexFace.enabled = fetusCount == 0;
        //musicPlayer.PlayAtIndex(fetusCount == 0 ? 0 : 1);
        musicPlayer.PlayAtIndex(1);
        StartCoroutine(musicPlayer.GraduallyUnmute(2f));
        bool eligibleForAchievement = true;
        bool topHeavyAtStart = false;
        float kickTimer = Random.Range(2.5f, 5f);

        displayedStomachContents = stomachContents;
        displayedIntestineContents = intestineContents;
        displayedGasContents = gasContents;
        displayedWombContents = wombContents;
        displayedCoomContents = coomContents;
        displayedEarnings = 0;
        displayedCalories = (int) dailyCalories;
        StartCoroutine(IncrementDisplayedValues());
        Vector3 mouthSpriteStartPos = mouthSprite.transform.localPosition;

        while (true)
        {
            if (!isAsleep && Settings.SaveEnabled) SaveGame();
            largeBreastMode = bankedCalories >= 50000;
            SetBellySprites(stomachContents + gasContents > intestineContents + wombContents + coomContents, imageIndex);
            adjustedStomachCapacity = stomachCapacity * hungerModifier * trainingModifier;
            ateThisTurn = false;
            if (stomachContents + gasContents > stomachCapacity * trainingModifier)
            {
                trainingModifier += Mathf.Round((stomachContents + gasContents - (stomachCapacity * trainingModifier)) * (isAsleep ? 20 : 10)) / 1000;
                //if (imageIndex > 3) gurglePlayer.PlayRandom();
                if (trainingModifier >= 3f)
                {
                    trainingModifier = 3f;
                    if (!achievements[7]) UpdateAchievements(7);
                }
            }
            if (hungerTimer > 5 && trainingModifier > 1f)
            {
                trainingModifier -= 0.05f;
                if (trainingModifier < 1f) trainingModifier = 1f;
                trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;
            }

            //stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier, stomachCapacityBar.localScale.y, 1);
            //intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier, intestineCapacityBar.localScale.y, 1);

            if (!achievements[1] && currentTime == 8) eligibleForAchievement = true;//no lunch break

            if (!isAsleep && currentTime > 8 && stomachContents <= stomachCapacity * trainingModifier) eligibleForAchievement = false;

            if (currentTime == 4)
            {
                munchiesConsumed = 0;
                hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
            }
            if (currentTime == 8) usedPlug = false;

            Color skyColor = new Color(1f, 1f, 1f);
            switch (currentTime)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 23:
                    skyColor = new Color(0.0754717f, 0.0754717f, 0.0754717f);
                    break;
                case 22:
                case 5:
                    skyColor = new Color(0.1132075f, 0.1132075f, 0.1132075f);
                    //dark
                    break;
                case 20:
                case 6:
                    skyColor = new Color(0.3113208f, 0.3113208f, 0.3113208f);
                    break;
                case 21:
                    skyColor = new Color(0.2113208f, 0.2113208f, 0.2113208f);
                    break;
                case 7:
                    skyColor = new Color(0.4150943f, 0.4150943f, 0.4150943f);
                    break;
                case 8:
                    skyColor = new Color(0.6509434f, 0.6509434f, 0.6509434f);
                    break;
                case 9:
                    skyColor = new Color(0.8396226f, 0.8396226f, 0.8396226f);
                    break;
                case 19:
                    skyColor = new Color(0.5f, 0.3848666f, 0.3608491f);
                    break;
                case 18:
                    skyColor = new Color(0.7907547f, 0.6703414f, 0.638795f);
                    break;
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    skyColor = Color.white;
                    break;
                case 17:
                    skyColor = new Color(0.8862745f, 0.8070158f, 0.7654859f);
                    break;
                default:
                    break;
            }
            roomBG.color = skyColor;

            if (fetusCount == 0 && !isAsleep)
            {
                //yield return StartCoroutine(SexMinigame(true));
            }
            if (holdFaceDuration <= 0f) faces.SetCounterTo(BellyToFaceIndex(false));
            PrintStats();
            UpdateMedicineText();

            skipTime.SetCounterTo(currentTime == (tookCaffeine ? 1 : 23) ? 2 : 0);

            achievementButton.GetComponent<Collider2D>().enabled = !isAsleep;
            pillButton.GetComponent<Collider2D>().enabled = !isAsleep;
            //sexButton.GetComponent<Collider2D>().enabled = (coomStorage >= 1f && !isAsleep && !isStreaming);     
            if (coomStorage >= 2f && !isAsleep && fetusCount == 0)
            {
                sexButton.GetComponent<AnimateSprite>().EnableAnimations(true);
            }
            else
            {
                sexButton.GetComponent<AnimateSprite>().EnableAnimations(false);
                sexButton.GetComponent<AnimateSprite>().SetAllColors((coomStorage >= 1f && !isAsleep) ? Color.white : new Color(1f, 1f, 1f, 0.2f));
            }
            recordButton.GetComponent<Collider2D>().enabled = !isAsleep && daysUntilNextStream <= 0;
            recordButton.Brighten(daysUntilNextStream <= 0);
            streamEarnings = 0;
            displayedEarnings = 0;
            int foodEaten = 0;
            int plateIndex = 0;
            int maxSize = imageIndex;
            //lastJiggledSize = (imageIndex >= 6 ? (imageIndex - 1) : 5);
            bool[] alreadySeenInteractions = new bool[9] {false, false, false, false, false, false, false, false, false};
            bool[] eligibleInteractions = new bool[9] {true, true, true, true, true, true, true, true, true};
            bool[] alreadyMentionedStreamers = new bool[8] { false, false, false, false, false, false, false, false};
            babiesKicking = false;
            bool interactedWithChat = false;
            bool suckingItIn = false;

            //storedSoda = 0;
            //inertSoda = 0;
            int lastSeenInteraction = -1;
            sodaMode = false;

            while (!Input.GetKeyDown(KeyCode.Return) && clickedButtonName != "skip_time_button" && !isAsleep)
            {
                maxSize = Mathf.Max(imageIndex, maxSize);
                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.Backspace))
                {
                    /*intestineMultiplier = 2f;
                    trainingModifier = 3f;
                    money = 99999;
                    dailyCalories = Mathf.Max(dailyCalories, 2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0));
                    for (int i = 0; i < achievements.Length; i++)
                    {
                        if (!achievements[i]) UpdateAchievements(i);
                    }
                    achievementText.text = "";
                    PrintStats();*/
                    //Debug.Log(Time.deltaTime);
                    //yield return StartCoroutine(SuckItIn());
                    faces.SetCounterTo(4);
                    stuffedMoansPlayer.PlayRandom();
                    gurglePlayer.PlayRandom();
                    yield return StartCoroutine(BellyJiggle(false, 0.3f));
                    yield return StartCoroutine(BellyJiggle(false, 0.2f));
                    faces.SetCounterTo(3);
                    yield return StartCoroutine(BellyJiggle(false, 0.8f));
                    yield return new WaitForSeconds(0.4f);
                    faces.SetCounterTo(BellyToFaceIndex(false));
                }
                adjustedStomachCapacity = stomachCapacity * hungerModifier * trainingModifier;

                bool fedDuringStream = false;

                if (clickedButtonName == "food_button" && isStreaming)
                {                  
                    sodaMode = !sodaMode;
                    foodButton.GetComponent<DigitCounter>().SetCounterTo(sodaMode ? 0 : 1);
                    streamFoodIcon.GetComponent<DigitCounter>().SetCounterTo(sodaMode ? 1 : 0);
                }

                if (clickedButtonName == "mouth") yield return StartCoroutine(SuckItIn());

                if (isStreaming && clickedButtonName == "streaming_food")
                {
                    cursor.GetComponent<SpriteRenderer>().sprite = draggableFood[sodaMode ? 1 : 0];
                    streamFoodIcon.enabled = false;
                    while (!Input.GetMouseButtonUp(0))
                    {
                        yield return null;
                    }
                    fedDuringStream = cursor.GetColliderName(5) == "mouth";
                    if (fedDuringStream && stomachContents + gasContents >= adjustedStomachCapacity) cursor.DropFood();
                    streamFoodIcon.enabled = true;
                    cursor.GetComponent<SpriteRenderer>().sprite = null;
                }

                if (fedDuringStream || (!isStreaming && (Input.GetKeyDown(KeyCode.Space) || (clickedButtonName == "food_button" && !isStreaming))))
                {
                    achievementText.text = "";

                    if (stomachContents + gasContents < adjustedStomachCapacity - 0.0001f && !isNauseous)
                    {
                        babiesKicking = false;
                        suckingItIn = false;
                        //kickTimer = 0f;
                        playingDigestionSounds = false;
                        streamDigestionSounds.Mute(true);
                        float slowdownMultiplier = 1f;
                        if (stomachContents + gasContents > stomachCapacity * trainingModifier) slowdownMultiplier += (stomachContents + gasContents - stomachCapacity * trainingModifier) / (stomachCapacity * trainingModifier * 3);
                        if (fedDuringStream || alwaysUseEatingAnimation)
                        {
                            achievementButton.GetComponent<Collider2D>().enabled = false;
                            recordButton.GetComponent<Collider2D>().enabled = false;
                            StartCoroutine(Bounce(0.2f));
                            mouthSpriteStartPos = mouthSprite.transform.localPosition;
                            mouthSprite.GetComponent<DigitCounter>().SetCounterTo(sodaMode ? 1 : 0);
                            mouthSprite.transform.localPosition = mouthSpriteStartPos + new Vector3(0f, (sodaMode ? -0.1f : 0f), 0f);
                            mouthSprite.enabled = true;
                            //overrideFace.enabled = true;
                            faces.SetCounterTo(0);//imageIndex > 3 ? 3 : 0);
                            gulpPlayer.PlayRandom();
                            yield return new WaitForSeconds(0.6f * slowdownMultiplier);
                            StartCoroutine(Bounce(0.2f));
                            yield return new WaitForSeconds(0.2f);
                            mouthSprite.enabled = false;
                            faces.SetCounterTo(BellyToFaceIndex(false));
                            //overrideFace.enabled = false;
                            if (!(achievements[1] && stomachContents + gasContents + 0.4f >= adjustedStomachCapacity)) yield return new WaitForSeconds(0.2f);
                            achievementButton.GetComponent<Collider2D>().enabled = true;
                        }
                        else
                        {
                            mouthSprite.enabled = true;
                            mouthSprite.GetComponent<DigitCounter>().SetCounterTo(0);
                            faces.SetCounterTo(0);
                            StartCoroutine(Bounce(0.3f));
                            holdFaceDuration = 0.3f;
                        }
                        //faces.SetCounterTo(BellyToFaceIndex(false));
                        if ((displayedStomachContents < stomachContents || sodaMode) && stomachContents + gasContents < 11.4f) gasContents += 0.2f;
                        if (sodaMode)
                        {
                            storedSoda++;
                            liquidContents += 0.4f;
                        }
                        stomachContents += 0.4f;             
                        if (isStreaming)
                        {
                            if (plateIndex <= plates.Length && !sodaMode) plates[plateIndex].SetActive(true);
                            foodEaten++;
                            if (!sodaMode) plateIndex++;
                            streamEarnings += (int)((1 + 2 * (stomachContents + intestineContents + wombContents + gasContents + coomContents)) * Mathf.Pow(1.013f, (foodEaten + preyInside * 10)));
                            chatButton.GetComponent<Collider2D>().enabled = true;
                            chatButton.GetComponent<SpriteRenderer>().color = Color.white;
                        }
                        if (fedDuringStream || alwaysUseEatingAnimation)
                        {
                            holdFaceDuration = 0f;
                            faces.SetCounterTo(BellyToFaceIndex(false));
                        }
                        gulpPlayer.PlayRandom();
                        if (achievements[1] && stomachContents + gasContents >= adjustedStomachCapacity)
                        {
                            foodDescription = "You're not done yet...";
                            foodText.text = foodDescription;
                            PrintStats();
                            if (fedDuringStream || alwaysUseEatingAnimation)
                            {
                                achievementButton.GetComponent<Collider2D>().enabled = false;
                                recordButton.GetComponent<Collider2D>().enabled = false;
                                StartCoroutine(Bounce(0.2f));
                                mouthSprite.enabled = true;
                                //mouthSprite.transform.position = mouthSpriteStartPos + new Vector3(0f, (sodaMode ? -0.1f : 0f), 0f);
                                //overrideFace.enabled = true;
                                faces.SetCounterTo(0);//imageIndex > 4 ? 3 : 0);
                                gulpPlayer.PlayRandom();
                                yield return new WaitForSeconds(0.6f * slowdownMultiplier);
                                StartCoroutine(Bounce(0.2f));
                                yield return new WaitForSeconds(0.2f);
                                mouthSprite.enabled = false;
                                //overrideFace.enabled = false;
                                faces.SetCounterTo(BellyToFaceIndex(false));
                                //yield return new WaitForSeconds(0.2f);
                                achievementButton.GetComponent<Collider2D>().enabled = true;
                            }
                            else
                            {
                                StartCoroutine(Bounce(0.3f));
                                yield return new WaitForSeconds(0.3f);
                            }
                            if ((displayedStomachContents < stomachContents || sodaMode) && stomachContents + gasContents < 11.4f) gasContents += 0.2f;
                            if (sodaMode)
                            {
                                storedSoda++;
                                liquidContents += 0.4f;
                            }
                            stomachContents += 0.4f;
                            if (isStreaming)
                            {
                                if (plateIndex <= plates.Length) plates[plateIndex].SetActive(true);
                                foodEaten++;
                                if (!sodaMode) plateIndex++;
                                streamEarnings += (int)((3 + 2 * (stomachContents + intestineContents + wombContents + gasContents + coomContents)) * Mathf.Pow(1.013f, foodEaten));
                            }
                            gulpPlayer.PlayRandom();
                        }
                        mouthSprite.transform.localPosition = mouthSpriteStartPos;
                        recordButton.GetComponent<Collider2D>().enabled = !isStreaming && !isAsleep && daysUntilNextStream <= 0;
                        ateThisTurn = true;
                        if (stomachContents + gasContents >= stomachCapacity * trainingModifier && hungerModifier > 1)
                        {
                            foodDescription = "You ignore the signals of fullness from your overstuffed belly and continue gorging yourself.";
                        }
                        else if (stomachContents + gasContents >= stomachCapacity * trainingModifier)
                        {
                            foodDescription = "You struggle to finish one last plate of food.";
                        }
                        else
                        {
                            foodDescription = "You finish a plate of food.";                         
                        }
                        if (fedDuringStream)
                        {
                            float digestiveContents = stomachContents + intestineContents;
                            if (digestiveContents >= 14f)
                            {
                                foodDescription = "Fueled by your determination to stuff yourself to the absolute limit, you strain your tired " + (sodaMode ? "throat " : "jaw ") + "muscles to gulp down one more " + (sodaMode ? "can of soda" : "plate of food") + ", and your gigantic, painfully stuffed belly expands just a bit more.";
                            }
                            else if (digestiveContents >= 11f)
                            {
                                foodDescription = "You struggle to " + (sodaMode ? "force even more soda " : "cram even more food ") + "into your massive, tightly stretched belly, panting heavily as your overfilled stomach pushes up against your lungs.";

                            }
                            else if (digestiveContents >= 8f)
                            {
                                string descriptor = "mountain of food ";
                                if (liquidContents / stomachContents > 0.2f) descriptor = "mountain of food and liquid ";
                                if (liquidContents / stomachContents > 0.8f) descriptor = "sea of liquid ";
                                foodDescription = "You manage to force down another " + (sodaMode ? "can" : "plate") + ", but you are starting to feel overwhelmed by the " + descriptor + "now sitting in your enormously stuffed belly.";

                            }
                            else if (digestiveContents >= 5f)
                            {
                                foodDescription = "Although you feel unimaginably full, you continue " + (sodaMode ? "drinking" : "eating") + ", motivated purely by the desire to make your belly as huge and round as possible.";

                            }
                            else if (digestiveContents >= 2f)
                            {
                                foodDescription = "Your full belly is starting to round out nicely, but you continue " + (sodaMode ? "drinking " : "eating ") + "at the same pace.";

                            }
                            else
                            {
                                foodDescription = "You greedily " + (sodaMode ? "chug a can of soda " : "devour a plate of food ") + "in mere seconds.";
                            }
                            UpdateEligibleMessages(foodEaten, alreadySeenInteractions);
                            
                            List<int> indexes = new List<int>();

                            for (int i = 0; i < messageList.Length; i++)
                            {
                                if (eligibleMessages[i] && !sentMessages[i]) indexes.Add(i);
                            }

                            int messagesThisCycle = Random.Range(1, 3);
                            while (indexes.Count > messagesThisCycle)
                            {
                                indexes.RemoveAt(Random.Range(0, indexes.Count));
                            }

                            if (indexes.Count > 0)
                            {
                                foreach (int index in indexes)
                                {
                                    //Debug.Log(messageList[index]);
                                    commentGenerator.GenerateComment(messageList[index]);
                                    sentMessages[index] = true;
                                }
                            }                           
                        }
                        if (gasContents > 0f && stomachContents + intestineContents >= 18f)
                        {
                            StartCoroutine(gaspPlayer.PlayCustomWaitFor(burpSounds[gasContents > 0.5f ? 1 : 0], stuffedMoansPlayer.GetComponent<AudioSource>()));
                            StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, burpSounds[gasContents > 0.5f ? 1 : 0].length + 0.1f, 0.08f));
                            gasContents = 0f;
                            //PrintStats();
                        }
                        UpdateMedicineText();
                        PrintStats();

                        //Debug.Log(messageList[Random.Range(0, messageList.Length)]);

                        //if (stomachContents > adjustedStomachCapacity && hungerModifier <= 1) stomachContents = adjustedStomachCapacity //restrict overstuffing if not unlocked
                    }
                    else if (isNauseous)
                    {
                        foodDescription = "You feel too nauseous to eat anything right now.";
                        sawNauseaMessage = true;
                        faces.SetCounterTo(6);
                        holdFaceDuration = 1.5f;
                    }
                    else if (hungerModifier <= 1f)
                    {
                        foodDescription = "You feel too full to " + (sodaMode ? "drink any more." : "eat another bite.");
                    }
                    else if (stomachContents + gasContents < 12f)
                    {
                        foodDescription = "Despite your enhanced appetite, the sensation of fullness in your overstuffed belly is too intense and you are unable to " + (sodaMode ? "drink " : "eat ") + "any more.";
                    }
                    else 
                    {
                        foodDescription = "You try to force down even more " + (sodaMode ? "soda" : "food") + ", but your overfilled stomach is unable to stretch any further, and swallowing has become physically impossible.";
                    }
                    //Debug.Log(foodDescription);
                    
                }

                //vore
                if (Input.GetMouseButtonUp(0) && cursor.GetAllColliderNames(5).Contains("mouth") && cursor.heldPrey != null)
                {
                    if (stomachContents + gasContents + 3f <= adjustedStomachCapacity)
                    {
                        if (Random.Range(0, 100) < 20 + (40 - cursor.heldPrey.health) * 4)
                        {
                            ateThisTurn = true;
                            babiesKicking = false;
                            suckingItIn = false;
                            //kickTimer = 0f;
                            playingDigestionSounds = false;
                            streamDigestionSounds.Mute(true);
                            doingSpecialMessage = true;
                            yield return StartCoroutine(Swallow(cursor.heldPrey));
                            switch (preyInside)
                            {
                                case 0:
                                case 1:
                                    foodDescription = "You swallow your prey whole.";
                                    break;
                                case 2:
                                    foodDescription = "You swallow your second prey.";
                                    break;
                                case 3:
                                    foodDescription = "You manage to swallow your third prey.";
                                    break;
                                default:
                                    foodDescription = "You're not supposed to be able to swallow more than 3. What happened??";
                                    break;
                            }

                            //add more descriptions depending on stomach contents
                            if (isStreaming)
                            {
                                streamEarnings += 300 * GetPreyCount();
                                chatButton.GetComponent<Collider2D>().enabled = true;
                                chatButton.GetComponent<SpriteRenderer>().color = Color.white;
                                UpdateEligibleMessages(foodEaten, alreadySeenInteractions);
                                List<int> indexes = new List<int>();
                                for (int i = 115; i < 125; i++)
                                {
                                    if (!sentMessages[i] && eligibleMessages[i]) indexes.Add(i);
                                }

                                if (indexes.Count > 0)
                                {
                                    int selectedIndex = indexes[Random.Range(0, indexes.Count)];
                                    commentGenerator.GenerateComment(messageList[selectedIndex]);
                                    sentMessages[selectedIndex] = true;
                                }

                                UpdateEligibleMessages(foodEaten, alreadySeenInteractions);
                            }
                            doingSpecialMessage = false;
                        }
                        else
                        {
                            foodDescription = "Your prey manages to escape from you this time.";
                            cursor.heldPrey.TossInRandomDirection(0.1f);
                            faces.SetCounterTo(6);
                            holdFaceDuration = 0.6f;
                        }
                    }
                    else
                    {
                        foodDescription = "You need to make more room in your stomach to fit more prey.";
                        cursor.heldPrey.TossInRandomDirection(0.1f);
                    }

                }

                if (clickedButtonName == "prey_button" && preyOutside + preyInside < (achievements[8] ? 3 : 2))
                {
                    preyOutside++;
                    preySpawner.SpawnPrey();
                    preySpawner.UpdateValues(preyOutside + preyInside, achievements[8] ? 3 : 2);
                }

                foodText.text = foodDescription;

                if (clickedButtonName == "toggle_sideview_fullscreen")
                {
                    pillButton.ForceState(false);
                    sideviewToggle.GetComponent<Collider2D>().enabled = false;
                    pillButton.GetComponent<Collider2D>().enabled = false;
                    recordButton.GetComponent<Collider2D>().enabled = false;
                                      
                    sideviewTop.GetComponent<SpriteRenderer>().color = Color.white;
                    sideviewBottom.GetComponent<SpriteRenderer>().color = Color.white;
                    Vector3 originalPosition = sideview.position;
                    Vector3 originalScale = sideview.localScale;
                    sideview.position = new Vector3(0.2f, 0.25f, 0f);
                    sideview.localScale = originalScale * 1.9f;
                    while (!Input.GetMouseButtonUp(0)) yield return null;
                    while (!Input.GetMouseButtonDown(0) || (Input.GetMouseButtonDown(0) && (clickedButtonName == this.gameObject.name || cursor.GetColliderName(4) == "sideview_base") || clickedButtonName == "mouth"))
                    {
                        if (cursor.GetColliderName(4) == "sideview_base" && Input.GetMouseButton(0))// && !isPlayingJiggleAnim)
                        {
                            sideviewBase.enabled = true;                            
                        }
                        else
                        {
                            sideviewBase.enabled = false;
                        }
                        if (clickedButtonName == "mouth") yield return StartCoroutine(SuckItIn());
                        if (clickedButtonName == this.gameObject.name)
                        {
                            sideviewBase.enabled = false;
                            if (suckingItIn)
                            {
                                yield return StartCoroutine(SuckItIn());
                            }
                            else if (!isPlayingJiggleAnim)
                            {
                                StartCoroutine(Bounce(0.3f));
                                yield return StartCoroutine(BellyJiggle(true));
                            }
                        }
                        //if (Input.GetMouseButtonDown(0) && cursor.GetColliderName(4) == "sideview_base") StartCoroutine(BellyJiggle(true));
                        if ((babiesKicking || preyInside > 0) && kickTimer <= 0f && !isPlayingJiggleAnim)
                        {
                            StartCoroutine(BellyJiggle(false));
                            kickTimer = Random.Range(2.5f, 5f);
                            if (preyInside == 0)
                            {
                                faces.SetCounterTo(gasContents > 0f ? 10 : (playingDigestionSounds ? 0 : 11));
                            }
                            else
                            {
                                kickTimer += Random.Range(5f, 10f);
                            }
                        }
                        if ((babiesKicking || preyInside > 0) && kickTimer > 0f && !doingSpecialMessage) kickTimer -= Time.deltaTime;
                        if (holdFaceDuration > 0f)
                        {
                            holdFaceDuration -= Time.deltaTime;
                            if (holdFaceDuration <= 0f)
                            {
                                holdFaceDuration = 0;
                                mouthSprite.enabled = false;
                                //overrideFace.enabled = isAsleep;
                                //if (isAsleep) faces.SetCounterTo(0);
                                if (babiesKicking)
                                {
                                    faces.SetCounterTo(11);
                                }
                                else if (playingDigestionSounds)
                                {
                                    faces.SetCounterTo(0);
                                }
                                else
                                {
                                    faces.SetCounterTo(BellyToFaceIndex(false));
                                }
                            }
                        }
                        yield return null;
                    }
                    sideviewBase.enabled = false;
                    sideview.position = originalPosition;
                    sideview.localScale = originalScale;
                    sideviewTop.GetComponent<SpriteRenderer>().color = transparentSideview ? oldColor : Color.white;
                    sideviewBottom.GetComponent<SpriteRenderer>().color = transparentSideview ? oldColor : Color.white;
                    pillButton.GetComponent<Collider2D>().enabled = !isAsleep;
                    recordButton.GetComponent<Collider2D>().enabled = !isStreaming && !isAsleep && daysUntilNextStream <= 0;
                    sideviewToggle.GetComponent<Collider2D>().enabled = true;
                }
                if (cursor.GetColliderName(4) == "sideview_base" && Input.GetMouseButtonDown(0))
                {
                    //sideviewBase.enabled = true;
                    transparentSideview = !transparentSideview;
                    sideviewTop.GetComponent<SpriteRenderer>().color = transparentSideview ? oldColor : Color.white;
                    sideviewBottom.GetComponent<SpriteRenderer>().color = transparentSideview ? oldColor : Color.white;
                    SaveOnlySettings();
                }

                if (clickedButtonName == "weed_button")
                {
                    achievementText.text = "";
                    weedStock--;
                    munchiesConsumed++;
                    if (stomachContents + intestineContents == 0f) PrintStats();
                    faces.SetCounterTo(10);
                    smokePlayer.PlayRandom();
                    mouthSprite.GetComponent<DigitCounter>().SetCounterTo(2);
                    mouthSprite.enabled = true;
                    holdFaceDuration = 1f;
                    //if (munchiesConsumed < 5) munchiesConsumed++;   
                    if (hungerModifier < 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed)) hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);

                    if (!achievements[0] && hungerModifier >= 4f) UpdateAchievements(0);
                    //if (hungerModifier < 1f + 0.2f * munchiesConsumed) hungerModifier = 1f + 0.2f * munchiesConsumed;
                    weedStockCounter.SetCounterTo(weedStock);
                    weedStockCounter.SetAltColor(weedStock >= 5);
                }
                weedButton.SetActive(weedStock > 0);
                weedStockCounter.SetCounterTo(weedStock);
                weedStockCounter.SetAltColor(weedStock >= 5);
                hungerText.text = "Hunger multiplier: " + hungerModifier + "x";

                if (clickedButtonName == this.gameObject.name)
                {
                    mouthSprite.enabled = false;
                    if (suckingItIn)
                    {
                        yield return StartCoroutine(SuckItIn());
                    }
                    else if (!isPlayingJiggleAnim)
                    {
                        StartCoroutine(Bounce(0.3f));
                        yield return StartCoroutine(BellyJiggle(true));
                    }
                }
                if (holdFaceDuration > 0f)
                {
                    holdFaceDuration -= Time.deltaTime;
                    if (holdFaceDuration <= 0f)
                    {
                        holdFaceDuration = 0;
                        mouthSprite.enabled = false;
                        //overrideFace.enabled = isAsleep;
                        //if (isAsleep) faces.SetCounterTo(0);
                        if (babiesKicking)
                        {
                            faces.SetCounterTo(11);
                        }
                        else if (playingDigestionSounds)
                        {
                            faces.SetCounterTo(0);
                        }
                        else
                        {
                            faces.SetCounterTo(BellyToFaceIndex(false));
                        }
                    }
                }


                if (clickedButtonName == "sex_button")
                {
                    pillButton.ForceState(false);
                    mouthSprite.enabled = false;
                    //GameObject.Find("toggle_sideview").GetComponent<ToggleButton>().ForceState(false);
                    float mouseHeldDuration = 0f;
                    while (Input.GetMouseButton(0))
                    {
                        if (mouseHeldDuration < 1f && mouseHeldDuration + Time.deltaTime >= 1f)
                        {
                            nopan.GetComponent<SpriteRenderer>().enabled = !nopan.GetComponent<SpriteRenderer>().enabled;
                            nopanMode = nopan.GetComponent<SpriteRenderer>().enabled;
                            SaveOnlySettings();
                        }
                        mouseHeldDuration += Time.deltaTime;
                        yield return null;
                    }
                    if (mouseHeldDuration < 1f && coomStorage >= 1f && !isAsleep && !isStreaming) yield return StartCoroutine(SexMinigame(fetusCount == 0));
                }

                if (clickedButtonName == "chat_button")
                {
                    chatButton.GetComponent<Collider2D>().enabled = false;
                    chatButton.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    interactedWithChat = true;

                    eligibleInteractions[4] = imageIndex > 3;
                    eligibleInteractions[5] = pregnancyDays >= 20 && imageIndex >= 6;
                    eligibleInteractions[6] = stomachContents + gasContents > stomachCapacity * trainingModifier * hungerModifier;
                    eligibleInteractions[7] = imageIndex > 5;                   

                    int interactionIndex = Random.Range(0, eligibleInteractions.Length);
                    while (!eligibleInteractions[interactionIndex]) interactionIndex = Random.Range(0, eligibleInteractions.Length);

                    if ((eligibleInteractions[6] && interactionIndex != 6) || interactionIndex == lastSeenInteraction)
                    {
                        interactionIndex = Random.Range(0, eligibleInteractions.Length);
                        while (!eligibleInteractions[interactionIndex]) interactionIndex = Random.Range(0, eligibleInteractions.Length);
                    }
                    if (eligibleInteractions[4] && !alreadySeenInteractions[4]) interactionIndex = 4;
                    //interactionIndex = 2;

                    string subMessage = "";
                    string subMessage2 = "";

                    switch (interactionIndex)
                    {
                        case 0://elbows
                            subMessage = "You touch your elbows together without much difficulty.";
                            if (imageIndex > 6 ^ largeBreastMode) subMessage = "It's a struggle, but you barely manage to bring your elbows together.";
                            if (imageIndex > 12 || (imageIndex > 6 && largeBreastMode)) subMessage = "You try bringing your elbows together, but it's impossible for obvious reasons.";
                            bellyText.text = "\"can you touch your elbows together?\"\n\n" + (alreadySeenInteractions[0] ? "You're not falling for that again." : (subMessage + " What was that all about?"));
                            if (!alreadySeenInteractions[0])
                            {
                                if (didElbows)
                                {
                                    commentGenerator.GenerateComment(Random.Range(0, 2) == 0 ? "how does she keep falling for this" : "i bet she's doing that on purpose");
                                }
                                else
                                {
                                    commentGenerator.GenerateComment(Random.Range(0, 2) == 0 ? "lmao she fell for it" : "lol, classic elbow trick");
                                }
                            }
                            if (alreadySeenInteractions[0]) eligibleInteractions[0] = false;
                            didElbows = true;
                            faces.SetCounterTo(1);
                            break;
                        case 1://other streamer
                            int streamerIndex = Random.Range(0, 8);
                            bool allMentioned = true;
                            for (int i = 0; i < alreadyMentionedStreamers.Length; i++) { if (!alreadyMentionedStreamers[i]) allMentioned = false; }
                            while (alreadyMentionedStreamers[streamerIndex] && !allMentioned) streamerIndex = Random.Range(0, 8);
                            switch (streamerIndex)
                            {
                                case 0:
                                    subMessage = "Ayume";
                                    subMessage2 = "stuffing";
                                    break;
                                case 1:
                                    subMessage = "Roxanne";
                                    subMessage2 = "stuffing";
                                    break;
                                case 2:
                                    subMessage = "Emmie";
                                    subMessage2 = "stuffing";
                                    break;
                                case 3:
                                    subMessage = "Hazumi";
                                    subMessage2 = "pregnancy";
                                    break;
                                case 4:
                                    subMessage = "Leah";
                                    subMessage2 = "pregnancy";
                                    break;
                                case 5:
                                    subMessage = "Marie";
                                    subMessage2 = "pregnancy";
                                    break;
                                case 6:
                                    subMessage = "Mikami";
                                    subMessage2 = "pregnancy";
                                    break;
                                case 7:
                                    subMessage = "Mina";
                                    subMessage2 = "stuffing";
                                    break;
                            }
                            alreadyMentionedStreamers[streamerIndex] = true;
                            if (allMentioned)
                            {
                                bellyText.text = "\"does anyone know any other streamers who do similar content\"\n\n" + "You tell the chatter that you've already mentioned all the belly content creators that you know of.";
                                eligibleInteractions[1] = false;
                            }
                            else
                            {
                                bellyText.text = "\"does anyone know any other streamers who do similar content\"\n\n" + "You playfully admonish the chatter for thinking about other girls, but mention that you know a girl named " + subMessage + " who does " + subMessage2 + " content.";
                                if (!alreadySeenInteractions[1]) commentGenerator.GenerateComment(subMessage + "? what game is she from");
                            }
                            faces.SetCounterTo(1);
                            break;
                        case 2://digestion sounds
                            bellyText.text = "\"can you do tummy noises pls\"\n\nYou oblige and hold the microphone against your belly.";
                            faces.SetCounterTo(0);
                            playingDigestionSounds = true;
                            streamDigestionSounds.Mute(!GameObject.Find("toggle_SFX").GetComponent<ToggleButton>().isActive);
                            break;
                        case 3://how much more
                            int amount = (int)Mathf.Ceil(((stomachCapacity * trainingModifier * hungerModifier) - stomachContents - gasContents) / 0.4f);
                            if (achievements[1]) amount++;
                            if (stomachContents + gasContents < stomachCapacity * trainingModifier * hungerModifier)
                            {
                                subMessage = "You estimate that your stomach" + (foodEaten == 0 ? " " : " still ") + "has enough room for " + amount + (foodEaten == 0 ? " " : " more ") + (amount == 1 ? "plate" : "plates") + " of food." + ((amount > 5 && amount + foodEaten > 25) ? " Some of your newer viewers don't seem to believe you." : "");
                            }
                            else
                            {
                                subMessage = "You tell them that you are stuffed to the limit, " + ((hungerModifier >= 4f && intestineContents >= intestineCapacity * trainingModifier) ? "and it is physically impossible to stuff your overstretched belly any further." : "but you might be able to force yourself to eat more with some encouragement from chat.");
                                if (gasContents > 0)
                                {
                                    subMessage2 = "\n\nYou can probably free up some space by burping.";
                                }
                                else if (intestineContents < intestineCapacity * trainingModifier * intestineMultiplier)
                                {
                                    subMessage2 = "\n\nMaybe you can free up more space in your stomach if you could get some food to flow into your lower digestive tract.";
                                }
                                else if (hungerTimer < 10)
                                {
                                    subMessage2 = "\n\nMaybe taking some appetite-increasing drugs would also help.";
                                }
                                else if (munchiesConsumed < 5)
                                {
                                    subMessage2 = "\n\nYou might be able to gulp down more food if you had some more weed in your system.";
                                }
                            }
                            bellyText.text = "\"How much more do you think you can eat?\"\n\n" + subMessage + subMessage2;
                            faces.SetCounterTo(1);
                            break;
                        case 4://guess how many babies
                            subMessage = "\"Oh, you're pregnant? Congratulations!!!\"";
                            float totalBellyContents = stomachContents + intestineContents + wombContents + coomContents + gasContents;
                            if (totalBellyContents >= 5f) subMessage = "\"She definitely looks pregnant. Imagine if she was just trolling us with a food baby though\"";
                            if (totalBellyContents >= 7f) subMessage = "\"I thought she was already in her third trimester. is this a trick question?\"";
                            if (totalBellyContents >= 9f) subMessage = "\"She's way too big for a single pregnancy. I think maybe twins?\"";
                            if (totalBellyContents >= 11f) subMessage = "\"she's fucking HUGE!! is she having triplets?\"";
                            if (totalBellyContents >= 13f) subMessage = "\"guys I did the math, based on her size she looks like she's carrying " + (int)Mathf.Ceil((totalBellyContents - 6) / 2) + " babies, full term\"";
                            int foodBabyCount = (int)(Mathf.Max(1, Mathf.Ceil(totalBellyContents - gasContents - 6) / 2)) - (int)Mathf.Max(0, (wombContents - 6) / 2);
                            subMessage2 = "You tell them to look forward to how much bigger you'll be when the stream ends.";
                            if (stomachContents + intestineContents + coomContents > 1f)
                            {
                                string descriptor = "all food.";
                                if (stomachContents == 0f) descriptor = "all \"food\".";
                                if (liquidContents > 0f && liquidContents / stomachContents > 0.5f) descriptor = "all food and soda.";
                                if (liquidContents > 0f && liquidContents == stomachContents) descriptor = "all soda.";
                                subMessage2 = "The rest is " + descriptor + (coomContents >= 0.4f ? " (You decide not to tell them what else is in there.)" : "");
                            }
                            string trimester = "are in roughly your first trimester. ";
                            if (pregnancyDays > 13) trimester = "are in roughly your second trimester. ";
                            if (pregnancyDays > 26) trimester = "are in roughly your third trimester. ";
                            if (pregnancyDays > 39) trimester = "your pregnancy has reached full term. ";
                            if (pregnancyDays > 4)
                            {
                                bellyText.text = "You ask some newly-joined viewers to guess how many babies you are carrying right now.\n\n" + subMessage + (alreadySeenInteractions[4] ? ("\n\nYou let the other chatters tell the newcomers about the " + (fetusCount == 1 ? "single baby" : (fetusCount + " babies")) + (foodBabyCount > 0 ? (" (and " + foodBabyCount + " food bab" + (foodBabyCount == 1 ? "y)" : "ies)")) : "") + " in your belly.") : "\n\nAfter a while, you reveal that you are carrying " + fetusCount + (fetusCount == 1 ? " baby" : " babies") + " and " + trimester + subMessage2);                       
                                faces.SetCounterTo(1);
                                if (!alreadySeenInteractions[4] && fetusCount > 1)
                                {
                                    switch (fetusCount)
                                    {
                                        case 2:
                                            subMessage = "twins? whoa nice";
                                            break;
                                        case 3:
                                            subMessage = "triplets? " + (wombContents > 6 ? "no wonder she's so huge" : "dang i can't wait to see how big she gets");
                                            break;
                                        case 4:
                                            subMessage = "quadruplets? you're not kidding?";
                                            break;
                                        case 5:
                                            subMessage = "five babies? are you sure???";
                                            break;
                                        case 6:
                                            subMessage = "six? six babies?! did I hear that correctly";
                                            break;
                                        case 7:
                                            subMessage = "show us the ultrasound, no way there's actually seven babies in there";
                                            break;
                                    }
                                    commentGenerator.GenerateComment(subMessage);
                                    //if (fetusCount == 3) commentGenerator.GenerateComment("oh baby a triple!!!");
                                }
                            }
                            else if (pregnancyDays > 0)
                            {
                                bellyText.text = "You ask some newly-joined viewers to guess how many babies you are carrying right now.\n\n" + subMessage + (alreadySeenInteractions[4] ? ("\n\nYou let the other chatters tell the newcomers that you are still in the early stages of pregnancy, and your " + GetBellyDescriptor() + " belly is the result of the " + (foodBabyCount > 1 ? (foodBabyCount + " food babies resting inside.") : "food baby resting inside.")) : ("\n\nAfter a while, you reveal that " + (wombContents >= 1f ? "while it's still early, " : "") + "you're pretty sure that you are pregnant, " + (wombContents >= 1f ? "most likely with multiples based on how fast your belly is growing." : "but it's too early to tell how many.") + " The " + GetBellyDescriptor() + " belly that they're seeing is the result of the " + (foodBabyCount > 1 ? (foodBabyCount + " food babies resting inside.") : "food baby resting inside.")));

                            }
                            else
                            {
                                bellyText.text = "You ask some newly-joined viewers to guess how many babies you are carrying right now.\n\n" + subMessage + (alreadySeenInteractions[4] ? ("\n\nYou let the other chatters tell the newcomers that you are not pregnant, and your " + GetBellyDescriptor() + " belly is the result of the " + (foodBabyCount > 1 ? (foodBabyCount + " food babies resting inside.") : "food baby resting inside.")) : ("\n\nAfter a while, you reveal that you are not pregnant (yet), and the " + GetBellyDescriptor() + " belly that they're seeing is the result of the " + (foodBabyCount > 1 ? (foodBabyCount + " food babies resting inside.") : "food baby resting inside.")));
                            }
                            break;
                        case 5://babies kicking
                            if (stomachContents + intestineContents > 2f) subMessage = (stomachContents > stomachCapacity * trainingModifier) ? "overstuffed " : "full ";
                            bellyText.text = "Your " + (fetusCount == 1 ? "baby starts" : "babies start") + " kicking and squirming inside your " + subMessage + "belly. You turn towards the camera to present your viewers with the best possible view.";
                            //faces.SetCounterTo(gasContents > 0f ? 10 : 11);
                            if (!achievements[5]) UpdateAchievements(5);
                            StartCoroutine(Bounce(0.3f));
                            yield return StartCoroutine(BellyJiggle(true));
                            babiesKicking = true;
                            kickTimer = Random.Range(2.5f, 5f);                          
                            break;
                        case 6://dishes
                            subMessage = "Your round tummy presses into the counter, leaving you without much room to work with";
                            if (imageIndex > 7) subMessage = "You have to rest your huge belly on the edge of the sink to make room";
                            if (imageIndex > 13) subMessage = "You have to lean forward with your enormous belly pressed against the front of the counter in order to reach the sink";
                            if (intestineContents < intestineCapacity * intestineMultiplier * trainingModifier)
                            {
                                topHeavyAtStart = stomachContents + gasContents > intestineContents + wombContents + coomContents;
                                if (stomachContents >= flowRate && (intestineContents + flowRate) <= intestineCapacity * intestineMultiplier * trainingModifier)
                                {
                                    stomachContents -= flowRate;
                                    intestineContents += flowRate;
                                }
                                else if (intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier) && (intestineContents + flowRate) > intestineCapacity * intestineMultiplier * trainingModifier)
                                {
                                    stomachContents -= ((intestineCapacity * intestineMultiplier * trainingModifier) - intestineContents);
                                    intestineContents = intestineCapacity * intestineMultiplier * trainingModifier;
                                }
                                else if (stomachContents < flowRate)
                                {
                                    intestineContents += stomachContents;
                                    stomachContents = 0f;
                                }
                                else
                                {
                                    Debug.Log("should not reach this point");
                                }
                                liquidContents -= flowRate;
                                if (inertSoda > 0)
                                {
                                    inertSoda--;
                                    sodaInIntestines++;
                                }
                                else if (storedSoda > 0)
                                {
                                    storedSoda--;
                                    sodaInIntestines++;
                                }
                                if (liquidContents < 0f) liquidContents = 0f;
                                PrintStats();
                                if (stomachContents + gasContents <= (intestineContents + wombContents + coomContents) && topHeavyAtStart && imageIndex > 3)
                                {
                                    gurglePlayer.PlayRandom();
                                    topHeavyAtStart = false;
                                }
                                subMessage2 = "\n\nWhile you wash the dishes, you feel some of your stomach contents flow into the lower parts of your abdomen.";
                            }
                            bellyText.text = "You are too full to eat another bite, so you decide it's a good time to take a break and wash some dishes. " + subMessage + ", but you manage to make some decent progress." + subMessage2;
                            faces.SetCounterTo(0);
                            holdFaceDuration = 0.3f;
                            StartCoroutine(Bounce(0.3f));
                            yield return StartCoroutine(BellyJiggle(false));
                            break;
                        case 7://suck it in
                            bellyText.text = "\"I want to see what it looks like when you try sucking it in\"\n\nSure, why not?\n\n(press and hold on your belly to suck it in)";
                            suckingItIn = true;
                            faces.SetCounterTo(1);
                            break;
                        case 8://measurements
                            float inches = Mathf.Round(10 * (24 + 2 * (stomachContents + intestineContents + wombContents + coomContents + gasContents))) / 10;
                            float centimetres = Mathf.Round(10 * 2.54f * inches) / 10;
                            float kilograms = Mathf.Round(10 * foodEaten * 0.4f) / 10;
                            float pounds = Mathf.Round(10 * kilograms * 2.20462f) / 10;
                            bellyText.text = "\"can you measure your belly?\"\n\nYou get out the tape measure to check. It looks like your belly measures " + inches + " inches (" + centimetres + " cm) at the widest point." + (foodEaten > 0 ? "\n\nYou've " + (foodEaten > storedSoda + inertSoda + sodaInIntestines ? ("eaten " + (foodEaten - inertSoda - storedSoda - sodaInIntestines) + (foodEaten - inertSoda - storedSoda - sodaInIntestines == 1 ? " plate" : " plates") + " of food") : ("drank " + (storedSoda + inertSoda + sodaInIntestines) + (storedSoda + inertSoda + sodaInIntestines > 1 ? " cans" : " can") + " of soda")) + ((foodEaten > storedSoda + inertSoda + sodaInIntestines && storedSoda + inertSoda + sodaInIntestines > 0) ? (" and " + (storedSoda + inertSoda + sodaInIntestines) + (storedSoda + inertSoda + sodaInIntestines > 1 ? " cans" : " can") + " of soda") : "") + " weighing a total of " + pounds + " pounds " + "(" + kilograms + " kg)." : "");

                            faces.SetCounterTo(11);
                            break;
                    }
                    lastSeenInteraction = interactionIndex;
                    alreadySeenInteractions[interactionIndex] = true;
                }

                if (clickedButtonName == "record_button" && !isStreaming)
                {
                    if (isNauseous)
                    {
                        foodDescription = "You feel nauseous. Doing a mukbang stream doesn't seem like a good idea.";
                        sawNauseaMessage = true;
                        faces.SetCounterTo(6);
                        holdFaceDuration = 1.5f;
                    }
                    else
                    {
                        achievementText.text = "";
                        skipTime.SetCounterTo(1);
                        foodButton.GetComponent<DigitCounter>().SetCounterTo(1);
                        foodButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                        isStreaming = true;
                        recordButton.GetComponent<Collider2D>().enabled = false;
                        //sexButton.GetComponent<Collider2D>().enabled = false;                     
                        sexButton.GetComponent<AnimateSprite>().EnableAnimations(false);
                        sexButton.GetComponent<AnimateSprite>().SetAllColors(new Color(1f, 1f, 1f, 0.2f));
                        chatButton.GetComponent<Collider2D>().enabled = true;
                        chatButton.GetComponent<SpriteRenderer>().color = Color.white;
                        startingSize = imageIndex;
                        lastJiggledSize = (imageIndex >= 6 ? (imageIndex - 1) : 5);
                        StartCoroutine(musicPlayer.ChangeTrackTo(2, 1.5f));
                        donationsText.text = "Donations: $" + streamEarnings;
                        UpdateMedicineText();
                    }
                }

                //if (clickedButtonName == "always_eat_button") alwaysUseEatingAnimation = !alwaysUseEatingAnimation;

                if (clickedButtonName == "pill_button")
                {
                    float mouseHeldDuration = 0f;
                    while (Input.GetMouseButton(0))
                    {
                        if (mouseHeldDuration < 3f && mouseHeldDuration + Time.deltaTime >= 3f)
                        {
                            intestineMultiplier = 2f;
                            trainingModifier = 3f;
                            money = (int)Mathf.Max(money, 99999);
                            dailyCalories = Mathf.Max(dailyCalories, 2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0));
                            for (int i = 0; i < achievements.Length; i++)
                            {
                                if (!achievements[i]) UpdateAchievements(i);
                            }
                            achievementText.text = "";
                            PrintStats();
                            UpdateMedicineText();
                        }
                        mouseHeldDuration += Time.deltaTime;
                        yield return null;
                    }
                    achievementText.text = "";
                    UpdateMedicineText();
                }

                if (clickedButtonName == "toggle_achievement")
                {
                    PrintAchievementBoard();
                    recordButton.GetComponent<Collider2D>().enabled = false;
                    pillButton.GetComponent<Collider2D>().enabled = false;
                    sideviewToggle.GetComponent<Collider2D>().enabled = false;
                    skipTime.GetComponent<Collider2D>().enabled = false;                   
                    deleteSaveText.text = (Settings.SaveEnabled ? "Delete save data" : "Resume old save");
                    yield return null;
                    //while (clickedButtonName != "toggle_achievement") yield return null;
                    while (!(Input.GetMouseButtonDown(0) && !cursor.GetAllColliderNames(4).Contains("achievementTextBackdrop")) || clickedButtonName == "always_eat_button" || clickedButtonName == "toggle_tattoo")
                    {
                        yield return null;
                        if (clickedButtonName == "always_eat_button")
                        {
                            alwaysUseEatingAnimation = alwaysEatButton.isActive;
                            SaveOnlySettings();
                        }
                        if (clickedButtonName == "toggle_tattoo") SaveOnlySettings();
                        if (clickedButtonName == "delete_save_button")
                        {
                            if (Settings.SaveEnabled)
                            {
                                deleteSaveText.text = "Press again to confirm";
                                clickedButtonName = "";
                                yield return null;
                                while (!Input.GetMouseButtonDown(0))
                                {
                                    yield return null;
                                }
                                if (clickedButtonName == "delete_save_button")
                                {
                                    WipeSaveData();
                                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                                }
                                else
                                {
                                    deleteSaveText.text = "Delete save data";
                                }
                            }
                            else
                            {
                                Settings.SaveEnabled = true;
                                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                            }
                        }
                        if (clickedButtonName == "restart_button")
                        {
                            //LoadBlankSaveData();
                            Settings.SaveEnabled = false;
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        }
                    }
                    recordButton.GetComponent<Collider2D>().enabled = !isAsleep && daysUntilNextStream <= 0;
                    pillButton.GetComponent<Collider2D>().enabled = !isAsleep;
                    sideviewToggle.GetComponent<Collider2D>().enabled = !isAsleep;
                    skipTime.GetComponent<Collider2D>().enabled = true;
                    clickedButtonName = "toggle_achievement";
                    //achievementButton.GetComponent<ToggleButton>().ForceState(false);
                }

                if (clickedButtonName == "toggle_time")
                {
                    ampmMode = !ampmMode;
                    timeText.text = (ampmMode ? (ConvertToAMPM(currentTime)) : ((currentTime < 10 ? "0" : "") + currentTime + ":00")) + " | Day " + (fetusCount > 0 ? actualDays : "--");
                    SaveOnlySettings();
                }

                if (clickedButtonName == "toggle_xray") SaveOnlySettings();

                if (clickedButtonName == "relaxant")// && intestineMultiplier < 2f)
                {
                    intestineMultiplier += 0.1f;
                    money -= medicinePrices[0];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "cervical_plug")// && !usedPlug)
                {
                    usedPlug = true;
                    money -= medicinePrices[6];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "surrogate_egg")// && fetusCount < maxFetusCount)
                {
                    //fetusCount++;
                    fertilityBonus++;
                    money -= medicinePrices[7];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "enzyme")// && stomachContents > 0f && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier))
                {
                    topHeavyAtStart = stomachContents + gasContents > intestineContents + wombContents + coomContents;
                    /*if (stomachContents >= stomachCapacity * trainingModifier)
                    {
                        hungerTimer = 0;
                        hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
                        hungerText.text = "Hunger multiplier: " + hungerModifier + "x";
                    }*/
                    float preyVolume = GetPreyVolume();
                    if (stomachContents <= preyVolume)
                    {
                        DamageAllPrey();
                        //PrintStats();
                        //yield return new WaitForSeconds(0.2f);
                    }
                    if (stomachContents >= flowRate + preyVolume && (intestineContents + flowRate) <= intestineCapacity * intestineMultiplier * trainingModifier)
                    {
                        stomachContents -= flowRate;
                        intestineContents += flowRate;
                    }
                    else if (intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier) && (intestineContents + flowRate) > intestineCapacity * intestineMultiplier * trainingModifier)
                    {
                        stomachContents -= ((intestineCapacity * intestineMultiplier * trainingModifier) - intestineContents);
                        intestineContents = intestineCapacity * intestineMultiplier * trainingModifier;
                    }
                    else if (stomachContents < flowRate + preyVolume)
                    {
                        intestineContents += stomachContents - preyVolume;
                        stomachContents = preyVolume;
                    }
                    else
                    {
                        Debug.Log("should not reach this point");
                    }
                    DamageAllPrey();
                    liquidContents -= flowRate;
                    if (inertSoda > 0)
                    {
                        inertSoda--;
                        sodaInIntestines++;
                    }
                    else if (storedSoda > 0)
                    {
                        storedSoda--;
                        sodaInIntestines++;
                    }
                    if (liquidContents < 0f) liquidContents = 0f;
                    money -= medicinePrices[1];
                    PrintStats();
                    if (stomachContents + gasContents <= (intestineContents + wombContents + coomContents) && topHeavyAtStart && imageIndex > 4)
                    {
                        gurglePlayer.PlayRandom();
                        topHeavyAtStart = false;
                    }
                    UpdateMedicineText();
                }

                if (clickedButtonName == "laxative" || Input.GetKeyDown(KeyCode.Delete))
                {                
                    if (isStreaming)
                    {
                        pillButton.ForceState(false);
                        money -= medicinePrices[2];
                        PrintStats();
                        UpdateMedicineText();
                        yield return StartCoroutine(SodaBloat());
                    }
                    else
                    {
                        dailyCalories += Mathf.Round(intestineContents * 1000);
                        if (!achievements[2] && (int)dailyCalories >= 8000) UpdateAchievements(2);
                        intestineContents = 0f;
                        sodaInIntestines = 0;
                        disposalTimer = 0;
                        money -= medicinePrices[2];
                        tookLaxative = true;
                        PrintStats();
                        UpdateMedicineText();
                    }
                   
                }

                if (clickedButtonName == "folate" && wombContents < (pregnancyDays * (0.15f + 0.05f * fetusCount))) //(fetusCount + 3f) * 0.05f * pregnancyDays)
                {
                    wombContents += fetusCount * 0.05f;
                    //wombContents += (fetusCount + 3f) * 0.04f;
                    wombContents = Mathf.Round(wombContents * 10000) / 10000;
                    if (coomContents + wombContents > 21f) coomContents = 21f - wombContents;
                    coomContents = Mathf.Round(coomContents * 10000) / 10000;
                    money -= medicinePrices[3];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "ghrelin" && hungerTimer < 10)
                {
                    hungerTimer++;
                    PrintStats();
                    if (hungerTimer > 3 && !gurglePlayer.GetComponent<AudioSource>().isPlaying)
                    {
                        gurglePlayer.PlayCustom(hungrySound, hungerTimer / 20f);
                        holdFaceDuration = hungrySound.length;
                    }
                    if (hungerTimer > 5)
                    {
                        faces.SetCounterTo(5);
                        //overrideFace.enabled = true;
                    }

                    hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
                    if (!achievements[0] && hungerModifier >= 4f) UpdateAchievements(0);
                    hungerText.text = "Hunger multiplier: " + hungerModifier + "x";
                    money -= medicinePrices[4];
                    UpdateMedicineText();
                }

                if (clickedButtonName == "caffeine" && !tookCaffeine)
                {
                    tookCaffeine = true;
                    if (currentTime == 23 && !isStreaming) skipTime.SetCounterTo(0);
                    money -= medicinePrices[5];
                    UpdateMedicineText();
                }

                if ((babiesKicking || preyInside > 0) && kickTimer <= 0f && !isPlayingJiggleAnim)
                {
                    StartCoroutine(BellyJiggle(false));
                    kickTimer = Random.Range(2.5f, 5f);
                    if (preyInside == 0)
                    {
                        faces.SetCounterTo(gasContents > 0f ? 10 : (playingDigestionSounds ? 0 : 11));
                    }
                    else
                    {
                        kickTimer += Random.Range(5f, 10f);
                    }
                }
                if ((babiesKicking || preyInside > 0) && kickTimer > 0f && !doingSpecialMessage) kickTimer -= Time.deltaTime;

                yield return null;
            }
            if (mouthSprite.enabled)
            {
                yield return new WaitForSeconds(holdFaceDuration);
                mouthSprite.enabled = false;
            }
            skipTime.SetCounterTo(currentTime == (tookCaffeine ? 1 : 23) ? 2 : 0);
            if (!isAsleep) achievementText.text = "";
            playingDigestionSounds = false;
            streamDigestionSounds.Mute(true);
            bool canceledStream = false;
            if (isStreaming)
            {
                sodaMode = false;
                foodButton.GetComponent<SpriteRenderer>().color = Color.white;
                foodButton.GetComponent<DigitCounter>().SetCounterTo(0);
                streamFoodIcon.GetComponent<DigitCounter>().SetCounterTo(0);
                faces.SetCounterTo(BellyToFaceIndex(false));
                playingDigestionSounds = false;
                streamDigestionSounds.Mute(true);
                for (int i = 0; i < plates.Length; i++)
                {
                    plates[i].SetActive(false);
                }
                for (int i = 0; i < sentMessages.Length; i++)
                {
                    sentMessages[i] = false;
                }
                money += streamEarnings;
                cumulativeEarnings += streamEarnings;
                if (streamEarnings == 0 && !interactedWithChat) canceledStream = true;
                foodButton.SetActive(true);
                if (!achievements[9] && cumulativeEarnings >= 10000) UpdateAchievements(9);
                moneyText.text = "$" + money;
                StartCoroutine(musicPlayer.ChangeTrackTo(1, 1.5f));
                if (!canceledStream) daysUntilNextStream = achievements[9] ? 1 : 2;
            }
            recordButton.ForceState(false);
            isStreaming = false;            
            recordButton.GetComponent<Collider2D>().enabled = false;
            pillButton.ForceState(false);
            achievementButton.GetComponent<Collider2D>().enabled = false;
            pillButton.GetComponent<Collider2D>().enabled = false;

            foodDescription = "";
            foodText.text = foodDescription;

            //inertSoda += storedSoda;
            inertSoda = 0;
            storedSoda = 0;
            sodaInIntestines = 0;
            //end of hour
            if (gasContents > 0f && currentTime == (tookCaffeine ? 1 : 23))
            {
                faces.SetCounterTo(10);
                holdFaceDuration = 0.9f;
                StartCoroutine(gaspPlayer.PlayCustomWaitFor(burpSounds[gasContents > 0.5f ? 1 : 0], stuffedMoansPlayer.GetComponent<AudioSource>()));
                StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, burpSounds[gasContents > 0.5f ? 1 : 0].length + 0.1f, 0.08f));
                gasContents = 0f;
                PrintStats();
            }

            topHeavyAtStart = stomachContents + gasContents > intestineContents + wombContents + coomContents;

            if (!ateThisTurn && (hungerModifier - 0.2f * munchiesConsumed) <= 1f) foodDescription = "";
            if (stomachContents + gasContents >= adjustedStomachCapacity && !isAsleep && (hungerModifier - 0.2f * munchiesConsumed) > 1.01f)
            {
                foodDescription = "The adrenaline from your feeding frenzy wears off, and the intense sensation of fullness hits you all at once.";
            }
            else if (canceledStream)
            {
                foodDescription = "You quickly shut off your stream before anyone notices that you were live.";
            }
            else if (isNauseous && currentTime == 11)
            {
                if (sawNauseaMessage) foodDescription = "Your nausea wears off and you feel like you can eat again.";
                isNauseous = false;
                sawNauseaMessage = false;
            }
            else 
            {
                foodDescription = "";
            }

            if (foodDescription == "" && fetusCount > 0 && pregnancyDays >= 20 && Random.Range(0, 55 - pregnancyDays) == 1)
            {
                string babyDescriptor = "";
                string preyDescriptor = "";
                switch (fetusCount)
                {
                    case 1:
                        babyDescriptor = "your baby";
                        break;
                    case 2:
                        babyDescriptor = "your twins";
                        break;
                    case 3:
                        babyDescriptor = "your triplets";
                        break;
                    case 4:
                        babyDescriptor = "your quadruplets";
                        break;
                    default:
                        babyDescriptor = "a lot of babies";
                        break;
                }
                switch (preyInside)
                {
                    case 0:
                        preyDescriptor = "";
                        break;
                    case 1:
                        preyDescriptor = " and the prey that you swallowed";
                        break;
                    case 2:
                        preyDescriptor = " and both of the prey that you swallowed";
                        break;
                    case 3:
                        preyDescriptor = " and all of the prey that you swallowed";
                        break;
                    default:
                        preyDescriptor = " and more prey than you're supposed to have";
                        break;
                }
                if (stomachContents + intestineContents > 2.5f)
                {
                    foodDescription = "You feel " + babyDescriptor + preyDescriptor + " squirming in protest within your cramped, overstuffed tummy.";
                }
                else
                {
                    foodDescription = "You feel " + babyDescriptor + preyDescriptor + " shifting slightly inside your tummy.";
                }
                if (!achievements[5]) UpdateAchievements(5);
                if (preyInside > 0)
                {
                    foodDescription += " ";
                }
                foodText.text = foodDescription;
                if (preyInside == 0)
                {
                    yield return StartCoroutine(BellyJiggle(true));
                }
                else
                {
                    faces.SetCounterTo(4);
                    stuffedMoansPlayer.PlayRandom();
                    gurglePlayer.PlayRandom();
                    yield return StartCoroutine(BellyJiggle(false, 0.3f));
                    yield return StartCoroutine(BellyJiggle(false, 0.2f));
                    faces.SetCounterTo(3);
                    yield return StartCoroutine(BellyJiggle(false, 0.8f));
                    yield return new WaitForSeconds(0.4f);
                    faces.SetCounterTo(BellyToFaceIndex(false));
                }
            }
            /*else if (foodDescription == "" && Random.Range(0, 100) < GetPreyCount() * 25)
            {
                foodDescription = "You feel your prey struggling" + (preyHealth[0] > 20 ? "" : " weakly") + " inside your tummy.";
                trainingModifier += 0.05f;
                //if (imageIndex > 3) gurglePlayer.PlayRandom();
                if (trainingModifier >= 3f)
                {
                    trainingModifier = 3f;
                    if (!achievements[7]) UpdateAchievements(7);
                }
                foodText.text = foodDescription;
                yield return StartCoroutine(BellyJiggle(!isAsleep));
            }*/

            if (stomachContents > 0)
            {
                if (ateThisTurn)
                {
                    flowEnabled = false;
                    digestionTimer = achievements[2] ? 1 : 2; //upgrade to 1
                }
                else
                {
                    if (digestionTimer > 0) digestionTimer--;
                    if (digestionTimer == 0 && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier)) flowEnabled = true;

                }
                hungerTimer = 0;
                /*if (stomachContents >= stomachCapacity * trainingModifier) */hungerModifier = 1f + (0.2f * munchiesConsumed);
            }
            else
            {
                stomachContents = 0f;
                if (hungerTimer < 10 && intestineContents <= 0f && !isAsleep) hungerTimer++;

                if (hungerTimer > 3 && !gurglePlayer.GetComponent<AudioSource>().isPlaying)
                {
                    gurglePlayer.PlayCustom(hungrySound, hungerTimer / 20f);
                    holdFaceDuration = hungrySound.length;
                }
                if (hungerTimer > 5)
                {
                    faces.SetCounterTo(5);
                    //overrideFace.enabled = true;
                }

                hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
                if (!achievements[0] && hungerModifier >= 4f) UpdateAchievements(0);
                flowEnabled = false;
                //foodDescription = "";
            }
            foodText.text = foodDescription;

            if (coomContents > 0f && !usedPlug)
            {
                coomContents -= 0.1f;
                if (coomContents < 0f) coomContents = 0f;
            }
            if (coomContents + wombContents > 21f) coomContents = 21f - wombContents;
            coomContents = Mathf.Round(coomContents * 10000) / 10000;
            if (coomStorage < 3f)
            {
                coomStorage += 0.05f;
                if (coomStorage > 3f) coomStorage = 3f;
            }


            if (flowEnabled)
            {
                float preyVolume = GetPreyVolume();
                if (stomachContents <= preyVolume)
                {
                    DamageAllPrey();
                    //PrintStats();
                    //yield return new WaitForSeconds(0.2f);
                }
                if (stomachContents >= flowRate + preyVolume && (intestineContents + flowRate) <= intestineCapacity * intestineMultiplier * trainingModifier)
                {
                    stomachContents -= flowRate;
                    //Debug.Log(preyVolume);
                    intestineContents += flowRate;
                }
                else if (stomachContents >= intestineCapacity * intestineMultiplier * trainingModifier - intestineContents && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier) && (intestineContents + flowRate) > intestineCapacity * intestineMultiplier * trainingModifier)
                {
                    stomachContents -= ((intestineCapacity * intestineMultiplier * trainingModifier) - intestineContents);
                    intestineContents = intestineCapacity * intestineMultiplier * trainingModifier;
                    flowEnabled = false;
                }
                else if (stomachContents < flowRate + preyVolume)
                {
                    intestineContents += stomachContents - preyVolume;
                    stomachContents = preyVolume;
                }
                else
                {
                    Debug.Log("this should only happen if both stomach and intestines are stuffed, congratulations");
                    flowEnabled = false;
                }
                DamageAllPrey();
                liquidContents -= flowRate;
                /*if (inertSoda > 0)
                {
                    inertSoda--;
                    sodaInIntestines++;
                }
                else if (storedSoda > 0)
                {
                    storedSoda--;
                    sodaInIntestines++;
                }*/
                if (liquidContents < 0f) liquidContents = 0f;
            }
            if (intestineContents >= intestineCapacity * intestineMultiplier * trainingModifier) flowEnabled = false;

            if (intestineContents > 0)
            {
                if (!flowEnabled)
                {
                    disposalTimer++;
                }
                if ((currentTime == 23 || (currentTime == 1 && tookCaffeine)) && stomachContents == 0f && disposalTimer > 0)
                {
                    disposalTimer = disposalReq + 1;
                }
                if (disposalTimer > disposalReq && (!isAsleep || sleepCountdown == 1))
                {
                    dailyCalories += Mathf.Round(intestineContents * 1000);
                    if (!achievements[2] && (int)dailyCalories >= 8000) UpdateAchievements(2);
                    intestineContents = 0f;
                    sodaInIntestines = 0;
                    disposalTimer = 0;
                }
            }
            maintainEmptyBellyMessage = false;
            currentTime++;
            skipTime.SetCounterTo(currentTime == (tookCaffeine ? 1 : 23) ? 2 : 0);
            if (sleepCountdown > 0) sleepCountdown--;
            if (currentTime > 23)
            {
                currentTime = 0;
                if (fetusCount > 0) actualDays++;
            }
            if (currentTime == 0 + (tookCaffeine ? 2 : 0))
            { 
                sleepCountdown = 8 - (tookCaffeine ? 2 : 0);
                holdFaceDuration = 0f;
                isAsleep = true;
                tookCaffeine = false;
                if (!achievements[1] && eligibleForAchievement) UpdateAchievements(1);
            }

            //sexButton.GetComponent<Collider2D>().enabled = (coomStorage >= 1f && !isAsleep);          
            if (coomStorage >= 2f && !isAsleep && fetusCount == 0)
            {
                sexButton.GetComponent<AnimateSprite>().EnableAnimations(true);
            }
            else
            {
                sexButton.GetComponent<AnimateSprite>().EnableAnimations(false);
                sexButton.GetComponent<AnimateSprite>().SetAllColors((coomStorage >= 1f && !isAsleep) ? Color.white : new Color(1f, 1f, 1f, 0.2f));
            }

            if (currentTime == 16 && achievements[0] && weedStock < 5) weedStock++;

            if (currentTime == 4)
            {
                munchiesConsumed = 0;
                tookLaxative = false;
                fertilityBonus = 0;
                if (weedStock < 5)
                {
                    weedStock++;
                    weedButton.SetActive(weedStock > 0);
                    weedStockCounter.SetCounterTo(weedStock);
                    weedStockCounter.SetAltColor(weedStock >= 5);
                    //if (achievements[0] && weedStock < 5) weedStock++;
                }
                if (fetusCount > 0) pregnancyDays++;
                if (pregnancyDays > 4 && pregnancyDays < 16 && Random.Range(0, Mathf.Max(0, 9 - fetusCount)) == 0) isNauseous = true;
                if (daysUntilNextStream > 0) daysUntilNextStream--;
                //Debug.Log(pregnancyDays);
                if (fetusCount > 0)
                {
                    if (pregnancyDays < 20)
                    {
                        wombContents += 0.15f + ((dailyCalories >= 2000) ? (0.05f * fetusCount) : 0f);
                        //wombContents += (3f + fetusCount) * ((dailyCalories >= 2000) ? 0.05f : 0.01f);
                        if (coomContents + wombContents > 21f) coomContents = 21f - wombContents;
                        coomContents = Mathf.Round(coomContents * 10000) / 10000;
                    }
                    else if (pregnancyDays <= 40)
                    {
                        wombContents += 0.15f + ((dailyCalories >= 2000 + fetusCount * 500) ? (0.05f * fetusCount) : 0f);
                        //wombContents += (3f + fetusCount) * ((dailyCalories >= 2000 + fetusCount * 500) ? 0.05f : 0.01f); //Mathf.Clamp(0.025f * ((dailyCalories - 2000) / fetusCount) / 400, 0f, 0.025f));
                        if (coomContents + wombContents > 21f) coomContents = 21f - wombContents;
                        coomContents = Mathf.Round(coomContents * 10000) / 10000;
                    }
                    else
                    {
                        StartCoroutine(musicPlayer.GraduallyMute(2f));
                        stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier * barRatio, stomachCapacityBar.localScale.y, 1);
                        intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier * barRatio, intestineCapacityBar.localScale.y, 1);
                        yield return StartCoroutine(AnimateBirth());
                        musicPlayer.GetComponent<AudioSource>().time = 0f;
                        StartCoroutine(musicPlayer.GraduallyUnmute(2f));
                        wombContents = 0;
                        pregnancyDays = 0;
                        actualDays = 0;
                        lastSeenEmptyBelly = 0;
                        wombCapacityBar.localScale = new Vector3((6f + 2 * fetusCount) * barRatio, wombCapacityBar.localScale.y, 1);
                    }
                }
                //if (dailyCalories >= 2000 + fetusCount * 800) wombContents += fetusCount * 0.025f;
                //Mathf.Clamp(wombContents, 0f, fetusCount * pregnancyDays * 0.05f);

                if (fetusCount > 0) bankedCalories += (int)dailyCalories - (2000 + (500 * fetusCount * (pregnancyDays >= 20 ? 1 : 0)));
                bankedCalories -= fetusCount > 0 ? (600 - (pregnancyDays >= 20 ? (fetusCount * 300) : 0)) : 10000;
                bankedCalories = Mathf.Clamp(bankedCalories, 0, 100000);
                //Debug.Log("banked calories: " + bankedCalories);
                dailyCalories = 0;
            }

            if (isAsleep && sleepCountdown == 0)
            {
                isAsleep = false;
                if (!achievements[6] && stomachContents > stomachCapacity * trainingModifier) UpdateAchievements(6);
                if (!isNauseous && pregnancyDays > 3)
                {
                    if (Random.Range(0, 100) < (30 + 3 * fetusCount))
                    {
                        munchiesConsumed++;
                        if (Random.Range(0, 100) < (30 + 3 * fetusCount))
                        {
                            munchiesConsumed++;
                            foodDescription = "You wake up with intense cravings for food.";
                        }
                        else
                        {
                            foodDescription = "You wake up with heightened cravings for food.";
                        }
                        if (hungerModifier < 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed)) hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
                    }
                }
            }
            achievementButton.GetComponent<Collider2D>().enabled = !isAsleep;
            if (holdFaceDuration <= 0f && isAsleep)
            {
                //overrideFace.enabled = isAsleep;
                faces.SetCounterTo(0);
            }
            timeText.text = (ampmMode ? (ConvertToAMPM(currentTime)) : ((currentTime < 10 ? "0" : "") + currentTime + ":00")) + " | Day " + (fetusCount > 0 ? actualDays : "--");
            hungerText.text = "Hunger multiplier: " + hungerModifier + "x";

            //stomachContents = Mathf.Round(stomachContents * 1000) / 1000;
            //intestineContents = Mathf.Round(intestineContents * 1000) / 1000;
            //wombContents = Mathf.Round(wombContents * 1000) / 1000;
            //trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;
            if (stomachContents + gasContents <= (intestineContents + wombContents + coomContents) && topHeavyAtStart && imageIndex > 4) gurglePlayer.PlayRandom();
            PrintStats();

            if (actualDays == 40 && currentTime == 23 && !achievements[11]) UpdateAchievements(11);

            if (isAsleep)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }

    void PrintStats()
    {
        string bellyDescription = "You feel like a variable hasn't been assigned.";
        float totalBellyContents = stomachContents + intestineContents + wombContents + coomContents;
        float digestiveContents = Mathf.Round((stomachContents + intestineContents) * 1000) / 1000;
        switch (Mathf.Floor(digestiveContents))
        {
            case 0:
                if (digestiveContents > 0f)
                {
                    bellyDescription = "You feel like you can eat more.";
                }
                else
                {
                    bellyDescription = "Your belly is empty";
                    switch (hungerTimer)
                    {
                        case 0:
                        case 1:
                            //bellyDescription += "";
                            break;
                        case 2:
                        case 3:
                            bellyDescription += ", and you feel a bit hungry.";
                            break;
                        case 4:
                        case 5:
                            bellyDescription += " and starting to growl loudly.";
                            break;
                        case 6:
                        case 7:
                            bellyDescription += ", and feels like it could fit a meal for two and still have plenty of room left.";
                            break;
                        case 8:
                        case 9:
                        case 10:
                            bellyDescription += " and practically screaming to be crammed with as much food as it can possibly fit, and a few plates more for good measure.";
                            break;
                        default:
                            bellyDescription += ".";
                            break;
                    }
                }
                break;
            case 1:
                bellyDescription = "You feel satisfied.";
                break;
            case 2:
            case 3:
                bellyDescription = "You feel comfortably full.";
                break;
            case 4:
                bellyDescription = "You feel overwhelmingly full.";
                break;
            case 5:
                bellyDescription = "Your belly is so enormously stuffed that you can barely stand upright.";
                break;
            case 6:
            case 7:
                bellyDescription = "You can feel your immensely stuffed belly stretching to accomodate the tremendous amount of " + (liquidContents / stomachContents > 0.8f ? "liquid" : "food") + " packed inside of it.";
                break;
            case 8:
            case 9:
                bellyDescription = "Your hugely swollen belly is so thoroughly stuffed that it feels like a boulder. " + (imageIndex < 13 ? "Your arms can barely reach your belly button." : (imageIndex > 21 ? "Your arms can't even reach halfway to your belly button." : "Your arms can no longer reach your belly button."));
                break;
            case 10:
            case 11:
                bellyDescription = "The equivalent of an entire family-sized Thanksgiving feast now sits inside your massively engorged belly. You are so full that it is getting difficult to breathe.";
                break;
            case 12:
            case 13:
                bellyDescription = "The incredible amount of " + (liquidContents / stomachContents > 0.8f ? "liquid" : "food") + " in your belly " + (wombContents <= 8f ? "has expanded it beyond the size of a full-term pregnancy." : "feels as heavy as the weight of the " + IntToNumberofBabies(fetusCount) + " resting inside your distended womb.") + " The overwhelming sensation of fullness threatens to shut down your brain.";
                break;
            case 14:
            case 15:
                bellyDescription = "The signals of immense fullness from your ridiculously huge belly are laced with streaks of pain as you approach the physical limit of how much food you can cram into yourself.";
                break;
            case 16:
            case 17:
                bellyDescription = "Your gigantic belly has gone far beyond the limit of what the human body is meant to withstand. You feel your consciousness slipping away as your brain is bombarded with intense sensations of orgasmic pleasure and pain from your tightly stretched skin, abdominal muscles, and overstuffed guts.";
                break;
            default:
                bellyDescription = "Every inch of your digestive tract is stretched like a balloon, and your massively overstretched belly contains what feels like an entire ocean of " + (liquidContents / stomachContents > 0.8f ? "soda" : "food") + ". You cannot even burp, as a burp would imply the existence of a pocket of air trapped somewhere in your enormously distended abdomen, and every last bit of space relieved by previous burps has already been " + (liquidContents / stomachContents > 0.8f ? "filled with even more soda" : "crammed with even more food") + ". By volume, you are almost more food than girl at this point and you feel like you might burst if you force down one more bite.";
                if (!achievements[3]) UpdateAchievements(3);
                break;

        }
        int volumeInt = (int)Mathf.Floor(totalBellyContents + gasContents + 0.001f);
        if (volumeInt <= 20)
        {
            imageIndex = (int)Mathf.Min(characterSpritesBtm.Length / 2 - 1, volumeInt);
        }
        else
        {
            imageIndex = (int)Mathf.Min(characterSpritesBtm.Length / 2 - 1, 20 + (volumeInt - 20) / 2);
        }

        if (digestiveContents <= 0f && hungerTimer < 2)
        {
            if ((lastSeenEmptyBelly < imageIndex || maintainEmptyBellyMessage) && gasContents <= 0f)
            {
                if (coomContents < 1f)
                {
                    switch (imageIndex)
                    {
                        case 0:
                            break;
                        case 1:
                            bellyDescription += ". \n\nYou're not sure if you're imagining it, but it feels a bit rounder than usual.";
                            break;
                        case 2:
                            bellyDescription += ". \n\nThere is definitely a slight swell to it compared to the last time you checked.";
                            break;
                        case 3:
                            bellyDescription += ". \n\nHowever, it is starting to stick out a bit. You won't be able to hide this bump for much longer.";
                            break;
                        case 4:
                            bellyDescription += ". \n\nIt is getting noticeably rounder thanks to the " + (fetusCount == 1 ? "baby" : "babies") + " growing inside.";
                            break;
                        case 5:
                            bellyDescription += ". \n\nYou are really starting to feel the weight of the " + (fetusCount == 1 ? "baby" : "babies") + " growing within it.";
                            break;
                        case 6:
                            bellyDescription += ". \n\nWith all of that food out of the way, you now have a chance to appreciate how big your " + (fetusCount == 1 ? "baby" : "babies") + " have gotten.";
                            break;
                        case 7:
                            bellyDescription += ". \n\nWith no more food in your system, you now have a full, unobstructed view of your heavily pregnant belly.";
                            break;
                        case 8:
                            bellyDescription += ". \n\nIt has now reached the size of a normal full-term pregnancy." + (fetusCount > 1 ? " You feel a combination of nervousness and excitement knowing that it will continue to grow larger still." : "");
                            break;
                        case 9:
                            bellyDescription += ". \n\nYou can feel it stretching to contain the babies growing inside, and it will have to stretch further still if you want feed them the nutrition that they need.";
                            break;
                        case 10:
                            bellyDescription += ". \n\nIt has grown large enough that the lower part of your belly is now impossible to reach.";
                            break;
                        case 11:
                            bellyDescription += ". \n\nHowever, it is still much larger than a normal pregnancy, making it very obvious that there is more than one - no, more than two babies growing inside.";
                            break;
                        case 12:
                            bellyDescription += ". \n\n" + (fetusCount == 3 ? "The three babies inside are now fully-grown, and you give your belly a satisfied rub." : "It is now equivalent in size to a full-term triplet pregnancy, and yet it is still not done growing.");
                            break;
                        case 13:
                            bellyDescription += ". \n\nYou take a look in the mirror to see just how far your pregnancy has developed. It is now clear that your belly contains more than three babies.";
                            break;
                        case 14:
                            bellyDescription += ". \n\nAlthough you've grown somewhat accustomed to the weight of your " + IntToWord(fetusCount) + " babies by now, your belly still feels unbelievably heavy and tight.";
                            break;
                        case 15:
                            bellyDescription += ". \n\nAlthough your babies are not done growing, you feel an odd sense of pride in how huge they've gotten. At this size, you can tell that there are more than four.";
                            break;
                        case 16:
                            bellyDescription += ". \n\nYou use this break between stuffing sessions as an opportunity to admire your massive belly, generously filled out with " + (fetusCount == 5 ? "five fully-grown babies." : (IntToWord(fetusCount) + " still-growing babies."));
                            break;
                        case 17:
                            bellyDescription += ". \n\nEven without a massive meal stretching it out, your belly is still impossibly huge, with its incredible size dominating your otherwise slender frame. At this size, it must contain at least six babies.";
                            break;
                        case 18:
                            bellyDescription += ". \n\nIt is so enormous that most people couldn't even begin to imagine how many babies are growing inside.";
                            break;
                        case 19:
                            bellyDescription += ". \n\nAlthough your babies are not done growing yet, the incredible size of your belly makes it obvious that you are in the late stages of a very large multiple pregnancy.";
                            break;
                        case 20:
                            bellyDescription += ". \n\nYou take a moment to admire its sheer size, with your womb stretched to the limit by the seven fully-grown babies resting inside. This is as pregnant as you can possibly get.";
                            break;
                        default:
                            bellyDescription += ". \n\nYou would have something to say about the size of your belly, but your womb contents should never reach this size during normal gameplay. " + imageIndex;
                            break;
                    }
                    lastSeenEmptyBelly = imageIndex;
                    maintainEmptyBellyMessage = true;
                }
                else
                {
                    bellyDescription += ". \n\nYou feel a stretching sensation in your womb, but that's to be expected with all the baby batter that's been pumped into you.";
                }

            }
            else
            {
                bellyDescription += ".";
            }
        }


        if (!achievements[8] && imageIndex == characterSpritesBtm.Length/2 - 1) UpdateAchievements(8);
        /*switch (Mathf.Floor(totalBellyContents))
        {
            case 0:
                if (totalBellyContents > 0.5f)
                {
                    imageIndex = 1;
                }
                else
                {
                    imageIndex = 0;
                }
                break;
            case 1:
                imageIndex = 2;
                break;
            case 2:
                imageIndex = 3;
                break;
            case 3:
            case 4:
                imageIndex = 4;
                break;
            case 5:
            case 6:
                imageIndex = 5;
                break;
            case 7:
            case 8:
                imageIndex = 6;
                break;
            case 9:
            case 10:
                imageIndex = 7;
                break;
            case 11:
            case 12:
                imageIndex = 8;
                break;
            case 13:
            case 14:
                imageIndex = 9;
                break;
            case 15:
            case 16:
                imageIndex = 10;
                break;
            case 17:
            case 18:
            case 19:
                imageIndex = 11;
                break;
            case 20:
            case 21:
            case 22:
                imageIndex = 12;
                break;
            default:
                imageIndex = 13;
                if (!achievements[8]) UpdateAchievements(8);
                break;
        }*/
        /*if (isStreaming)
        {
            lastJiggledSize = imageIndex - startingSize;
            //Debug.Log(deltaSize);
        }*/
        xRayWomb.localScale = new Vector3(Mathf.Max(0.2f, 0.05f + (int)(wombContents) * 0.05f), Mathf.Max(0.2f, 0.05f + (int)(wombContents) * 0.05f), 1f);
        coomWomb.localScale = new Vector3(Mathf.Max(0.2f, 0.05f + (int)(wombContents + coomContents) * 0.05f), Mathf.Max(0.2f, 0.05f + (int)(wombContents + coomContents) * 0.05f), 1f);
        coomWombSprite.enabled = coomContents > 0f;
        if (pregnancyDays < 20)
        {
            wombSprites.SetCounterTo(0);
        }
        else
        {
            wombSprites.SetCounterTo(Mathf.Min(7, fetusCount));
        }
        caloriesText.text = Mathf.Round(displayedCalories) + " / " + (fetusCount > 0 ? (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0)) : "----");
        caloriesText.color = (Mathf.Round(dailyCalories) >= (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0)) && fetusCount > 0) ? new Color(0.5058824f, 1f, 0.3803922f, 1f) : Color.white;
        RefreshTouchColliders();
        UpdateWombTattoo(stomachContents + gasContents > intestineContents + wombContents + coomContents);
        SetBellySprites(stomachContents + gasContents > intestineContents + wombContents + coomContents, imageIndex);
    
        //Debug.Log("Total belly contents: " + (stomachContents + intestineContents) + " | " + bellyDescription);
        bellyText.text = bellyDescription;

        stomachContents = Mathf.Round(stomachContents * 10000) / 10000;
        gasContents = Mathf.Round(gasContents * 10000) / 10000;
        liquidContents = Mathf.Round(liquidContents * 10000) / 10000;
        intestineContents = Mathf.Round(intestineContents * 10000) / 10000;
        coomContents = Mathf.Round(coomContents * 10000) / 10000;
        wombContents = Mathf.Round(wombContents * 10000) / 10000;
        trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;

        coomStorageBar.localScale = new Vector3(coomStorage / 3f, coomStorageBar.localScale.y, 1);

        stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier * barRatio, stomachCapacityBar.localScale.y, 1);
        //stomachContentsBar.localScale = new Vector3(displayedStomachContents * barRatio, stomachContentsBar.localScale.y, 1);
        //gasContentsBar.localScale = new Vector3((displayedStomachContents + displayedGasContents) * barRatio, gasContentsBar.localScale.y, 1);
        contentsText[0].text = Mathf.Round((stomachContents + gasContents) * 100) / 100 + "L / " + Mathf.Round(stomachCapacity * trainingModifier * 100) / 100 + "L";
        intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier * barRatio, intestineCapacityBar.localScale.y, 1);
        //intestineContentsBar.localScale = new Vector3(displayedIntestineContents * barRatio, intestineContentsBar.localScale.y, 1);
        contentsText[1].text = Mathf.Round(intestineContents * 100) / 100 + "L / " + Mathf.Round(intestineCapacity * intestineMultiplier * trainingModifier * 100) / 100 + "L";
        //wombContentsBar.localScale = new Vector3(displayedWombContents * barRatio, wombContentsBar.localScale.y, 1);
        //coomContentsBar.localScale = new Vector3((displayedCoomContents + displayedWombContents) * barRatio, coomContentsBar.localScale.y, 1);
        wombCapacityBar.localScale = new Vector3((6f + 2 * fetusCount) * barRatio, wombCapacityBar.localScale.y, 1);
        contentsText[2].text = Mathf.Round((wombContents + coomContents) * 100) / 100 + "L / " + Mathf.Round((2 * fetusCount + 6) * 100) / 100 + "L";

        //Debug.Log("Time: " + currentTime + ":00 | stomach: " + (Mathf.Round(stomachContents * 1000) / 1000) + "L / " + (stomachCapacity * trainingModifier) + "L | intestines: " + (Mathf.Round(intestineContents * 1000) / 1000) + "L / " + (intestineCapacity * intestineMultiplier * trainingModifier) + "L | hunger modifier = " + hungerModifier);

    }

    IEnumerator IncrementDisplayedValues()
    {
        float speedMultiplier = 5f;
        while (true)
        {
            displayedStomachContents += (stomachContents - displayedStomachContents) * Time.deltaTime * speedMultiplier;
            if (Mathf.Abs(stomachContents - displayedStomachContents) < 0.01f) displayedStomachContents = stomachContents;
            stomachContentsBar.localScale = new Vector3(displayedStomachContents * barRatio, stomachContentsBar.localScale.y, 1);
            displayedIntestineContents += (intestineContents - displayedIntestineContents) * Time.deltaTime * speedMultiplier;// / 100;
            if (Mathf.Abs(intestineContents - displayedIntestineContents) < 0.01f) displayedIntestineContents = intestineContents;
            intestineContentsBar.localScale = new Vector3(displayedIntestineContents * barRatio, intestineContentsBar.localScale.y, 1);
            displayedWombContents += (wombContents - displayedWombContents) * Time.deltaTime * speedMultiplier;// / 100;
            if (Mathf.Abs(wombContents - displayedWombContents) < 0.01f) displayedWombContents = wombContents;
            wombContentsBar.localScale = new Vector3(displayedWombContents * barRatio, wombContentsBar.localScale.y, 1);
            displayedCoomContents += (coomContents - displayedCoomContents) * Time.deltaTime * speedMultiplier;// / 100;
            if (Mathf.Abs(coomContents - displayedCoomContents) < 0.01f) displayedCoomContents = coomContents;
            coomContentsBar.localScale = new Vector3((displayedCoomContents + displayedWombContents) * barRatio, coomContentsBar.localScale.y, 1);
            displayedGasContents += (gasContents - displayedGasContents) * Time.deltaTime * speedMultiplier;// / 100;
            if (Mathf.Abs(gasContents - displayedGasContents) < 0.01f) displayedGasContents = gasContents;
            gasContentsBar.localScale = new Vector3((displayedGasContents + displayedStomachContents) * barRatio, gasContentsBar.localScale.y, 1);
            if (isStreaming && displayedEarnings < streamEarnings)
            {
                displayedEarnings += (streamEarnings - displayedEarnings > 10 ? 10 : 1);
                donationsText.text = "Donations: $" + displayedEarnings;
            }

            if (((int)dailyCalories - displayedCalories) / 10 != 0)
            {
                displayedCalories += ((int)dailyCalories - displayedCalories) / 10;
            }
            else if ((int)dailyCalories != displayedCalories)
            {
                displayedCalories += (int)Mathf.Sign(dailyCalories - displayedCalories);
            }
            caloriesText.text = Mathf.Round(displayedCalories) + " / " + (fetusCount > 0 ? (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0)) : "----");
            yield return null;
        }
    }

    void SetBellySprites(bool isTopHeavy, int index)
    {
        spriteRenderer.sprite = (isTopHeavy ? characterSpritesTop[index + (largeBreastMode ? 28 : 0)] : characterSpritesBtm[index + (largeBreastMode ? 28 : 0)]);
        sideviewBase.GetComponent<DigitCounter>().SetCounterTo(largeBreastMode ? 1 : 0);
        if (isTopHeavy)
        {
            sideviewBottom.GetComponent<SpriteRenderer>().enabled = false;
            sideviewTop.GetComponent<SpriteRenderer>().enabled = true;
            sideviewTop.SetCounterTo(index + (largeBreastMode ? 28 : 0));
        }
        else
        {
            sideviewTop.GetComponent<SpriteRenderer>().enabled = false;
            sideviewBottom.GetComponent<SpriteRenderer>().enabled = true;
            sideviewBottom.SetCounterTo(index + (largeBreastMode ? 28 : 0));
        }
    }

    void RefreshTouchColliders()
    {
        for (int i = 0; i < touchColliders.Length; i++)
        {
            touchColliders[i].SetActive(i == imageIndex);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        Settings.SaveEnabled = true;
    }
}
