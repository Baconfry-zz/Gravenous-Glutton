using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedSlider : MonoBehaviour
{
    private float value;
    private float volume;
    private float climax;
    private float maxVolume;

    [SerializeField] private Transform volumeTransform;
    [SerializeField] private SpriteRenderer screenFlash;
    [SerializeField] private SpriteRenderer pregnancyPreview;
    [SerializeField] private SpriteRenderer sleepingHead;
    //[SerializeField] private SpriteRenderer sleepingHeadBackdrop;
    [SerializeField] private SpriteRenderer previewBG;
    [SerializeField] private Sprite[] previewSprites = new Sprite[3];
    [SerializeField] private ToggleButton autoButton;
    [SerializeField] private GameObject cancelButton;

    //[SerializeField] private DigitCounter backdropFace;
    [SerializeField] private DigitCounter previewFace;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buttons;
    [SerializeField] private MainLoop mainLoop;
    [SerializeField] private Transform sexMoverTransform;

    [SerializeField] private AudioPlayer sexualMoansPlayer;
    [SerializeField] private AudioPlayer plapsPlayer;
    //[SerializeField] private AudioPlayer coomPlayer;
    [SerializeField] private AudioClip orgasmMoan;
    [SerializeField] private AudioClip nutBuster;
    [SerializeField] private Text volumeText;
    [SerializeField] private Text volumeValue;

    //[SerializeField] private Text climaxText;
    [SerializeField] private Transform climaxTransform;

    public float speedMultiplier;
    //[SerializeField] private float initialFrameRate;
    private bool clickedThisCycle = false;
    private bool canceledSex = false;
    private bool autoMode = false;
    public float amountReleased;
    
    void Awake()
    {
        canvas.enabled = false;
        
    }
    // Start is called before the first frame update
    void Start()
    {       
        value = 0f;
        volume = 0f;
        climax = 0f;
        amountReleased = 0f;
        buttons.SetActive(false);
        //frameRate = initialFrameRate;

        //valueText.text = value.ToString();
        //volumeText.text = volume.ToString();
        volumeTransform.localScale = new Vector3(volume / 1000f, volumeTransform.localScale.y, 1);
        //climaxText.text = climax.ToString();
        climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);

        //StartCoroutine(Oscillate());
    }

    // Update is called once per frame
    void Update()
    {
        cancelButton.GetComponent<Collider2D>().enabled = volume <= 0f && !autoMode;
        cancelButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, ((volume > 0f || canceledSex || autoMode) ? 0.1f : 1f));

        
        if (mainLoop.clickedButtonName == "auto")
        {
            autoMode = !autoMode;
        }
        else if (mainLoop.clickedButtonName == "cancel_sex")
        {
            canceledSex = true;
        }
        else if (((Input.GetMouseButtonDown(0)) || Input.GetKeyDown(KeyCode.Space) || (value == 10f && autoMode && !canceledSex && !(climax > 10f && volume + 190f < maxVolume))) && !clickedThisCycle && !mainLoop.clickedButtonName.StartsWith("toggle"))
        {
            clickedThisCycle = true;
            volume += value;
            climax += 10f;
            if (climax > 200f) climax = 200f;
            if (volume > maxVolume) volume = maxVolume;
            volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
            climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
            if (climax < 200f) StartCoroutine(mainLoop.Bounce(0.2f));
            StartCoroutine(mainLoop.BellyJiggle(false));
            plapsPlayer.PlayRandom();
            sexualMoansPlayer.PlayRandom();
        }
        if (Input.GetKeyDown(KeyCode.Return) && climax < 200f && amountReleased == 0f)
        {
            clickedThisCycle = true;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
            { 
                volume = maxVolume; 
                volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);            
            }
            else
            {
                volume += 10f;
                if (volume > maxVolume) volume = maxVolume;
                volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
            }
            climax = 200f;
            climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
            //if (climax < 200f) StartCoroutine(mainLoop.Bounce(0.2f));
            StartCoroutine(mainLoop.BellyJiggle(false));
            plapsPlayer.PlayRandom();
            sexualMoansPlayer.PlayRandom();
        }
        screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, climax / 1000f);
        volumeText.text = "VOLUME:           L / " + (Mathf.Round(maxVolume * 100) / 10000) + "L";
        volumeValue.text = (Mathf.Round(volume * 100) / 10000).ToString();
    }

    public IEnumerator Oscillate(bool startPregnancy, float volumeLimit)
    {
        cancelButton.SetActive(true);
        autoButton.ForceState(false);
        autoMode = false;
        previewBG.enabled = false;
        pregnancyPreview.enabled = false;
        sleepingHead.enabled = true;
        //backdropFace.enabled = true;

        canceledSex = false;

        amountReleased = 0f;
        canvas.enabled = false;
        buttons.SetActive(false);
        maxVolume = volumeLimit * 100;
        Vector3 startPosition = sexMoverTransform.localPosition;
        while (climax < 200f && !canceledSex)
        {
            clickedThisCycle = false;
            speedMultiplier = Mathf.Min((40f + climax) / 2f, 69f);
            //Debug.Log(speedMultiplier);
            while (value < 10)
            {
                value += Time.deltaTime * speedMultiplier;
                if (value > 10) value = 10f;
                if (climax <= 0 && volume > 0)
                {
                    volume -= 10 * Time.deltaTime;
                    if (volume < 0) volume = 0f;
                    volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
                }
                //valueText.text = value.ToString();
                transform.localScale = new Vector3((value / 10f), transform.localScale.y, 1);
                sexMoverTransform.localPosition = new Vector3(startPosition.x, startPosition.y + (value / 100), startPosition.z);
                yield return null;
                previewFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
                //backdropFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
            }
            while (value > 0)
            {
                value -= Time.deltaTime * speedMultiplier;
                if (value < 0) value = 0f;
                if (climax <= 0 && volume > 0)
                {
                    volume -= 10 * Time.deltaTime;
                    if (volume < 0) volume = 0f;
                    volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
                }
                //valueText.text = value.ToString();
                transform.localScale = new Vector3((value / 10f), transform.localScale.y, 1);
                sexMoverTransform.localPosition = new Vector3(startPosition.x, startPosition.y + (value / 100), startPosition.z);
                yield return null;
                previewFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
                //backdropFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
            }
            if (!clickedThisCycle)
            {
                climax -= (10f - (int)(climax / 5f));
                if (climax < 0) climax = 0f;
                if (climax > 200) climax = 200f;
                //climaxText.text = climax.ToString();
                climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
            }
        }
        if (!canceledSex)
        {
            cancelButton.SetActive(false);
            sexMoverTransform.position = startPosition;
            StartCoroutine(mainLoop.Bounce(0.3f));
            mainLoop.StopCoroutine("BellyJiggle");
            mainLoop.isPlayingJiggleAnim = false;
            StartCoroutine(mainLoop.BellyJiggle(true));
            value = 0f;
            transform.localScale = new Vector3(0f, transform.localScale.y, 1f);
            climax = 200;
            amountReleased = volume;

            //if (amountReleased > 200) amountReleased = 200;
            if (startPregnancy)
            {
                pregnancyPreview.sprite = previewSprites[mainLoop.fertilityBonus + Mathf.Min((int)amountReleased, 200) / 100];
                pregnancyPreview.enabled = true;
                previewBG.enabled = true;
                pregnancyPreview.color = new Color(pregnancyPreview.color.r, pregnancyPreview.color.g, pregnancyPreview.color.b, 0.2f);
            }

            //previewBG.color = new Color(previewBG.color.r, previewBG.color.g, previewBG.color.b, 0.2f);
            previewFace.SetCounterTo(2);
            //backdropFace.SetCounterTo(2);

            plapsPlayer.PlayCustom(nutBuster, 0.4f);
            sexualMoansPlayer.PlayCustom(orgasmMoan);

            while (climax > 0)
            {
                volume -= 200 * Time.deltaTime;
                if (volume < 0) volume = 0;
                climax -= 200 * Time.deltaTime;
                if (climax < 0) climax = 0;
                //volumeText.text = volume.ToString();
                volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
                climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
                //pregnancyPreview.color = new Color(pregnancyPreview.color.r, pregnancyPreview.color.g, pregnancyPreview.color.b, Mathf.Max(0.2f, (float)climax / amountReleased));
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, (float)climax / 200);
                yield return null;
            }
            while (volume > 0)
            {
                volume -= 200 * Time.deltaTime;
                if (volume < 0) volume = 0f;
                volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(0.5f);
        previewFace.SetCounterTo(0);
        //backdropFace.SetCounterTo(0);
        pregnancyPreview.enabled = false;
        sleepingHead.enabled = false;
        previewBG.enabled = false;
        //backdropFace.enabled = false;
        value = 0;
        volume = 0;
        climax = 0;
        transform.localScale = new Vector3((value / 10f), transform.localScale.y, 1);
        climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
        volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
        canvas.enabled = true;
        buttons.SetActive(true);

        if (amountReleased >= 300 && !mainLoop.achievements[4]) mainLoop.UpdateAchievements(4);
    }
}
