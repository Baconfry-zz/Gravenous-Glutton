using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedSlider : MonoBehaviour
{
    private float value;
    private float volume;
    private float climax;
    [SerializeField] private float maxVolume;

    [SerializeField] private Transform volumeTransform;
    [SerializeField] private SpriteRenderer screenFlash;
    [SerializeField] private SpriteRenderer pregnancyPreview;
    [SerializeField] private SpriteRenderer sleepingHead;
    //[SerializeField] private SpriteRenderer sleepingHeadBackdrop;
    [SerializeField] private SpriteRenderer previewBG;
    [SerializeField] private Sprite[] previewSprites = new Sprite[3];

    [SerializeField] private DigitCounter backdropFace;
    [SerializeField] private DigitCounter previewFace;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject bars;
    [SerializeField] private MainLoop mainLoop;

    [SerializeField] private AudioPlayer sexualMoansPlayer;
    //[SerializeField] private AudioPlayer coomPlayer;
    [SerializeField] private AudioClip orgasmMoan;


    //[SerializeField] private Text climaxText;
    [SerializeField] private Transform climaxTransform;

    public float speedMultiplier;
    //[SerializeField] private float initialFrameRate;
    private bool clickedThisCycle = false;

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
        bars.SetActive(false);
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
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !clickedThisCycle && !mainLoop.clickedButtonName.StartsWith("toggle"))
        {
            clickedThisCycle = true;
            volume += value;
            climax += 10f;
            if (climax > 200f) climax = 200f;
            if (volume > maxVolume) volume = maxVolume;
            volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
            climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
            StartCoroutine(mainLoop.Bounce(0.1f));
            sexualMoansPlayer.PlayRandom();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            clickedThisCycle = true;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                volume = maxVolume;
                volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
            }
            climax = 200f;
            climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
            StartCoroutine(mainLoop.Bounce(0.1f));
            sexualMoansPlayer.PlayRandom();
        }
        screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, climax / 1000f);

    }

    public IEnumerator Oscillate(int currentIndex)
    {
        previewBG.enabled = false;
        pregnancyPreview.enabled = false;
        sleepingHead.enabled = true;
        backdropFace.enabled = true;

        amountReleased = 0f;
        canvas.enabled = false;
        buttons.SetActive(false);
        bars.SetActive(false);

        while (climax < 200f)
        {
            clickedThisCycle = false;
            speedMultiplier = Mathf.Min((40f + climax) / 2f, 69f);
            //Debug.Log(speedMultiplier);
            while (value < 10)
            {
                value += Time.deltaTime * speedMultiplier;
                if (value > 10) value = 10f;
                //valueText.text = value.ToString();
                transform.localScale = new Vector3((value / 10f), transform.localScale.y, 1);
                yield return null;
                previewFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
                backdropFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
            }
            while (value > 0)
            {
                value -= Time.deltaTime * speedMultiplier;
                if (value < 0) value = 0f;
                //valueText.text = value.ToString();
                transform.localScale = new Vector3((value / 10f), transform.localScale.y, 1);
                yield return null;
                previewFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
                backdropFace.SetCounterTo(Mathf.CeilToInt(climax / 100f));
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
        StartCoroutine(mainLoop.BellyJiggle(true));
        value = 0f;
        transform.localScale = new Vector3(0f, transform.localScale.y, 1f);
        climax = 200;
        amountReleased = volume;
        
        //if (amountReleased > 200) amountReleased = 200;
        pregnancyPreview.sprite = previewSprites[Mathf.Min((int)amountReleased, 200) / 100];
        /*int previewPregnancyIndex = 0;
        switch (amountReleased / 100)
        {
            case 0:
                previewPregnancyIndex = 5;
                break;
            case 1:
                previewPregnancyIndex = 8;
                break;
            case 2:
                previewPregnancyIndex = 10;
                break;
            default:
                previewPregnancyIndex = 0;
                break;
        }*/

        pregnancyPreview.enabled = true;
        //sleepingHead.enabled = true;
        previewBG.enabled = true;
        //Debug.Log(volume);
        pregnancyPreview.color = new Color(pregnancyPreview.color.r, pregnancyPreview.color.g, pregnancyPreview.color.b, 0.2f);
        //previewBG.color = new Color(previewBG.color.r, previewBG.color.g, previewBG.color.b, 0.2f);
        previewFace.SetCounterTo(2);
        backdropFace.SetCounterTo(2);

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
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, (float) climax / 200);
            yield return null;
        }
        while (volume > 0)
        {
            volume -= 200 * Time.deltaTime;
            if (volume < 0) volume = 0f;
            volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        previewFace.SetCounterTo(0);
        backdropFace.SetCounterTo(0);
        pregnancyPreview.enabled = false;
        sleepingHead.enabled = false;
        previewBG.enabled = false;
        backdropFace.enabled = false;
        value = 0;
        volume = 0;
        climax = 0;
        transform.localScale = new Vector3((value / 10f), transform.localScale.y, 1);
        climaxTransform.localScale = new Vector3(climax / 100f, climaxTransform.localScale.y, 1);
        volumeTransform.localScale = new Vector3(volume / 100f, volumeTransform.localScale.y, 1);
        canvas.enabled = true;
        bars.SetActive(true);
        buttons.SetActive(true);

        if (amountReleased >= maxVolume && !mainLoop.achievements[4]) mainLoop.UpdateAchievements(4);
    }
}
