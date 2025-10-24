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

    [SerializeField] private Text achievementBoard;
    [SerializeField] private Text achievementRewards;

    [SerializeField] private AudioPlayer musicPlayer;
    [SerializeField] private AudioPlayer stuffedMoansPlayer;
    [SerializeField] private AudioPlayer gulpPlayer;
    [SerializeField] private AudioPlayer gurglePlayer;
    [SerializeField] private AudioClip hungrySound;
    [SerializeField] private AudioClip lastBirthSound;
    [SerializeField] private AudioClip hiccupSound;

    [SerializeField] private AudioPlayer achievementPlayer;
    [SerializeField] private AudioPlayer gaspPlayer;

    [SerializeField] private SpriteRenderer overrideFace;
    [SerializeField] private SpriteRenderer sexFace;
    [SerializeField] private SpriteRenderer roomBG;
    [SerializeField] private SpriteRenderer streamFoodIcon;
    [SerializeField] private SpriteRenderer mouthSprite;

    [SerializeField] private GameObject minigameUI;
    [SerializeField] private TimedSlider sexMinigame;
    [SerializeField] private Cursor cursor;
    [SerializeField] private Sprite draggableFood;

    [SerializeField] private Transform stomachCapacityBar;
    [SerializeField] private Transform stomachContentsBar;
    [SerializeField] private Transform intestineCapacityBar;
    [SerializeField] private Transform intestineContentsBar;
    [SerializeField] private Transform wombContentsBar;
    [SerializeField] private Transform wombCapacityBar;

    [SerializeField] private GameObject weedButton;
    [SerializeField] private GameObject foodButton;
    [SerializeField] private ToggleButton alwaysEatButton;
    [SerializeField] private ToggleButton achievementButton;
    [SerializeField] private ToggleButton pillButton;
    [SerializeField] private ToggleButton recordButton;
    [SerializeField] private GameObject[] touchColliders;
    [SerializeField] private DigitCounter faces;
    [SerializeField] private DigitCounter weedStockCounter;
    [SerializeField] private DigitCounter wombTattoo;

    public bool[] achievements = new bool[11];
    [SerializeField] private GameObject[] medicineButtons = new GameObject[7];
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
     * theoretical max: (2*2)+4+6 = 18L
     * day end 0:00 day start 8:00
     * 0/232/426/735/1122/1548/1974/2632/3252/3677/4000/3574/4000
     * -1/0, 0/1 : -2/0, -1/1
     * 
     * achievements:
     * 
     * 
     * BUG: under certain conditions, weed leaf decreases hunger multiplier
     * training capacity doesn't visually update until after eating
     * belly description sometimes changes between 6/default when it shouldn't
     * sometimes stomach contents overflow intestines
     * 
     * for implementation: slightly randomize food intake?
     * calories digested use rolling animation
     * overrideFace active at all times
     * 
     * */
    float holdFaceDuration = 0f;

    float stomachCapacity = 1.0f;
    public float stomachContents = 0f; //save
    public int digestionTimer = 0; //save
    public int hungerTimer = 0; //save

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
    public int pregnancyDays = 0; //save
    public int actualDays = 0; //save
    string foodDescription = "";

    public int currentTime = 8; //save
    public int sleepCountdown = 0; //save
    public bool isAsleep = false; //save
    bool isBouncing = false;
    public bool tookCaffeine = false; //save
    public bool isStreaming = false; //save
    public bool alwaysUseEatingAnimation = false;
    public bool jiggledDuringStream = false;
    public int daysUntilNextStream = 0;

    int imageIndex = 0;
    int startingSize = 0;
    int deltaSize = 0;
    [SerializeField] Sprite[] characterSpritesBtm = new Sprite[14];
    [SerializeField] Sprite[] characterSpritesTop = new Sprite[14];

    public void SaveGame()
    {
        SaveData saveData = new SaveData();

        saveData.stomachContents = stomachContents;
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
        saveData.isStreaming = isStreaming;
        saveData.jiggledDuringStream = jiggledDuringStream;
        saveData.daysUntilNextStream = daysUntilNextStream;
        saveData.sleepCountdown = sleepCountdown;
        saveData.isAsleep = isAsleep;
        saveData.achievements = achievements;
        saveData.alwaysUseEatingAnimation = alwaysUseEatingAnimation;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savedGame.json", json);
        //Debug.Log(Application.persistentDataPath);
        //Debug.Log(json);
    }

    public void WipeSaveData()
    {
        SaveData saveData = new SaveData();

        saveData.stomachContents = 0f;
        saveData.intestineContents = 0f;
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
        saveData.isStreaming = false;
        saveData.jiggledDuringStream = false;
        saveData.daysUntilNextStream = 0;
        saveData.sleepCountdown = 0;
        saveData.isAsleep = false;
        saveData.achievements = new bool[11];
        saveData.alwaysUseEatingAnimation = false;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savedGame.json", json);
    }

    IEnumerator Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //weedStockCounter = weedButton.transform.Find("counter").GetComponent<DigitCounter>();
        //fetusCount = 0;
        //if (Random.Range(0, 10) > 7) fetusCount++;
        if (File.Exists(Application.persistentDataPath + "/savedGame.json"))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(Application.persistentDataPath + "/savedGame.json"), this);
        }
        yield return null;
        alwaysEatButton.ForceState(alwaysUseEatingAnimation);
        PrintAchievementBoard();
        StartCoroutine(MainRoutine());

    }

    int BellyToFaceIndex()
    {
        int reactionFaceIndex;
        switch (imageIndex)
        {
            case 0:
            case 1:
                reactionFaceIndex = 0;
                break;
            case 2:
            case 3:
            case 4:
                reactionFaceIndex = 2;
                break;
            case 5:
            case 6:
                reactionFaceIndex = 3;
                break;
            default:
                reactionFaceIndex = 4;
                break;

        }
        if (isAsleep) reactionFaceIndex = 0;
        return reactionFaceIndex;
    }

    public IEnumerator BellyJiggle(bool useDefaultBehavior)
    {
        //bool initialState = overrideFace.enabled;
        bool initialButtonState = achievementButton.GetComponent<Collider2D>().enabled;

        achievementButton.GetComponent<Collider2D>().enabled = false;
        recordButton.GetComponent<Collider2D>().enabled = false;

        if (useDefaultBehavior)
        {
            overrideFace.enabled = true;

            if (imageIndex > 4)
            {
                if (!isAsleep) stuffedMoansPlayer.PlayRandom();
                if (stomachContents + intestineContents > 1.5f && Random.Range(0, Mathf.Max(0, 10 - imageIndex)) == 0) gurglePlayer.PlayRandom();
            }


            faces.SetCounterTo(BellyToFaceIndex());
        }


        float baseJiggleRate = 0.1f;
        baseJiggleRate *= 1 + ((float)imageIndex / 20);

        if (stomachContents > intestineContents)
        {
            spriteRenderer.sprite = characterSpritesBtm[imageIndex];
            UpdateWombTattoo(false);
            yield return new WaitForSeconds(baseJiggleRate * 1);
            spriteRenderer.sprite = characterSpritesTop[imageIndex];
            UpdateWombTattoo(true);
            yield return new WaitForSeconds(baseJiggleRate * 2);
            if (imageIndex >= 4)
            {
                if (isStreaming && !jiggledDuringStream)
                {
                    jiggledDuringStream = true;
                    string bellyDescriptor = "swollen ";
                    if (imageIndex > 5) bellyDescriptor = "huge ";
                    if (imageIndex > 7) bellyDescriptor = "enormous ";
                    if (imageIndex > 9) bellyDescriptor = "gigantic ";
                    foodDescription = "A wave of donations rolls in as your " + bellyDescriptor + "belly jiggles in front of the camera.";                   
                }
                spriteRenderer.sprite = characterSpritesBtm[imageIndex];
                UpdateWombTattoo(false);
                yield return new WaitForSeconds(baseJiggleRate * 1);
                spriteRenderer.sprite = characterSpritesTop[imageIndex];
                UpdateWombTattoo(true);
            }

        }
        else
        {
            spriteRenderer.sprite = characterSpritesTop[imageIndex];
            UpdateWombTattoo(true);
            yield return new WaitForSeconds(baseJiggleRate * 1);
            spriteRenderer.sprite = characterSpritesBtm[imageIndex];
            UpdateWombTattoo(false);
            yield return new WaitForSeconds(baseJiggleRate * 1);
            if (imageIndex >= 4)
            {
                if (isStreaming && !jiggledDuringStream)
                {
                    jiggledDuringStream = true;
                    string bellyDescriptor = "swollen ";
                    if (imageIndex > 5) bellyDescriptor = "huge ";
                    if (imageIndex > 7) bellyDescriptor = "enormous ";
                    if (imageIndex > 9) bellyDescriptor = "gigantic ";
                    foodDescription = "A flood of donations rolls in as your " + bellyDescriptor + "belly jiggles in front of the camera.";
                }
                spriteRenderer.sprite = characterSpritesTop[imageIndex];
                UpdateWombTattoo(true);
                yield return new WaitForSeconds(baseJiggleRate * 2);
                spriteRenderer.sprite = characterSpritesBtm[imageIndex];
                UpdateWombTattoo(false);
            }
        }
        recordButton.GetComponent<Collider2D>().enabled = !isStreaming && !isAsleep && daysUntilNextStream <= 0;

        if (!isAsleep && stomachContents + intestineContents > 2f && Random.Range(0, Mathf.Max(3, 10 - imageIndex)) == 0)
        {
            StartCoroutine(gaspPlayer.PlayCustomWaitFor(hiccupSound, stuffedMoansPlayer.GetComponent<AudioSource>()));
            StartCoroutine(ChangeFaceDuringClip(stuffedMoansPlayer, 0.2f, 0.08f, 6));
        }
        achievementButton.GetComponent<Collider2D>().enabled = initialButtonState;

        holdFaceDuration = 0.6f;
    }

    IEnumerator ChangeFaceDuringClip(AudioPlayer player, float duration, float delay, int index)
    {
        while (player.GetComponent<AudioSource>().isPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(delay);
        faces.SetCounterTo(index);
        overrideFace.enabled = true;
        holdFaceDuration += duration;
        yield return new WaitForSeconds(duration);
        faces.SetCounterTo(BellyToFaceIndex());
    }

    public void UpdateMedicineText()
    {
        medicineButtons[0].SetActive(intestineMultiplier < 2f);
        medicineButtons[1].SetActive(stomachContents > 0f && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier));
        medicineButtons[2].SetActive(intestineContents > 0f && !isStreaming);
        medicineButtons[3].SetActive(Mathf.Round(wombContents * 100) / 100 < Mathf.Round(fetusCount * 5f * pregnancyDays) / 100);
        medicineButtons[4].SetActive(hungerTimer < 10);
        medicineButtons[5].SetActive(!tookCaffeine);
        medicineButtons[6].SetActive(achievements[10] && pregnancyDays == 0 && fetusCount < 6);

        string updatedText = "";
        updatedText += "Relaxant: +0.1 intestine capacity (max 2.0)\n\n";
        updatedText += "Enzyme: digest some food in stomach\n\n";
        updatedText += "Laxative: " + (isStreaming ? "don't take laxatives while streaming\n\n" : "digest all food in intestines\n\n");
        updatedText += "Folate: restore 1 day of missed growth\n\n";
        updatedText += "Ghrelin: +1 hr of natural hunger\n\n";
        updatedText += "Caffeine: sleep at 2:00\n\n";
        if (achievements[10]) updatedText += "Egg implant: +1 fetus (Day 0 only, max 6)";

        medicineText.text = updatedText;
        updatedText = "";

        for (int i = 0; i < medicinePrices.Length; i++)
        {
            if (i != 6 || achievements[10]) updatedText += "$" + medicinePrices[i] + "\n\n";
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
                achievementMessage = "Miss Piggy: Digest 6000 calories in one day.";
                rewardMessage = "Reward: digestion starts 1 hour sooner";
                break;
            case 3:
                achievementMessage = "Mucho Texto: Have a total of 7L or more in your stomach and intestines.";
                rewardMessage = "Reward: intestines fill up 33% quicker";
                flowRate = 0.4f;
                break;
            case 4:
                achievementMessage = "Take That, Triple Finish: Get filled with the maximum amount of batter.";
                rewardMessage = "Reward: womb tattoo";
                wombTattoo.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case 5:
                achievementMessage = "Opening Kickoff: Experience your first fetal movement.";
                break;
            case 6:
                achievementMessage = "The Morning After: Wake up with your stomach still overfilled from last night.";
                break;
            case 7:
                achievementMessage = "Elastigirl: Reach the highest possible stretching multiplier.";
                break;
            case 8:
                achievementMessage = "That's No Moon: Reach the largest possible belly size.";
                break;
            case 9:
                achievementMessage = "Queen of Kebabs: Earn a total of $2000 through streaming.";
                rewardMessage = "Reward: stream every day";
                break;
            case 10:
                achievementMessage = "Seed of Destiny: Deliver a perfectly healthy baby.";
                rewardMessage = "Reward: egg implants unlocked";
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
                    achievementDescription = "Digest 6000 calories in one day.";
                    rewardMessage = "-1 digestion delay";
                    break;
                case 3:
                    achievementName = "Mucho Texto";
                    achievementDescription = "Have a total of 7L or more in your stomach and intestines.";
                    rewardMessage = "+33% flow rate";
                    break;
                case 4:
                    achievementName = "Take That, Triple Finish";
                    achievementDescription = "Get filled with the maximum amount of batter.";
                    rewardMessage = "womb tattoo";
                    break;
                case 5:
                    achievementName = "Opening Kickoff";
                    achievementDescription = "Experience your first fetal movement.";
                    rewardMessage = "--";
                    break;
                case 6:
                    achievementName = "The Morning After";
                    achievementDescription = "Wake up with your stomach still overfilled from last night.";
                    rewardMessage = "--";
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
                    achievementDescription = "Earn a total of $2000 through streaming.";
                    rewardMessage = "stream every day";
                    break;
                case 10:
                    achievementName = "Seed of Destiny";
                    achievementDescription = "Give birth to a perfectly healthy baby.";
                    rewardMessage = "egg implants unlocked";
                    break;
                default:
                    achievementName = "Unhandled Exception Guy";
                    achievementDescription = "Attempt to access an array index that is out of bounds.";
                    rewardMessage = "--";
                    break;
            }
            finalString += achievementName + ": " + (achievements[i] ? achievementDescription : "??????") + "\n\n";
            rewardString += "\n" + (achievements[i] ? rewardMessage : "") + "\n";
        }
        achievementBoard.text = finalString;
        achievementRewards.text = rewardString;  
    }

    void UpdateWombTattoo()
    {
        wombTattoo.SetCounterTo(imageIndex + (stomachContents > intestineContents ? 0 : 14));
    }

    void UpdateWombTattoo(bool isTopHeavy)
    {
        wombTattoo.SetCounterTo(imageIndex + (isTopHeavy ? 0 : 14));
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
        float babyVolume = wombContents / fetusCount;

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
            wombContents -= babyVolume;
            foodText.text = "You give birth to a " + babyDescriptor + "baby.";
            PrintStats();
        }
        yield return new WaitForSeconds(1f);
        faces.SetCounterTo(0);
    }

    IEnumerator MainRoutine()
    {
        float adjustedStomachCapacity;
        bool ateThisTurn;
        PrintStats();
        caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 400 : 0));
        timeText.text = currentTime + ":00 | Day " + actualDays;
        hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
        hungerText.text = "Hunger multiplier: " + hungerModifier + "x";
        moneyText.text = "$" + money;
        wombTattoo.GetComponent<SpriteRenderer>().enabled = achievements[4];

        RefreshTouchColliders();
        sexFace.enabled = fetusCount == 0;
        musicPlayer.PlayAtIndex(fetusCount == 0 ? 0 : 1);
        bool eligibleForAchievement = true;
        bool topHeavyAtStart = false;

        while (true)
        {
            if (!isAsleep) SaveGame();
            adjustedStomachCapacity = stomachCapacity * hungerModifier * trainingModifier;
            ateThisTurn = false;
            if (stomachContents > stomachCapacity * trainingModifier)
            {
                trainingModifier += Mathf.Round((stomachContents - (stomachCapacity * trainingModifier)) * (isAsleep ? 30 : 10)) / 1000;
                //if (imageIndex > 3) gurglePlayer.PlayRandom();
                if (trainingModifier >= 2f)
                {
                    trainingModifier = 2f;
                    if (!achievements[7]) UpdateAchievements(7);
                }
            }
            if (hungerTimer > 7 && trainingModifier > 1f)
            {
                trainingModifier -= 0.05f;
                if (trainingModifier < 1f) trainingModifier = 1f;
                trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;
            }

            


            PrintStats();
            //stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier, stomachCapacityBar.localScale.y, 1);
            //intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier, intestineCapacityBar.localScale.y, 1);

            if (!achievements[1] && currentTime == 8) eligibleForAchievement = true;

            if (!isAsleep && currentTime > 8 && stomachContents <= stomachCapacity * trainingModifier) eligibleForAchievement = false;

            if (currentTime == 4)
            {
                munchiesConsumed = 0;
                hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
            }

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
                minigameUI.SetActive(true);
                StartCoroutine(musicPlayer.ChangeTrackTo(0, 3f));
                achievementButton.GetComponent<Collider2D>().enabled = false;
                achievementButton.Brighten(false);
                yield return StartCoroutine(sexMinigame.Oscillate(imageIndex));
                achievementButton.Brighten(true);
                achievementButton.GetComponent<Collider2D>().enabled = true;
                StartCoroutine(musicPlayer.ChangeTrackTo(1, 3f));
                fetusCount = 1 + Mathf.Min((int)sexMinigame.amountReleased, 200) / 100;
                //Debug.Log(sexMinigame.amountReleased);
                sexMinigame.amountReleased = 0;
                wombCapacityBar.localScale = new Vector3(fetusCount * 2f, wombCapacityBar.localScale.y, 1);
                minigameUI.SetActive(false);
                overrideFace.enabled = false;
            }
            UpdateMedicineText();

            achievementButton.GetComponent<Collider2D>().enabled = !isAsleep;
            pillButton.GetComponent<Collider2D>().enabled = !isAsleep;
            recordButton.GetComponent<Collider2D>().enabled = !isAsleep && daysUntilNextStream <= 0;
            recordButton.Brighten(daysUntilNextStream <= 0);
            int streamEarnings = 0;
            int foodEaten = 0;
            int maxSize = imageIndex;
            deltaSize = 0;
            while (!Input.GetKeyDown(KeyCode.Return) && clickedButtonName != "skip_time_button" && !isAsleep)
            {
                foodButton.SetActive(!isStreaming);
                maxSize = Mathf.Max(imageIndex, maxSize);
                streamEarnings = (int)(foodEaten * Mathf.Pow(1.5f, deltaSize) + (jiggledDuringStream ? maxSize : 0));
                donationsText.text = "Donations: $" + streamEarnings;
                if (Input.GetKeyDown(KeyCode.Delete) && Input.GetKey(KeyCode.Backspace))
                {
                    WipeSaveData();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
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

                    if (stomachContents < adjustedStomachCapacity)
                    {
                        if (fedDuringStream || alwaysUseEatingAnimation)
                        {
                            achievementButton.GetComponent<Collider2D>().enabled = false;
                            recordButton.GetComponent<Collider2D>().enabled = false;
                            StartCoroutine(Bounce(0.2f));
                            mouthSprite.enabled = true;
                            overrideFace.enabled = true;
                            faces.SetCounterTo(0);//imageIndex > 3 ? 3 : 0);
                            gulpPlayer.PlayRandom();
                            yield return new WaitForSeconds(0.6f);
                            StartCoroutine(Bounce(0.2f));
                            yield return new WaitForSeconds(0.2f);
                            mouthSprite.enabled = false;
                            overrideFace.enabled = false;
                            achievementButton.GetComponent<Collider2D>().enabled = true;
                        }
                        stomachContents += 0.4f;
                        if (isStreaming) foodEaten++;
                        holdFaceDuration = 0f;
                        gulpPlayer.PlayRandom();
                        if (achievements[1] && stomachContents >= adjustedStomachCapacity)
                        {
                            PrintStats();
                            if (fedDuringStream || alwaysUseEatingAnimation)
                            {
                                achievementButton.GetComponent<Collider2D>().enabled = false;
                                recordButton.GetComponent<Collider2D>().enabled = false;
                                StartCoroutine(Bounce(0.2f));
                                mouthSprite.enabled = true;
                                overrideFace.enabled = true;
                                faces.SetCounterTo(0);//imageIndex > 4 ? 3 : 0);
                                gulpPlayer.PlayRandom();
                                yield return new WaitForSeconds(0.6f);
                                StartCoroutine(Bounce(0.2f));
                                yield return new WaitForSeconds(0.2f);
                                mouthSprite.enabled = false;
                                overrideFace.enabled = false;
                                achievementButton.GetComponent<Collider2D>().enabled = true;
                            }
                            else
                            {
                                yield return new WaitForSeconds(0.3f);
                            }
                            stomachContents += 0.4f;
                            if (isStreaming) foodEaten++;
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
                            if (digestiveContents > 6f)
                            {
                                foodDescription = "You struggle to cram one more burger into your greedy, gigantic belly, panting heavily as your overfilled stomach pushes up against your lungs.";

                            }
                            else if (digestiveContents > 4f)
                            {
                                foodDescription = "You manage to force down another burger, but you are starting to feel overwhelmed by the mountain of food now sitting in your enormously stuffed belly.";

                            }
                            else if (digestiveContents > 2f)
                            {
                                foodDescription = "Although you feel unimaginably full, you continue eating, motivated purely by the desire to make your belly as huge and round as possible.";

                            }
                            else if (digestiveContents > 1f)
                            {
                                foodDescription = "Your full belly is starting to round out nicely, but you continue eating at the same pace.";

                            }
                            else
                            {
                                foodDescription = "You greedily devour a burger in mere seconds.";
                            }
                        }
                        UpdateMedicineText();
                        PrintStats();
                        //if (stomachContents > adjustedStomachCapacity && hungerModifier <= 1) stomachContents = adjustedStomachCapacity //restrict overstuffing if not unlocked
                    }
                    else if (hungerModifier <= 1f)
                    {
                        foodDescription = "You feel too full to eat another bite.";
                    }
                    else
                    {
                        foodDescription = "You try to force down even more food, but the pressure in your overfilled stomach is so immense that swallowing has become physically impossible.";
                    }
                    //Debug.Log(foodDescription);
                    
                }
                foodText.text = foodDescription;

                if (clickedButtonName == "weed_button")
                {
                    achievementText.text = "";
                    weedStock--;
                    munchiesConsumed++;
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
                    StartCoroutine(Bounce(0.3f));
                    yield return StartCoroutine(BellyJiggle(true));
                }
                if (holdFaceDuration > 0f) holdFaceDuration -= Time.deltaTime;
                if (holdFaceDuration <= 0f)
                {
                    holdFaceDuration = 0;
                    overrideFace.enabled = isAsleep;
                    if (isAsleep) faces.SetCounterTo(0);
                }

                if (clickedButtonName == "record_button" && !isStreaming)
                {
                    achievementText.text = "";
                    isStreaming = true;
                    recordButton.GetComponent<Collider2D>().enabled = false;
                    startingSize = imageIndex;
                    deltaSize = 0;
                    StartCoroutine(musicPlayer.ChangeTrackTo(2, 2f));
                    donationsText.text = "Donations: $" + streamEarnings;
                    UpdateMedicineText();
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
                    while (!Input.GetMouseButtonDown(0) || clickedButtonName == "always_eat_button")
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

                if (clickedButtonName == "surrogate_egg" && fetusCount < 6)
                {
                    fetusCount++;
                    money -= medicinePrices[6];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "enzyme" && stomachContents > 0f && intestineContents < (intestineCapacity * intestineMultiplier * trainingModifier))
                {
                    topHeavyAtStart = stomachContents > intestineContents;
                    if (stomachContents >= stomachCapacity * trainingModifier)
                    {
                        hungerTimer = 0;
                        hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
                        hungerText.text = "Hunger multiplier: " + hungerModifier + "x";
                    }
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
                    if (stomachContents <= intestineContents && topHeavyAtStart && imageIndex > 3)
                    {
                        gurglePlayer.PlayRandom();
                        topHeavyAtStart = false;
                    }
                    UpdateMedicineText();
                }

                if (clickedButtonName == "laxative" && intestineContents > 0f)
                {
                    dailyCalories += Mathf.Round(intestineContents * 1000);
                    if (!achievements[2] && (int)dailyCalories >= 6000) UpdateAchievements(2);
                    caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 400 : 0));
                    intestineContents = 0f;
                    disposalTimer = 0;
                    money -= medicinePrices[2];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "folate" && wombContents < fetusCount * 0.05f * pregnancyDays)
                {
                    wombContents += fetusCount * 0.04f;
                    wombContents = Mathf.Round(wombContents * 1000) / 1000;
                    money -= medicinePrices[3];
                    PrintStats();
                    UpdateMedicineText();
                }

                if (clickedButtonName == "ghrelin" && hungerTimer < 10)
                {
                    hungerTimer++;

                    if (hungerTimer > 3 && !gurglePlayer.GetComponent<AudioSource>().isPlaying)
                    {
                        gurglePlayer.PlayCustom(hungrySound, hungerTimer / 20f);
                        holdFaceDuration = hungrySound.length;
                    }
                    if (hungerTimer > 5)
                    {
                        faces.SetCounterTo(5);
                        overrideFace.enabled = true;
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



                yield return null;
            }
            if (!isAsleep) achievementText.text = "";
            if (isStreaming)
            {
                money += streamEarnings;
                cumulativeEarnings += streamEarnings;
                //Debug.Log(cumulativeEarnings);
                if (!achievements[9] && cumulativeEarnings >= 2000) UpdateAchievements(9);
                moneyText.text = "$" + money;
                StartCoroutine(musicPlayer.ChangeTrackTo(1, 2f));
                daysUntilNextStream = achievements[9] ? 1 : 2;
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
            overrideFace.enabled = isAsleep;
            //end of hour
            topHeavyAtStart = stomachContents > intestineContents;

            if (!ateThisTurn && (hungerModifier - 0.2f * munchiesConsumed) <= 1f) foodDescription = "";
            if (stomachContents >= adjustedStomachCapacity)
            {
                if (!isAsleep && (hungerModifier - 0.2f * munchiesConsumed) > 1.01f)
                {
                    foodDescription = "The adrenaline from your feeding frenzy wears off, and the intense sensation of fullness hits you all at once.";
                }            
                else
                {
                    foodDescription = "";
                }
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
                        default:
                            foodDescription = "You feel numerous babies squirming in protest within your cramped, overstuffed tummy.";
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
                        default:
                            foodDescription = "You feel numerous babies shifting slightly inside your tummy.";
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
                    overrideFace.enabled = true;
                }

                hungerModifier = 1f + (hungerTimer * 0.2f) + (0.2f * munchiesConsumed);
                if (!achievements[0] && hungerModifier >= 4f) UpdateAchievements(0);
                flowEnabled = false;
                //foodDescription = "";
            }
            foodText.text = foodDescription;





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
                    if (!achievements[2] && (int)dailyCalories >= 6000) UpdateAchievements(2);
                    caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 400 : 0));
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



            if (currentTime == 4)
            {
                munchiesConsumed = 0;
                if (weedStock < 5)
                {
                    weedStock++;
                    if (achievements[0] && weedStock < 5) weedStock++;
                }
                pregnancyDays++;
                if (daysUntilNextStream > 0) daysUntilNextStream--;
                //Debug.Log(pregnancyDays);
                if (pregnancyDays < 20)
                {
                    wombContents += fetusCount * ((dailyCalories >= 2000) ? 0.05f : 0.01f);
                }
                else if (pregnancyDays <= 40) 
                {
                    wombContents += fetusCount * ((dailyCalories >= 2000 + fetusCount * 400) ? 0.05f : 0.01f); //Mathf.Clamp(0.025f * ((dailyCalories - 2000) / fetusCount) / 400, 0f, 0.025f));
                }
                else
                {
                    StartCoroutine(musicPlayer.GraduallyMute(2f));
                    stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier, stomachCapacityBar.localScale.y, 1);
                    intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier, intestineCapacityBar.localScale.y, 1);
                    yield return StartCoroutine(AnimateBirth());
                    wombContents = 0;
                    pregnancyDays = 0;
                    actualDays = 0;
                    wombCapacityBar.localScale = new Vector3(fetusCount * 2f, wombCapacityBar.localScale.y, 1);
                }
                //if (dailyCalories >= 2000 + fetusCount * 800) wombContents += fetusCount * 0.025f;
                //Mathf.Clamp(wombContents, 0f, fetusCount * pregnancyDays * 0.05f);

                bankedCalories += (int)dailyCalories - (2000 + 400 * (pregnancyDays >= 20 ? 1 : 0));
                Mathf.Clamp(bankedCalories, 0, 10000);
                //Debug.Log("banked calories: " + bankedCalories);
                dailyCalories = 0;
                caloriesText.text = Mathf.Round(dailyCalories) + " / " + (2000 + fetusCount * (pregnancyDays >= 20 ? 400 : 0));
            }

            if (isAsleep && sleepCountdown == 0)
            {
                isAsleep = false;
                if (!achievements[6] && stomachContents > stomachCapacity * trainingModifier) UpdateAchievements(6);
            }
            achievementButton.GetComponent<Collider2D>().enabled = !isAsleep;
            if (holdFaceDuration <= 0f)
            {
                overrideFace.enabled = isAsleep;
                faces.SetCounterTo(0);
            }
            timeText.text = currentTime + ":00 | Day " + actualDays;
            hungerText.text = "Hunger multiplier: " + hungerModifier + "x";

            //stomachContents = Mathf.Round(stomachContents * 1000) / 1000;
            //intestineContents = Mathf.Round(intestineContents * 1000) / 1000;
            //wombContents = Mathf.Round(wombContents * 1000) / 1000;
            //trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;
            if (stomachContents <= intestineContents && topHeavyAtStart && imageIndex > 3) gurglePlayer.PlayRandom();
            PrintStats();
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
        float totalBellyContents = stomachContents + intestineContents + wombContents;
        float digestiveContents = stomachContents + intestineContents;
        switch (Mathf.Floor(digestiveContents))
        {
            case 0:
                if (digestiveContents > 0.5f)
                {
                    bellyDescription = "You feel satisfied.";
                }
                else if (digestiveContents <= 0f)
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
                else
                {
                    bellyDescription = "You feel like you can eat more.";
                }
                break;
            case 1:
                if (digestiveContents >= 1.5f)
                {
                    bellyDescription = "You feel overwhelmingly full.";
                }
                else
                {
                    bellyDescription = "You feel comfortably full.";
                }
                break;
            case 2:
                if (digestiveContents > 2.5f)
                {
                    bellyDescription = "Your swollen, tightly stretched belly is so densely packed that it feels like a boulder. Your arms can barely reach your belly button.";
                }
                else
                {
                    bellyDescription = "Your belly is so enormously stuffed that you can barely stand upright.";
                }
                break;
            case 3:
                bellyDescription = "Your belly, stuffed beyond the limits of what should be possible, is so massively engorged that stretch marks are starting to form.";
                break;
            case 4:
                bellyDescription = "Your huge, food-packed belly is even larger than the size of a full-term pregnancy. The overwhelming sensation of fullness threatens to shut down your brain.";
                break;
            case 5:
                bellyDescription = "The sensations of pleasure from your ridiculous belly, now rivaling the size of the rest of your body, are laced with streaks of pain as you approach the physical limit of how much food you can cram into yourself.";
                break;
            case 6:
                bellyDescription = "Your gigantic belly has gone far beyond the limit of what the human body is meant to withstand. By volume, you are almost more food than girl at this point and you feel your consciousness slipping away as your brain is bombarded with intense sensations of orgasmic pleasure and pain from your overstretched skin, abdominal muscles, and tightly stuffed guts.";
                break;
            default:
                bellyDescription = "Every inch of your digestive tract is thoroughly stuffed and your belly feels as huge as it can possibly get. You cannot even burp, as a burp would imply the existence of a pocket of air trapped somewhere in your enormously distended abdomen, and every last bit of space relieved by previous burps has already been crammed with even more food. Your massive belly easily contains more than your own body weight and feels like it might burst if you force down one more bite.";
                if (!achievements[3]) UpdateAchievements(3);
                break;

        }
        switch (Mathf.Floor(totalBellyContents))
        {
            case 0:
                if (totalBellyContents > 0.5f)
                {
                    //bellyDescription = "You feel satisfied.";
                    imageIndex = 2;
                }
                else if (totalBellyContents <= 0f)
                {
                    /*switch (hungerTimer)
                    {
                        case 0:
                        case 1:
                            break;
                        case 2:
                        case 3:
                            break;
                        case 4:
                        case 5:
                            break;
                        case 6:
                        case 7:
                            break;
                        case 8:
                        case 9:
                        case 10:
                            break;
                        default:
                            break;
                    }*/
                    imageIndex = 0;
                }
                else
                {
                    imageIndex = 1;
                }
                break;
            case 1:
                if (totalBellyContents >= 1.5f)
                {
                    imageIndex = 4;
                }
                else
                {
                    imageIndex = 3;
                }
                break;
            case 2:
                if (totalBellyContents > 2.5f)
                {
                    imageIndex = 6;
                }
                else
                {
                    imageIndex = 5;
                }
                break;
            case 3:
                imageIndex = 7;
                break;
            case 4:
                imageIndex = 8;
                break;
            case 5:
                imageIndex = 9;
                break;
            case 6:
                imageIndex = 10;
                break;
            case 7:
                imageIndex = 11;
                break;
            case 8:
            case 9:
                imageIndex = 12;
                break;
            default:
                imageIndex = 13;
                if (!achievements[8]) UpdateAchievements(8);
                break;
        }
        if (isStreaming)
        {
            deltaSize = imageIndex - startingSize;
            //Debug.Log(deltaSize);
        }
        RefreshTouchColliders();
        if (stomachContents > intestineContents)
        {
            spriteRenderer.sprite = characterSpritesTop[imageIndex];
            UpdateWombTattoo();
        }
        else 
        { 
            spriteRenderer.sprite = characterSpritesBtm[imageIndex];
            UpdateWombTattoo();
        }
        //Debug.Log("Total belly contents: " + (stomachContents + intestineContents) + " | " + bellyDescription);
        bellyText.text = bellyDescription;

        stomachContents = Mathf.Round(stomachContents * 1000) / 1000;
        intestineContents = Mathf.Round(intestineContents * 1000) / 1000;
        wombContents = Mathf.Round(wombContents * 100) / 100;
        trainingModifier = Mathf.Round(trainingModifier * 1000) / 1000;

        stomachCapacityBar.localScale = new Vector3(stomachCapacity * trainingModifier, stomachCapacityBar.localScale.y, 1);
        stomachContentsBar.localScale = new Vector3(stomachContents, stomachContentsBar.localScale.y, 1);
        intestineCapacityBar.localScale = new Vector3(intestineCapacity * intestineMultiplier * trainingModifier, intestineCapacityBar.localScale.y, 1);
        intestineContentsBar.localScale = new Vector3(intestineContents, intestineContentsBar.localScale.y, 1);
        wombContentsBar.localScale = new Vector3(wombContents, intestineContentsBar.localScale.y, 1);
        wombCapacityBar.localScale = new Vector3(fetusCount * 2f, wombCapacityBar.localScale.y, 1);
        //Debug.Log("poop timer: " + disposalTimer);
        Debug.Log("Time: " + currentTime + ":00 | stomach: " + stomachContents + "L / " + (stomachCapacity * trainingModifier) + "L | intestines: " + intestineContents + "L / " + (intestineCapacity * intestineMultiplier * trainingModifier) + "L | hunger modifier = " + hungerModifier);

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
}
