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

    [SerializeField] private Text achievementBoard;
    [SerializeField] private Text achievementRewards;

    [SerializeField] private AudioPlayer musicPlayer;
    [SerializeField] private AudioPlayer stuffedMoansPlayer;
    [SerializeField] private AudioPlayer gulpPlayer;
    [SerializeField] private AudioPlayer gurglePlayer;
    [SerializeField] private AudioClip hungrySound;
    [SerializeField] private AudioClip lastBirthSound;
    [SerializeField] private AudioClip hiccupSound;
    [SerializeField] private AudioPlayer streamDigestionSounds;

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
    [SerializeField] private Sprite draggableFood;

    [SerializeField] private Transform stomachCapacityBar;
    [SerializeField] private Transform stomachContentsBar;
    [SerializeField] private Transform intestineCapacityBar;
    [SerializeField] private Transform intestineContentsBar;
    [SerializeField] private Transform wombContentsBar;
    [SerializeField] private Transform coomContentsBar;
    [SerializeField] private Transform wombCapacityBar;
    [SerializeField] private Transform coomStorageBar;
    [SerializeField] private Transform xRayWomb;
    [SerializeField] private Transform coomWomb;
    [SerializeField] private DigitCounter wombSprites;
    [SerializeField] private SpriteRenderer coomWombSprite;
    private Vector3 xRayStartPosition;

    [SerializeField] private GameObject weedButton;
    [SerializeField] private GameObject foodButton;
    [SerializeField] private GameObject sexButton;
    [SerializeField] private GameObject chatButton;
    [SerializeField] private GameObject tattooToggle;
    [SerializeField] private GameObject xRayToggle;
    [SerializeField] private GameObject deleteSaveButton;
    [SerializeField] private ToggleButton alwaysEatButton;
    [SerializeField] private ToggleButton achievementButton;
    [SerializeField] private ToggleButton pillButton;
    [SerializeField] private ToggleButton recordButton;
    [SerializeField] private GameObject[] touchColliders;
    [SerializeField] private DigitCounter faces;
    [SerializeField] private DigitCounter weedStockCounter;
    [SerializeField] private DigitCounter wombTattoo;

    [SerializeField] private string[] messageList;
    private bool[] eligibleMessages;
    private bool[] sentMessages;

    public bool[] achievements = new bool[12];
    [SerializeField] private GameObject[] medicineButtons = new GameObject[7];
    [SerializeField] private GameObject[] plates = new GameObject[50];
    [SerializeField] private int[] medicinePrices = new int[7];
    [SerializeField] private Text medicinePricesText;

    public string clickedButtonName = "";

    /*
     * gestation: 40 weeks (shorten: 40 days)
     * average birth weight: 4 kg, range 3-5
     * 1kg = 1L
     * stomach psychological limit: 1L * training multiplier * hunger multiplier
     * stomach physical limit: 3L * training multiplier
     * training multipler up to 2x
     * per hour (content-psychmax - 1) * n added to training multiplier, updated 0:00
     * if contents == 0, hunger multiplier increased by 0.5 / hr, max 3.0
     * after 4hr with contents == 0, training multiplier decreases slightly
     * intestinal flow rate = 1 L/hr
     * flow rate does not start until 2hrs with content > 0
     * intestine capacity 3L, empties fully after 3hr with capacity > 0. all calories absorbed
     * fetus req 400 cal/day
     * main req 2000 cal/day
     * daily weight increase in kg: 0.075 + min [0.025, 0.025*[(total cal - mainReq)/fetusCount]/400]
     * theoretical max: (6*2)+(2*4 + 0.8)+4 = 24.8L
     * day end 0:00 day start 8:00
     * 0/232/426/735/1122/1548/1974/2632/3252/3677/4000/3574/4000
     * -1/0, 0/1 : -2/0, -1/1
     * 
     * 5 + 2n
     * 
     * 1:0.5
     * 2:1
     * 3:2
     * 4:3
     * 5:5 
     * 6:7 =1
     * 7:9 =2
     * 8:11=3
     * 9:13 =4
     * 10:15 =5
     * 11:17 =6
     * 12:20
     * 13:23
     * 
     * BUG: under certain conditions, weed leaf decreases hunger multiplier
     * training capacity doesn't visually update until after eating
     * belly description sometimes changes between 6/default when it shouldn't
     * sometimes stomach contents overflow intestines
     * 
     * 
     * for implementation: slightly randomize food intake?
     * calories digested use rolling animation
     * 
     * */
    float holdFaceDuration = 0f;
    bool isPlayingJiggleAnim = false;

    float stomachCapacity = 1.0f;
    public float stomachContents = 0f; //save
    public int digestionTimer = 0; //save
    public int hungerTimer = 0; //save
    public float coomContents = 0f; //save
    public float coomStorage = 3f; //save

    public float trainingModifier = 1.0f; //save
    public float hungerModifier = 1.0f;
    public float intestineMultiplier = 1.5f; //save

    public int munchiesConsumed = 0; //save
    public int weedStock = 0; //save

    public int money = 0;
    public int cumulativeEarnings = 0;

    bool flowEnabled = false;
    public float flowRate = 0.3f; //save
    float intestineCapacity = 1.0f;
    public float intestineContents = 0f; //save
    public int disposalTimer = 0; //save
    int disposalReq = 4; //save

    public float dailyCalories = 1000f; //save
    int bankedCalories = 0;

    public float wombContents = 0f; //save
    public int fetusCount; //save
    int maxFetusCount = 7;
    public int pregnancyDays = 0; //save
    public int actualDays = 0; //save
    string foodDescription = "";

    public int currentTime = 8; //save
    public int sleepCountdown = 0; //save
    public bool isAsleep = false; //save
    bool isBouncing = false;
    bool didElbows = false;
    public bool tookCaffeine = false; //save
    public bool isNauseous = false; //save
    public bool isStreaming = false; //save
    public bool alwaysUseEatingAnimation = false; //save
    public bool tattooToggledOn = false; //save
    public bool xRayToggledOn = false;
    public bool jiggledDuringStream = false;
    public bool tookLaxative = false; //save
    public bool usedPlug = false; //save
    public int daysUntilNextStream = 0;
    public bool playingDigestionSounds = false;

    int imageIndex = 0;
    int startingSize = 0;
    int lastJiggledSize = 0;
    [SerializeField] Sprite[] characterSpritesBtm = new Sprite[14];
    [SerializeField] Sprite[] characterSpritesTop = new Sprite[14];

    public void SaveGame()
    {
        SaveData saveData = new SaveData();

        saveData.stomachContents = stomachContents;
        saveData.coomContents = coomContents;
        saveData.coomStorage = coomStorage;
        saveData.intestineContents = intestineContents;
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
        saveData.wombContents = wombContents;
        saveData.fetusCount = fetusCount;
        saveData.pregnancyDays = pregnancyDays;
        saveData.actualDays = actualDays;
        saveData.currentTime = currentTime;
        saveData.tookCaffeine = tookCaffeine;
        saveData.isNauseous = isNauseous;
        saveData.isStreaming = isStreaming;
        saveData.jiggledDuringStream = jiggledDuringStream;
        saveData.tookLaxative = tookLaxative;
        saveData.usedPlug = usedPlug;
        saveData.daysUntilNextStream = daysUntilNextStream;
        saveData.sleepCountdown = sleepCountdown;
        saveData.isAsleep = isAsleep;
        saveData.achievements = achievements;
        saveData.alwaysUseEatingAnimation = alwaysUseEatingAnimation;
        saveData.tattooToggledOn = tattooToggle.GetComponent<ToggleButton>().isActive;
        saveData.xRayToggledOn = xRayToggle.GetComponent<ToggleButton>().isActive;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savedGame.json", json);
        //Debug.Log(Application.persistentDataPath);
        //Debug.Log(json);
    }

    public void LoadBlankSaveData()
    {
        stomachContents = 0f;
        intestineContents = 0f;
        coomContents = 0f;
        coomStorage = 3f;
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
        wombContents = 0f;
        fetusCount = 0;
        pregnancyDays = 0;
        actualDays = 0;
        currentTime = 8;
        tookCaffeine = false;
        isNauseous = false;
        isStreaming = false;
        jiggledDuringStream = false;
        tookLaxative = false;
        usedPlug = false;
        daysUntilNextStream = 0;
        sleepCountdown = 0;
        isAsleep = false;
        achievements = new bool[12];
        alwaysUseEatingAnimation = false;
        tattooToggledOn = false;
        xRayToggledOn = false;
    }

    public void WipeSaveData()
    {
        SaveData saveData = new SaveData();

        saveData.stomachContents = 0f;
        saveData.intestineContents = 0f;
        saveData.coomContents = 0f;
        saveData.coomStorage = 3f;
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
        saveData.wombContents = 0f;
        saveData.fetusCount = 0;
        saveData.pregnancyDays = 0;
        saveData.actualDays = 0;
        saveData.currentTime = 8;
        saveData.tookCaffeine = false;
        saveData.isNauseous = false;
        saveData.isStreaming = false;
        saveData.jiggledDuringStream = false;
        saveData.tookLaxative = false;
        saveData.usedPlug = false;
        saveData.daysUntilNextStream = 0;
        saveData.sleepCountdown = 0;
        saveData.isAsleep = false;
        saveData.achievements = new bool[12];
        saveData.alwaysUseEatingAnimation = false;
        saveData.tattooToggledOn = false;
        saveData.xRayToggledOn = false;

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
        deleteSaveButton.SetActive(Settings.SaveEnabled);
        if (achievements.Length < 12)
        {
            bool[] updatedAchievements = new bool[12];
            for (int i = 0; i < achievements.Length; i++)
            {
                updatedAchievements[i] = achievements[i];
            }
            achievements = updatedAchievements;
        }
        eligibleMessages = new bool[messageList.Length];
        sentMessages = new bool[messageList.Length];
        xRayStartPosition = xRayWomb.localPosition;
        yield return null;
        alwaysEatButton.ForceState(alwaysUseEatingAnimation);
        xRayToggle.GetComponent<ToggleButton>().ForceState(achievements[5] && xRayToggledOn);
        xRayToggle.SetActive(achievements[5]);
        tattooToggle.GetComponent<ToggleButton>().ForceState(achievements[4] && tattooToggledOn);
        tattooToggle.SetActive(achievements[4]);
        PrintAchievementBoard();
        StartCoroutine(MainRoutine());
    }

    void UpdateEligibleMessages(int foodEaten, bool[] seenInteractions)
    {
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
        eligibleMessages[13] = foodEaten > 1;
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
        eligibleMessages[37] = true;
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
        eligibleMessages[48] = imageIndex > 4 && foodEaten > 1;
        eligibleMessages[49] = true;
        eligibleMessages[50] = foodEaten > 6 && foodEaten < 10;
        eligibleMessages[51] = imageIndex > 12;
        eligibleMessages[52] = foodEaten > 15;
        eligibleMessages[53] = fetusCount > 1 && seenInteractions[4] && foodEaten > 15;
        eligibleMessages[54] = wombContents > 3f && cumulativeEarnings > 0;
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
        eligibleMessages[98] = seenInteractions[4] || seenInteractions[5];
    }

    int BellyToFaceIndex(bool isJiggling)
    {
        int reactionFaceIndex;
        float foodAndCoomContents = Mathf.Round((stomachContents + intestineContents + coomContents) * 1000) / 1000;
        switch (Mathf.Floor(foodAndCoomContents))
        {
            case 0:
                reactionFaceIndex = isJiggling ? 0 : (imageIndex < 6 ? 7 : 2);
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                reactionFaceIndex = isJiggling ? (imageIndex < 6 ? 2 : 1) : (imageIndex < 6 ? 7 : 2);
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
        return reactionFaceIndex;
    }

    public IEnumerator BellyJiggle(bool useDefaultBehavior)
    {
        if (isPlayingJiggleAnim && !useDefaultBehavior) yield break;
        isPlayingJiggleAnim = true;
        //bool initialState = overrideFace.enabled;
        bool initialButtonState = achievementButton.GetComponent<Collider2D>().enabled;

        achievementButton.GetComponent<Collider2D>().enabled = false;
        recordButton.GetComponent<Collider2D>().enabled = false;

        if (useDefaultBehavior && !minigameUI.activeInHierarchy)
        {
            //overrideFace.enabled = true;

            if (Mathf.Floor(Mathf.Round((stomachContents + intestineContents + coomContents) * 1000) / 1000) >= 5)
            {
                if (!isAsleep) stuffedMoansPlayer.PlayRandom();
                if (stomachContents + intestineContents > 1.5f && Random.Range(0, Mathf.Max(0, 8 - imageIndex)) == 0) gurglePlayer.PlayRandom();
            }


            faces.SetCounterTo(BellyToFaceIndex(true));
        }


        float baseJiggleRate = 0.1f;
        baseJiggleRate *= 1 + ((float)imageIndex / 20);

        bool topHeavy = stomachContents > intestineContents + wombContents + coomContents;
        spriteRenderer.sprite = topHeavy ? characterSpritesBtm[imageIndex] : characterSpritesTop[imageIndex];
        UpdateWombTattoo(!topHeavy);
        yield return new WaitForSeconds(baseJiggleRate * 1);
        spriteRenderer.sprite = topHeavy ? characterSpritesTop[imageIndex] : characterSpritesBtm[imageIndex];
        UpdateWombTattoo(topHeavy);
        yield return new WaitForSeconds(baseJiggleRate * 2);
        if (imageIndex >= 6)
        {
            if (isStreaming && lastJiggledSize < imageIndex)
            {
                jiggledDuringStream = true;
                lastJiggledSize = imageIndex;
                string bellyDescriptor = "swollen ";
                if (imageIndex > 7) bellyDescriptor = "huge ";
                if (imageIndex > 13) bellyDescriptor = "enormous ";
                if (imageIndex > 18) bellyDescriptor = "gigantic ";
                foodDescription = "A few extra donations roll in as your " + bellyDescriptor + "belly jiggles in front of the camera.";
                if (!sentMessages[77])
                {
                    commentGenerator.GenerateComment("BELLY JIGGLE LET'S FUCKING GOOOOOOO", Random.Range(-1f, 1f));
                    sentMessages[77] = true;
                }
            }
            spriteRenderer.sprite = topHeavy ? characterSpritesBtm[imageIndex] : characterSpritesTop[imageIndex];
            UpdateWombTattoo(!topHeavy);
            yield return new WaitForSeconds(baseJiggleRate * 1);
            spriteRenderer.sprite = topHeavy ? characterSpritesTop[imageIndex] : characterSpritesBtm[imageIndex];
            UpdateWombTattoo(topHeavy);
        }
        recordButton.GetComponent<Collider2D>().enabled = !isStreaming && !isAsleep && daysUntilNextStream <= 0;

        if (useDefaultBehavior && !minigameUI.activeInHierarchy && !isAsleep && stomachContents + intestineContents > 2f && Random.Range(0, Mathf.Max(3, 10 - imageIndex)) == 0)
        {
            StartCoroutine(gaspPlayer.PlayCustomWaitFor(hiccupSound, stuffedMoansPlayer.GetComponent<AudioSource>()));
            StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, 0.2f, 0.08f));
        }
        achievementButton.GetComponent<Collider2D>().enabled = initialButtonState;

        if (useDefaultBehavior) holdFaceDuration = 0.6f;
        isPlayingJiggleAnim = false;
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
        medicineButtons[0].SetActive(intestineMultiplier < 2f);
        medicineButtons[1].SetActive(stomachContents > 0f && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier));
        medicineButtons[2].SetActive(intestineContents > 0f && !isStreaming && !tookLaxative);
        medicineButtons[3].SetActive(Mathf.Round(wombContents * 10000) / 10000 < Mathf.Round((fetusCount + 2.5f) * 500f * pregnancyDays) / 10000);
        medicineButtons[4].SetActive(hungerTimer < 10);
        medicineButtons[5].SetActive(!tookCaffeine);
        medicineButtons[6].SetActive(!usedPlug && coomContents > 0f);
        medicineButtons[7].SetActive(achievements[10] && pregnancyDays == 0 && fetusCount < maxFetusCount);

        string updatedText = "";
        updatedText += "Relaxant: +0.1 intestine capacity (max 2.0)\n\n";
        updatedText += "Enzyme: digest some food in stomach\n\n";
        updatedText += "Laxative: " + (isStreaming ? "can't take laxatives while streaming\n\n" : (tookLaxative ? "already taken today\n\n" : "empty intestines for 50% calories\n\n"));
        updatedText += "Folate: restore 1 day of missed growth\n\n";
        updatedText += "Ghrelin: +1 hr of natural hunger\n\n";
        updatedText += "Caffeine: sleep at 2:00\n\n";
        updatedText += "Cervical plug: " + (usedPlug ? "already used today\n\n" : "stop leakage until next 8:00\n\n");
        if (achievements[10]) updatedText += "Egg implant: +1 fetus (Day 0 only, max " + maxFetusCount + ")";

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
                rewardMessage = "Reward: womb tattoo";
                wombTattoo.GetComponent<SpriteRenderer>().enabled = true;
                tattooToggle.SetActive(true);
                tattooToggle.GetComponent<ToggleButton>().ForceState(true);
                break;
            case 5:
                achievementMessage = "Opening Kickoff: Experience your first fetal movement.";
                rewardMessage = "Reward: X-ray button";
                xRayToggle.SetActive(true);
                xRayToggle.GetComponent<ToggleButton>().ForceState(true);
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
                break;
            case 9:
                achievementMessage = "Queen of Kebabs: Earn a total of $10000 through streaming.";
                rewardMessage = "Reward: stream every day";
                break;
            case 10:
                achievementMessage = "Seed of Destiny: Deliver a perfectly healthy baby.";
                rewardMessage = "Reward: egg implants unlocked";
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
                    rewardMessage = "womb tattoo";
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
                    rewardMessage = "--";
                    break;
                case 9:
                    achievementName = "Queen of Kebabs";
                    achievementDescription = "Earn a total of $10000 through streaming.";
                    rewardMessage = "stream every day";
                    break;
                case 10:
                    achievementName = "Seed of Destiny";
                    achievementDescription = "Give birth to a perfectly healthy baby.";
                    rewardMessage = "egg implants unlocked";
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
        wombTattoo.SetCounterTo(imageIndex + (isTopHeavy ? 0 : 27));
        xRayWomb.localPosition = xRayStartPosition + (isTopHeavy ? new Vector3(0f, 0.1f * (int)(Mathf.Max(4f, wombContents) + coomContents) / 19, 0f) : Vector3.zero) + new Vector3(-0.02f * (imageIndex - (int)(Mathf.Max(4f, wombContents) + coomContents)), -0.04f * (imageIndex - (int)(Mathf.Max(4f, wombContents) + coomContents)), 0f);
        coomWomb.localPosition = xRayWomb.localPosition;
    }

    public IEnumerator Bounce(float bounceDuration)
    {
        //yield return null;
        if (!isBouncing)
        {
            isBouncing = true;
            float initialYPos = transform.position.y;
            float timer = 0;

            while (timer < bounceDuration)
            {
                transform.position = new Vector3(transform.position.x, initialYPos + 0.1f * Mathf.Sin(timer * Mathf.PI / bounceDuration), transform.position.z);
                timer += Time.deltaTime;
                yield return null;
            }
            transform.position = new Vector3(transform.position.x, initialYPos, transform.position.z);
            isBouncing = false;
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator AnimateBirth()
    {
        if (fetusCount == 0) yield break;
        foodText.text = "You feel your contractions starting.";
        timeText.text = currentTime + ":00 | Day " + actualDays;
        faces.SetCounterTo(5);
        float babyVolume = (wombContents - 5) / fetusCount;

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

        float amountToDecrement = 2f;//wombContents / fetusCount;

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
        PrintStats();
        //wombContentsBar.localScale = new Vector3(wombContents / 2, wombContentsBar.localScale.y, 1);
        yield return new WaitForSeconds(1f);
        faces.SetCounterTo(0);
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
        achievementButton.Brighten(false);
        yield return StartCoroutine(sexMinigame.Oscillate(startPregnancy, coomStorage));
        achievementButton.Brighten(true);
        achievementButton.GetComponent<Collider2D>().enabled = true;
        StartCoroutine(musicPlayer.ChangeTrackTo(1, 3f));
        if (fetusCount == 0) fetusCount = 1 + Mathf.Min((int)sexMinigame.amountReleased, 200) / 100;
        coomContents += sexMinigame.amountReleased / 100;
        if (coomContents + wombContents > 20f) coomContents = 20f - wombContents;
        coomContents = Mathf.Round(coomContents * 10000) / 10000;
        //Debug.Log(sexMinigame.amountReleased);
        //coomStorage -= sexMinigame.amountReleased / 100;
        //coomStorage = Mathf.Round(coomStorage * 10000) / 10000;
        coomStorage = 0f;
        sexMinigame.amountReleased = 0;
        wombCapacityBar.localScale = new Vector3((5f + 2 * fetusCount) * 0.45f, wombCapacityBar.localScale.y, 1);
        minigameUI.SetActive(false);
        //overrideFace.enabled = false;
        faces.SetCounterTo(BellyToFaceIndex(false));
        sexButton.GetComponent<Collider2D>().enabled = (coomStorage >= 1f && !isAsleep);
        sexButton.GetComponent<SpriteRenderer>().color = ((coomStorage >= 1f && !isAsleep) ? Color.white : new Color(1f, 1f, 1f, 0.2f));
        PrintStats();
    }

    IEnumerator MainRoutine()
    {
        float adjustedStomachCapacity;
        bool ateThisTurn;
        faces.SetCounterTo(BellyToFaceIndex(false));
        PrintStats();
        caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0));
        caloriesText.color = Mathf.Round(dailyCalories) >= (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0)) ? new Color(0.5058824f, 1f, 0.3803922f, 1f) : Color.white;
        timeText.text = currentTime + ":00 | Day " + actualDays;
        hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
        hungerText.text = "Hunger multiplier: " + hungerModifier + "x";
        moneyText.text = "$" + money;
        wombTattoo.GetComponent<SpriteRenderer>().enabled = achievements[4];
        statsView.SetActive(achievements[6]);

        RefreshTouchColliders();
        sexFace.enabled = fetusCount == 0;
        musicPlayer.PlayAtIndex(fetusCount == 0 ? 0 : 1);
        bool eligibleForAchievement = true;
        bool topHeavyAtStart = false;

        while (true)
        {
            if (!isAsleep && Settings.SaveEnabled) SaveGame();
            adjustedStomachCapacity = stomachCapacity * hungerModifier * trainingModifier;
            ateThisTurn = false;
            if (stomachContents > stomachCapacity * trainingModifier)
            {
                trainingModifier += Mathf.Round((stomachContents - (stomachCapacity * trainingModifier)) * (isAsleep ? 20 : 10)) / 1000;
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
                yield return StartCoroutine(SexMinigame(true));
            }
            if (holdFaceDuration <= 0f) faces.SetCounterTo(BellyToFaceIndex(false));
            PrintStats();
            UpdateMedicineText();

            achievementButton.GetComponent<Collider2D>().enabled = !isAsleep;
            pillButton.GetComponent<Collider2D>().enabled = !isAsleep;
            sexButton.GetComponent<Collider2D>().enabled = (coomStorage >= 1f && !isAsleep && !isStreaming);
            sexButton.GetComponent<SpriteRenderer>().color = ((coomStorage >= 1f && !isAsleep && !isStreaming) ? Color.white : new Color(1f, 1f, 1f, 0.2f));
            recordButton.GetComponent<Collider2D>().enabled = !isAsleep && daysUntilNextStream <= 0;
            recordButton.Brighten(daysUntilNextStream <= 0);
            int streamEarnings = 0;
            int foodEaten = 0;
            int maxSize = imageIndex;
            lastJiggledSize = -1;
            bool[] alreadySeenInteractions = new bool[7] {false, false, false, false, false, false, false};
            bool[] eligibleInteractions = new bool[7] {true, true, true, true, true, true, true};
            bool[] alreadyMentionedStreamers = new bool[8] { false, false, false, false, false, false, false, false};
            bool babiesKicking = false;
            float kickTimer = 0f;
            bool interactedWithChat = false;
            

            while (!Input.GetKeyDown(KeyCode.Return) && clickedButtonName != "skip_time_button" && !isAsleep)
            {
                foodButton.SetActive(!isStreaming);
                maxSize = Mathf.Max(imageIndex, maxSize);
                //streamEarnings = (int)(foodEaten * Mathf.Pow(1.5f, deltaSize) + (jiggledDuringStream ? maxSize : 0));
                donationsText.text = "Donations: $" + streamEarnings;
                /*if (Input.GetKeyDown(KeyCode.Delete) && Input.GetKey(KeyCode.Backspace))
                {
                    WipeSaveData();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }*/
                adjustedStomachCapacity = stomachCapacity * hungerModifier * trainingModifier;

                bool fedDuringStream = false;
                if (isStreaming && clickedButtonName == "streaming_food")
                {
                    cursor.GetComponent<SpriteRenderer>().sprite = draggableFood;
                    streamFoodIcon.enabled = false;
                    while (!Input.GetMouseButtonUp(0))
                    {
                        yield return null;
                    }
                    fedDuringStream = cursor.GetColliderName(5) == "mouth";
                    streamFoodIcon.enabled = true;
                    cursor.GetComponent<SpriteRenderer>().sprite = null;
                }

                if (fedDuringStream || (!isStreaming && (Input.GetKeyDown(KeyCode.Space) || clickedButtonName == "food_button")))
                {
                    achievementText.text = "";

                    if (stomachContents < adjustedStomachCapacity && !isNauseous)
                    {
                        babiesKicking = false;
                        kickTimer = 0f;
                        playingDigestionSounds = false;
                        streamDigestionSounds.Mute(true);
                        if (fedDuringStream || alwaysUseEatingAnimation)
                        {
                            achievementButton.GetComponent<Collider2D>().enabled = false;
                            recordButton.GetComponent<Collider2D>().enabled = false;
                            StartCoroutine(Bounce(0.2f));
                            mouthSprite.enabled = true;
                            //overrideFace.enabled = true;
                            faces.SetCounterTo(0);//imageIndex > 3 ? 3 : 0);
                            gulpPlayer.PlayRandom();
                            yield return new WaitForSeconds(0.6f);
                            StartCoroutine(Bounce(0.2f));
                            yield return new WaitForSeconds(0.2f);
                            mouthSprite.enabled = false;
                            //overrideFace.enabled = false;
                            
                            achievementButton.GetComponent<Collider2D>().enabled = true;
                        }
                        faces.SetCounterTo(BellyToFaceIndex(false));
                        stomachContents += 0.4f;
                        if (isStreaming)
                        {
                            if (foodEaten <= plates.Length) plates[foodEaten].SetActive(true);
                            foodEaten++;
                            streamEarnings += (int)(Mathf.Pow(1.19f, imageIndex));
                            chatButton.GetComponent<Collider2D>().enabled = true;
                            chatButton.GetComponent<SpriteRenderer>().color = Color.white;
                        }
                        holdFaceDuration = 0f;
                        gulpPlayer.PlayRandom();
                        if (achievements[1] && stomachContents >= adjustedStomachCapacity)
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
                                //overrideFace.enabled = true;
                                faces.SetCounterTo(0);//imageIndex > 4 ? 3 : 0);
                                gulpPlayer.PlayRandom();
                                yield return new WaitForSeconds(0.6f);
                                StartCoroutine(Bounce(0.2f));
                                yield return new WaitForSeconds(0.2f);
                                mouthSprite.enabled = false;
                                //overrideFace.enabled = false;
                                faces.SetCounterTo(BellyToFaceIndex(false));
                                achievementButton.GetComponent<Collider2D>().enabled = true;
                            }
                            else
                            {
                                yield return new WaitForSeconds(0.3f);
                            }
                            stomachContents += 0.4f;
                            if (isStreaming)
                            {
                                if (foodEaten <= plates.Length) plates[foodEaten].SetActive(true);
                                foodEaten++;
                                streamEarnings += (int)(Mathf.Pow(1.19f, imageIndex));
                            }
                            gulpPlayer.PlayRandom();
                        }
                        recordButton.GetComponent<Collider2D>().enabled = !isStreaming && !isAsleep && daysUntilNextStream <= 0;
                        ateThisTurn = true;
                        if (stomachContents >= stomachCapacity * trainingModifier && hungerModifier > 1)
                        {
                            foodDescription = "You ignore the signals of fullness from your overstuffed belly and continue gorging yourself.";
                        }
                        else if (stomachContents >= stomachCapacity * trainingModifier)
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
                            if (digestiveContents >= 8f)
                            {
                                foodDescription = "You struggle to cram even more food into your gigantic, tightly stretched belly, panting heavily as your overfilled stomach pushes up against your lungs.";

                            }
                            else if (digestiveContents >= 6f)
                            {
                                foodDescription = "You manage to force down another plate, but you are starting to feel overwhelmed by the mountain of food now sitting in your enormously stuffed belly.";

                            }
                            else if (digestiveContents >= 4f)
                            {
                                foodDescription = "Although you feel unimaginably full, you continue eating, motivated purely by the desire to make your belly as huge and round as possible.";

                            }
                            else if (digestiveContents >= 2f)
                            {
                                foodDescription = "Your full belly is starting to round out nicely, but you continue eating at the same pace.";

                            }
                            else
                            {
                                foodDescription = "You greedily devour a plate of food in mere seconds.";
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
                                    commentGenerator.GenerateComment(messageList[index], Random.Range(-1f, 1f));
                                    sentMessages[index] = true;
                                }
                            }                           
                        }
                        UpdateMedicineText();
                        PrintStats();

                        //Debug.Log(messageList[Random.Range(0, messageList.Length)]);

                        //if (stomachContents > adjustedStomachCapacity && hungerModifier <= 1) stomachContents = adjustedStomachCapacity //restrict overstuffing if not unlocked
                    }
                    else if (isNauseous)
                    {
                        foodDescription = "You feel too nauseous to eat anything right now.";
                        faces.SetCounterTo(6);
                        holdFaceDuration = 1.5f;
                    }
                    else if (hungerModifier <= 1f)
                    {
                        foodDescription = "You feel too full to eat another bite.";
                    }
                    else if (stomachContents < 12f)
                    {
                        foodDescription = "Despite your enhanced appetite, the sensation of fullness in your overstuffed belly is too intense and you are unable to eat any more.";
                    }
                    else 
                    {
                        foodDescription = "You try to force down even more food, but your overfilled stomach is unable to stretch any further, and swallowing has become physically impossible.";
                    }
                    //Debug.Log(foodDescription);
                    
                }
                foodText.text = foodDescription;

                if (clickedButtonName == "weed_button")
                {
                    achievementText.text = "";
                    weedStock--;
                    munchiesConsumed++;
                    PrintStats();
                    if (!gurglePlayer.GetComponent<AudioSource>().isPlaying)
                    {
                        gurglePlayer.PlayCustom(hungrySound, Mathf.Max(0.2f, hungerTimer / 20f));
                        holdFaceDuration = hungrySound.length;
                        faces.SetCounterTo((stomachContents + intestineContents == 0) ? 5 : 1);
                    }
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

                jiggledDuringStream = false;
                if (clickedButtonName == this.gameObject.name)
                {
                    StartCoroutine(Bounce(0.3f));
                    yield return StartCoroutine(BellyJiggle(true));
                }
                if (holdFaceDuration > 0f)
                {
                    holdFaceDuration -= Time.deltaTime;
                    if (holdFaceDuration <= 0f)
                    {
                        holdFaceDuration = 0;
                        //overrideFace.enabled = isAsleep;
                        //if (isAsleep) faces.SetCounterTo(0);
                        if (babiesKicking)
                        {
                            faces.SetCounterTo(1);
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
                    yield return StartCoroutine(SexMinigame(false));
                }

                if (clickedButtonName == "chat_button")
                {
                    chatButton.GetComponent<Collider2D>().enabled = false;
                    chatButton.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    interactedWithChat = true;

                    eligibleInteractions[5] = pregnancyDays >= 20;
                    eligibleInteractions[6] = stomachContents > stomachCapacity * trainingModifier * hungerModifier;

                    int interactionIndex = Random.Range(0, 7);
                    while (!eligibleInteractions[interactionIndex]) interactionIndex = Random.Range(0, 7);

                    string subMessage = "";
                    string subMessage2 = "";

                    switch (interactionIndex)
                    {
                        case 0://elbows
                            subMessage = "You touch your elbows together without much difficulty.";
                            if (imageIndex > 6) subMessage = "It's a struggle, but you barely manage to bring your elbows together.";
                            if (imageIndex > 12) subMessage = "You try bringing your elbows together, but it's impossible for obvious reasons.";
                            bellyText.text = "\"can you touch your elbows together?\"\n\n" + (alreadySeenInteractions[0] ? "You're not falling for that again." : (subMessage + " What was that all about?"));
                            if (!alreadySeenInteractions[0])
                            {
                                if (didElbows)
                                {
                                    commentGenerator.GenerateComment((Random.Range(0, 2) == 0 ? "how does she keep falling for this" : "i bet she's doing that on purpose"), Random.Range(-1f, 1f));
                                }
                                else
                                {
                                    commentGenerator.GenerateComment((Random.Range(0, 2) == 0 ? "lmao she fell for it" : "lol, classic elbow trick"), Random.Range(-1f, 1f));
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
                            }
                            else
                            {
                                bellyText.text = "\"does anyone know any other streamers who do similar content\"\n\n" + "You playfully admonish the chatter for thinking about other girls, but mention that you know a girl named " + subMessage + " who does " + subMessage2 + " content.";
                                if (!alreadySeenInteractions[1]) commentGenerator.GenerateComment(subMessage + "? what game is she from", Random.Range(-1f, 1f));
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
                            int amount = (int)Mathf.Ceil(((stomachCapacity * trainingModifier * hungerModifier) - stomachContents) / 0.4f);
                            if (achievements[1]) amount++;
                            if (stomachContents < stomachCapacity * trainingModifier * hungerModifier)
                            {
                                subMessage = "You estimate that you can probably fit " + amount + (foodEaten == 0 ? " " : " more ") + (amount == 1 ? "plate" : "plates") + " of food in your belly." + (amount > 9 ? " Some of your newer viewers don't seem to believe you." : "");
                            }
                            else
                            {
                                subMessage = "You tell them that you are stuffed to the limit, " + ((hungerModifier == 4f && intestineContents >= intestineCapacity * trainingModifier) ? "and it is physically impossible to stuff your overstretched belly any further." : "but you might be able to force yourself to eat more with some encouragement from chat.");
                                if (intestineContents < intestineCapacity * trainingModifier * intestineMultiplier)
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
                            float totalBellyContents = stomachContents + intestineContents + wombContents + coomContents;
                            if (totalBellyContents >= 3f) subMessage = "\"She definitely looks pregnant. Imagine if she was just trolling us with a food baby though\"";
                            if (totalBellyContents >= 7f) subMessage = "\"I thought she was already in her third trimester. is this a trick question?\"";
                            if (totalBellyContents >= 9f) subMessage = "\"She's way too big for a single pregnancy. I think maybe twins?\"";
                            if (totalBellyContents >= 11f) subMessage = "\"she's fucking HUGE!! is she having triplets?\"";
                            if (totalBellyContents >= 13f) subMessage = "\"guys I did the math, based on her size she looks like she's carrying " + (int)((totalBellyContents - 5) / 2) + " babies, full term\"";

                            subMessage2 = "You tell them to look forward to how much bigger you'll be when the stream ends.";
                            if (stomachContents + intestineContents + coomContents > 1f)
                            {
                                subMessage2 = "The rest is all food." + (coomContents >= 0.4f ? " (You decide not to tell them what else is in there.)" : "");
                            }
                            string trimester = "first";
                            if (pregnancyDays > 13) trimester = "second";
                            if (pregnancyDays > 26) trimester = "third";
                            bellyText.text = "You ask some newly-joined viewers to guess how many babies you are carrying right now.\n\n" + subMessage + (alreadySeenInteractions[4] ? ("\n\nYou let the other chatters tell the newcomers about the " + (fetusCount == 1 ? "single baby" : (fetusCount + " babies")) + " in your belly.") : "\n\nAfter a while, you reveal the truth: you are carrying " + fetusCount + (fetusCount == 1 ? " baby" : " babies") + " and are in roughly your " + trimester + " trimester. " + subMessage2);
                            faces.SetCounterTo(1);
                            if (!alreadySeenInteractions[4] && fetusCount > 1)
                            {
                                switch (fetusCount)
                                {
                                    case 2:
                                        subMessage = "twins? whoa";
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
                                commentGenerator.GenerateComment(subMessage, Random.Range(-1f, 1f));
                                //if (fetusCount == 3) commentGenerator.GenerateComment("oh baby a triple!!!", Random.Range(-1f, 1f));
                            }
                            break;
                        case 5://babies kicking
                            if (stomachContents + intestineContents > 2f) subMessage = (stomachContents > stomachCapacity * trainingModifier) ? "overstuffed " : "full ";
                            bellyText.text = "Your " + (fetusCount == 1 ? "baby starts" : "babies start") + " kicking and squirming inside your " + subMessage + "belly. You turn towards the camera to present your viewers with the best possible view.";
                            faces.SetCounterTo(1);
                            //StartCoroutine(BellyJiggle(false));
                            if (lastJiggledSize < imageIndex)
                            {
                                jiggledDuringStream = true;
                                lastJiggledSize = imageIndex;
                                string bellyDescriptor = "swollen ";
                                if (imageIndex > 7) bellyDescriptor = "huge ";
                                if (imageIndex > 13) bellyDescriptor = "enormous ";
                                if (imageIndex > 18) bellyDescriptor = "gigantic ";
                                foodDescription = "A few extra donations roll in as your " + bellyDescriptor + "belly jiggles in front of the camera.";
                                if (!sentMessages[77])
                                {
                                    commentGenerator.GenerateComment("BELLY JIGGLE LET'S FUCKING GOOOOOOO", Random.Range(-1f, 1f));
                                    sentMessages[77] = true;
                                }
                            }
                            babiesKicking = true;
                            if (!achievements[5]) UpdateAchievements(5);
                            break;
                        case 6://dishes
                            subMessage = "Your round tummy presses into the counter, leaving you without much room to work with";
                            if (imageIndex > 7) subMessage = "You have to rest your huge belly on the edge of the sink to make room";
                            if (imageIndex > 13) subMessage = "You have to lean forward with your enormous belly pressed against the front of the counter in order to reach the sink";
                            if (intestineContents < intestineCapacity * intestineMultiplier * trainingModifier)
                            {
                                topHeavyAtStart = stomachContents > intestineContents + wombContents + coomContents;
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
                                PrintStats();
                                if (stomachContents <= (intestineContents + wombContents + coomContents) && topHeavyAtStart && imageIndex > 3)
                                {
                                    gurglePlayer.PlayRandom();
                                    topHeavyAtStart = false;
                                }
                                subMessage2 = "\n\nWhile you wash the dishes, you feel some food flowing into the lower parts of your abdomen.";
                            }
                            bellyText.text = "You are too full to eat another bite, so you decide it's a good time to take a break and wash some dishes. " + subMessage + ", but you manage to make some decent progress." + subMessage2;
                            faces.SetCounterTo(1);
                            break;
                    }

                    alreadySeenInteractions[interactionIndex] = true;
                }

                if (jiggledDuringStream)
                {
                    streamEarnings += 2;
                    jiggledDuringStream = false;
                }

                if (clickedButtonName == "record_button" && !isStreaming)
                {
                    if (isNauseous)
                    {
                        foodDescription = "You feel nauseous. Doing a mukbang stream doesn't seem like a good idea.";
                        faces.SetCounterTo(6);
                        holdFaceDuration = 1.5f;
                    }
                    else
                    {
                        achievementText.text = "";
                        isStreaming = true;
                        recordButton.GetComponent<Collider2D>().enabled = false;
                        sexButton.GetComponent<Collider2D>().enabled = false;
                        sexButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
                        chatButton.GetComponent<Collider2D>().enabled = true;
                        chatButton.GetComponent<SpriteRenderer>().color = Color.white;
                        startingSize = imageIndex;
                        lastJiggledSize = 0;
                        StartCoroutine(musicPlayer.ChangeTrackTo(2, 1.5f));
                        donationsText.text = "Donations: $" + streamEarnings;
                        UpdateMedicineText();
                    }
                }

                if (clickedButtonName == "always_eat_button") alwaysUseEatingAnimation = !alwaysUseEatingAnimation;

                if (clickedButtonName == "pill_button")
                {
                    achievementText.text = "";
                    UpdateMedicineText();
                }

                if (clickedButtonName == "toggle_achievement")
                {
                    PrintAchievementBoard();
                    recordButton.GetComponent<Collider2D>().enabled = false;
                    pillButton.GetComponent<Collider2D>().enabled = false;
                    yield return null;
                    //while (clickedButtonName != "toggle_achievement") yield return null;
                    while (!Input.GetMouseButtonDown(0) || clickedButtonName == "always_eat_button" || clickedButtonName == "toggle_tattoo")
                    {
                        yield return null;
                        if (clickedButtonName == "always_eat_button") alwaysUseEatingAnimation = alwaysEatButton.isActive;
                        if (clickedButtonName == "delete_save_button")
                        {
                            GameObject.Find("delete_save_text").GetComponent<Text>().text = "Press again to confirm";
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
                                GameObject.Find("delete_save_text").GetComponent<Text>().text = "Delete save data";
                            }
                        }
                        if (clickedButtonName == "restart_button")
                        {
                            LoadBlankSaveData();
                            Settings.SaveEnabled = false;
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        }
                    }
                    recordButton.GetComponent<Collider2D>().enabled = !isAsleep && daysUntilNextStream <= 0;
                    pillButton.GetComponent<Collider2D>().enabled = !isAsleep;
                    clickedButtonName = "toggle_achievement";
                    //achievementButton.GetComponent<ToggleButton>().ForceState(false);
                }

                if (clickedButtonName == "relaxant" && intestineMultiplier < 2f)
                {
                    intestineMultiplier += 0.1f;
                    money -= medicinePrices[0];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "cervical_plug" && !usedPlug)
                {
                    usedPlug = true;
                    money -= medicinePrices[6];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "surrogate_egg" && fetusCount < maxFetusCount)
                {
                    fetusCount++;
                    money -= medicinePrices[7];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "enzyme" && stomachContents > 0f && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier))
                {
                    topHeavyAtStart = stomachContents > intestineContents + wombContents + coomContents;
                    /*if (stomachContents >= stomachCapacity * trainingModifier)
                    {
                        hungerTimer = 0;
                        hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
                        hungerText.text = "Hunger multiplier: " + hungerModifier + "x";
                    }*/
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
                    money -= medicinePrices[1];
                    PrintStats();
                    if (stomachContents <= (intestineContents + wombContents + coomContents) && topHeavyAtStart && imageIndex > 4)
                    {
                        gurglePlayer.PlayRandom();
                        topHeavyAtStart = false;
                    }
                    UpdateMedicineText();
                }

                if (clickedButtonName == "laxative" && intestineContents > 0f)
                {
                    dailyCalories += Mathf.Round(intestineContents * 500);
                    if (!achievements[2] && (int)dailyCalories >= 8000) UpdateAchievements(2);
                    caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0));
                    caloriesText.color = Mathf.Round(dailyCalories) >= (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0)) ? new Color(0.5058824f, 1f, 0.3803922f, 1f) : Color.white;
                    intestineContents = 0f;
                    disposalTimer = 0;
                    money -= medicinePrices[2];
                    tookLaxative = true;
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "folate" && wombContents < (fetusCount + 2.5) * 0.05f * pregnancyDays)
                {
                    wombContents += (fetusCount + 2.5f) * 0.04f;
                    wombContents = Mathf.Round(wombContents * 10000) / 10000;
                    if (coomContents + wombContents > 20f) coomContents = 20f - wombContents;
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
                    money -= medicinePrices[5];
                    UpdateMedicineText();
                }

                if (babiesKicking && kickTimer <= 0f)
                {
                    StartCoroutine(BellyJiggle(false));
                    faces.SetCounterTo(1);
                    kickTimer = Random.Range(2.5f, 5f);
                }
                if (babiesKicking && kickTimer > 0f) kickTimer -= Time.deltaTime;

                yield return null;
            }
            if (!isAsleep) achievementText.text = "";
            playingDigestionSounds = false;
            streamDigestionSounds.Mute(true);
            bool canceledStream = false;
            if (isStreaming)
            {
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
                //Debug.Log(cumulativeEarnings);
                if (!achievements[9] && cumulativeEarnings >= 10000) UpdateAchievements(9);
                moneyText.text = "$" + money;
                StartCoroutine(musicPlayer.ChangeTrackTo(1, 1.5f));
                if (!canceledStream) daysUntilNextStream = achievements[9] ? 1 : 2;
            }
            recordButton.ForceState(false);
            isStreaming = false;
            jiggledDuringStream = false;
            recordButton.GetComponent<Collider2D>().enabled = false;

            pillButton.ForceState(false);
            achievementButton.GetComponent<Collider2D>().enabled = false;
            pillButton.GetComponent<Collider2D>().enabled = false;

            foodDescription = "";
            foodText.text = foodDescription;
            //overrideFace.enabled = isAsleep;
            //end of hour
            topHeavyAtStart = stomachContents > intestineContents + wombContents + coomContents;

            if (!ateThisTurn && (hungerModifier - 0.2f * munchiesConsumed) <= 1f) foodDescription = "";
            if (stomachContents >= adjustedStomachCapacity && !isAsleep && (hungerModifier - 0.2f * munchiesConsumed) > 1.01f)
            {
                foodDescription = "The adrenaline from your feeding frenzy wears off, and the intense sensation of fullness hits you all at once.";
            }
            else if (canceledStream)
            {
                foodDescription = "You quickly shut off your stream before anyone notices that you were live.";
            }
            else if (isNauseous && currentTime == 11)
            {
                foodDescription = "Your nausea wears off and you feel like you can eat again.";
                isNauseous = false;
            }
            else 
            {
                foodDescription = "";
            }

            if (foodDescription == "" && fetusCount > 0 && pregnancyDays >= 20 && Random.Range(0, 55 - pregnancyDays) == 1)
            {
                if (stomachContents + intestineContents > 2.5f)
                {
                    switch (fetusCount)
                    {
                        case 1:
                            foodDescription = "You feel the baby squirming in protest within your cramped, overstuffed tummy.";
                            break;
                        case 2:
                            foodDescription = "You feel the twins squirming in protest within your cramped, overstuffed tummy.";
                            break;
                        case 3:
                            foodDescription = "You feel the triplets squirming in protest within your cramped, overstuffed tummy.";
                            break;
                        case 4:
                            foodDescription = "You feel the quadruplets squirming in protest within your cramped, overstuffed tummy.";
                            break;
                        default:
                            foodDescription = "You feel a lot of babies squirming in protest within your cramped, overstuffed tummy.";
                            break;
                    }
                }
                else
                {
                    switch (fetusCount)
                    {
                        case 1:
                            foodDescription = "You feel the baby shifting slightly inside your tummy.";
                            break;
                        case 2:
                            foodDescription = "You feel the twins shifting slightly inside your tummy.";
                            break;
                        case 3:
                            foodDescription = "You feel the triplets shifting slightly inside your tummy.";
                            break;
                        case 4:
                            foodDescription = "You feel the quadruplets shifting slightly inside your tummy.";
                            break;
                        default:
                            foodDescription = "You feel a lot of babies shifting slightly inside your tummy.";
                            break;
                    }
                }

                if (!achievements[5]) UpdateAchievements(5);

                foodText.text = foodDescription;
                yield return StartCoroutine(BellyJiggle(true));
            }

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
                if (hungerTimer < 10 && intestineContents <= 0f) hungerTimer++;

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
            if (coomContents + wombContents > 20f) coomContents = 20f - wombContents;
            coomContents = Mathf.Round(coomContents * 10000) / 10000;
            if (coomStorage < 3f)
            {
                coomStorage += 0.05f;
                if (coomStorage > 3f) coomStorage = 3f;
            }


            if (flowEnabled)
            {
                if (stomachContents >= flowRate && (intestineContents + flowRate) <= intestineCapacity * intestineMultiplier * trainingModifier)
                {
                    stomachContents -= flowRate;
                    intestineContents += flowRate;
                }
                else if (stomachContents >= intestineCapacity * intestineMultiplier * trainingModifier - intestineContents && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier) && (intestineContents + flowRate) > intestineCapacity * intestineMultiplier * trainingModifier)
                {
                    stomachContents -= ((intestineCapacity * intestineMultiplier * trainingModifier) - intestineContents);
                    intestineContents = intestineCapacity * intestineMultiplier * trainingModifier;
                    flowEnabled = false;
                }
                else if (stomachContents < flowRate)
                {
                    intestineContents += stomachContents;
                    stomachContents = 0f;
                }
                else
                {
                    Debug.Log("this should only happen if both stomach and intestines are stuffed, congratulations");
                    flowEnabled = false;
                }
            }
            if (intestineContents >= intestineCapacity * intestineMultiplier * trainingModifier) flowEnabled = false;

            if (intestineContents > 0)
            {
                if (!flowEnabled)
                {
                    disposalTimer++;
                }
                if (disposalTimer > disposalReq && (!isAsleep || sleepCountdown == 1))
                {
                    dailyCalories += Mathf.Round(intestineContents * 1000);
                    if (!achievements[2] && (int)dailyCalories >= 8000) UpdateAchievements(2);
                    caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0));
                    caloriesText.color = Mathf.Round(dailyCalories) >= (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0)) ? new Color(0.5058824f, 1f, 0.3803922f, 1f) : Color.white;
                    intestineContents = 0f;
                    disposalTimer = 0;
                }
            }

            currentTime++;
            if (sleepCountdown > 0) sleepCountdown--;
            if (currentTime > 23)
            {
                currentTime = 0;
                actualDays++;
            }
            if (currentTime == 0 + (tookCaffeine ? 2 : 0))
            { 
                sleepCountdown = 8 - (tookCaffeine ? 2 : 0);
                holdFaceDuration = 0f;
                isAsleep = true;
                tookCaffeine = false;
                if (!achievements[1] && eligibleForAchievement) UpdateAchievements(1);
            }

            sexButton.GetComponent<Collider2D>().enabled = (coomStorage >= 1f && !isAsleep);
            sexButton.GetComponent<SpriteRenderer>().color = ((coomStorage >= 1f && !isAsleep) ? Color.white : new Color(1f, 1f, 1f, 0.2f));


            if (currentTime == 4)
            {
                munchiesConsumed = 0;
                tookLaxative = false;
                if (weedStock < 5)
                {
                    weedStock++;
                    if (achievements[0] && weedStock < 5) weedStock++;
                }
                pregnancyDays++;
                if (pregnancyDays > 4 && pregnancyDays < 16 && Random.Range(0, Mathf.Max(0, 9 - fetusCount)) == 0) isNauseous = true;
                if (daysUntilNextStream > 0) daysUntilNextStream--;
                //Debug.Log(pregnancyDays);
                if (pregnancyDays < 20)
                {
                    wombContents += (2.5f + fetusCount) * ((dailyCalories >= 2000) ? 0.05f : 0.01f);
                    if (coomContents + wombContents > 20f) coomContents = 20f - wombContents;
                    coomContents = Mathf.Round(coomContents * 10000) / 10000;
                }
                else if (pregnancyDays <= 40) 
                {
                    wombContents += (2.5f + fetusCount) * ((dailyCalories >= 2000 + fetusCount * 500) ? 0.05f : 0.01f); //Mathf.Clamp(0.025f * ((dailyCalories - 2000) / fetusCount) / 400, 0f, 0.025f));
                    if (coomContents + wombContents > 20f) coomContents = 20f - wombContents;
                    coomContents = Mathf.Round(coomContents * 10000) / 10000;
                }
                else
                {
                    StartCoroutine(musicPlayer.GraduallyMute(2f));
                    stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier * 0.45f, stomachCapacityBar.localScale.y, 1);
                    intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier * 0.45f, intestineCapacityBar.localScale.y, 1);
                    yield return StartCoroutine(AnimateBirth());
                    wombContents = 0;
                    pregnancyDays = 0;
                    actualDays = 0;
                    wombCapacityBar.localScale = new Vector3((5f + 2 * fetusCount) * 0.45f, wombCapacityBar.localScale.y, 1);
                }
                //if (dailyCalories >= 2000 + fetusCount * 800) wombContents += fetusCount * 0.025f;
                //Mathf.Clamp(wombContents, 0f, fetusCount * pregnancyDays * 0.05f);

                bankedCalories += (int)dailyCalories - (2000 + 500 * (pregnancyDays >= 20 ? 1 : 0));
                Mathf.Clamp(bankedCalories, 0, 10000);
                //Debug.Log("banked calories: " + bankedCalories);
                dailyCalories = 0;
                caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0));
                caloriesText.color = Mathf.Round(dailyCalories) >= (2000 + fetusCount * (pregnancyDays >= 20 ? 500 : 0)) ? new Color(0.5058824f, 1f, 0.3803922f, 1f) : Color.white;

            }

            if (isAsleep && sleepCountdown == 0)
            {
                isAsleep = false;
                if (!achievements[6] && stomachContents > stomachCapacity * trainingModifier) UpdateAchievements(6);
            }
            achievementButton.GetComponent<Collider2D>().enabled = !isAsleep;
            if (holdFaceDuration <= 0f && isAsleep)
            {
                //overrideFace.enabled = isAsleep;
                faces.SetCounterTo(0);
            }
            timeText.text = currentTime + ":00 | Day " + actualDays;
            hungerText.text = "Hunger multiplier: " + hungerModifier + "x";

            //stomachContents = Mathf.Round(stomachContents * 1000) / 1000;
            //intestineContents = Mathf.Round(intestineContents * 1000) / 1000;
            //wombContents = Mathf.Round(wombContents * 1000) / 1000;
            //trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;
            if (stomachContents <= (intestineContents + wombContents + coomContents) && topHeavyAtStart && imageIndex > 4) gurglePlayer.PlayRandom();
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
                            bellyDescription += ".";
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
                            bellyDescription += ", and all you can think about is food. You feel like you could easily eat a meal for two and have plenty of room left.";
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
                bellyDescription = "You can feel your immensely stuffed belly stretching to accomodate the tremendous amount of food packed inside of it.";
                break;
            case 8:
            case 9:
                bellyDescription = "Your hugely swollen belly is so thoroughly stuffed that it feels like a boulder. " + (imageIndex < 13 ? "Your arms can barely reach your belly button." : "Your arms can no longer reach your belly button.");
                break;
            case 10:
            case 11:
                bellyDescription = "The equivalent of an entire family-sized Thanksgiving feast now sits inside your massively engorged belly. You are so full that it is getting difficult to breathe.";
                break;
            case 12:
            case 13:
                bellyDescription = "The incredible amount of food in your belly " + (wombContents <= 7f ? "has expanded it beyond the size of a full-term pregnancy." : "feels as heavy as the weight of the " + IntToNumberofBabies(fetusCount) + " resting inside your distended womb.") + " The overwhelming sensation of fullness threatens to shut down your brain.";
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
                bellyDescription = "Every inch of your digestive tract is stretched like a balloon and your belly feels as huge as it can possibly get. You cannot even burp, as a burp would imply the existence of a pocket of air trapped somewhere in your enormously distended abdomen, and every last bit of space relieved by previous burps has already been crammed with even more food. By volume, you are almost more food than girl at this point and you feel like you might burst if you force down one more bite.";
                if (!achievements[3]) UpdateAchievements(3);
                break;

        }
        imageIndex = (int) Mathf.Min(characterSpritesBtm.Length - 1, Mathf.Floor(totalBellyContents));
        if (!achievements[8] && imageIndex == characterSpritesBtm.Length - 1) UpdateAchievements(8);
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
            wombSprites.SetCounterTo(Mathf.Min(maxFetusCount, fetusCount));
        }
        RefreshTouchColliders();
        UpdateWombTattoo(stomachContents > intestineContents + wombContents + coomContents);
        spriteRenderer.sprite = (stomachContents > intestineContents + wombContents + coomContents ? characterSpritesTop[imageIndex] : characterSpritesBtm[imageIndex]);
    
        //Debug.Log("Total belly contents: " + (stomachContents + intestineContents) + " | " + bellyDescription);
        bellyText.text = bellyDescription;

        stomachContents = Mathf.Round(stomachContents * 10000) / 10000;
        intestineContents = Mathf.Round(intestineContents * 10000) / 10000;
        coomContents = Mathf.Round(coomContents * 10000) / 10000;
        wombContents = Mathf.Round(wombContents * 10000) / 10000;
        trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;

        coomStorageBar.localScale = new Vector3(coomStorage / 3f, coomStorageBar.localScale.y, 1);
        stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier * 0.45f, stomachCapacityBar.localScale.y, 1);
        stomachContentsBar.localScale = new Vector3(stomachContents * 0.45f, stomachContentsBar.localScale.y, 1);
        contentsText[0].text = Mathf.Round(stomachContents * 100) / 100 + "L / " + Mathf.Round(stomachCapacity * trainingModifier * 100) / 100 + "L";
        intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier * 0.45f, intestineCapacityBar.localScale.y, 1);
        intestineContentsBar.localScale = new Vector3(intestineContents * 0.45f, intestineContentsBar.localScale.y, 1);
        contentsText[1].text = Mathf.Round(intestineContents * 100) / 100 + "L / " + Mathf.Round(intestineCapacity * intestineMultiplier * trainingModifier * 100) / 100 + "L";
        wombContentsBar.localScale = new Vector3(wombContents * 0.45f, wombContentsBar.localScale.y, 1);
        coomContentsBar.localScale = new Vector3((coomContents + wombContents) * 0.45f, coomContentsBar.localScale.y, 1);
        wombCapacityBar.localScale = new Vector3((5f + 2 * fetusCount) * 0.45f, wombCapacityBar.localScale.y, 1);
        contentsText[2].text = Mathf.Round((wombContents + coomContents) * 100) / 100 + "L / " + Mathf.Round((2 * fetusCount + 5) * 100) / 100 + "L";

        //Debug.Log("Time: " + currentTime + ":00 | stomach: " + (Mathf.Round(stomachContents * 1000) / 1000) + "L / " + (stomachCapacity * trainingModifier) + "L | intestines: " + (Mathf.Round(intestineContents * 1000) / 1000) + "L / " + (intestineCapacity * intestineMultiplier * trainingModifier) + "L | hunger modifier = " + hungerModifier);

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
