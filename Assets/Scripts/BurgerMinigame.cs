using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerMinigame : MonoBehaviour
{
    [SerializeField] private GameObject[] layers = new GameObject[6];
    [SerializeField] private Text scoreText;
    private List<int> alreadyCollectedIngredients = new List<int>();
    private List<Transform> alreadySpawnedIngredients = new List<Transform>();
    public int layerIndex = 0;
    private int amountCompleted = 0;
    public int totalReward = 0;
    public int startingFoodStock = 0;
    [SerializeField] private GameObject ingredient;
    [SerializeField] private Cursor cursor;
    private int[] spawnedIDs = new int[] {0, 1, 2, 3, 4, 5};
    [SerializeField] private float minXvelocity;
    [SerializeField] private float maxXvelocity;
    [SerializeField] private float minYvelocity;
    [SerializeField] private float maxYvelocity;
    [SerializeField] private float minDelay;
    [SerializeField] private float maxDelay;
    private int layerMask = 1 << 6;
    private bool manuallyCanceled = false;
    [SerializeField] private SpriteRenderer endButton;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(ConstantlySpawnIngredients());
        //StartCoroutine(CollectFallingIngredients());

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Max(-2f, cursor.transform.position.x), transform.position.y, transform.position.z);
        if ((Input.GetMouseButtonDown(0) && cursor.GetColliderName(5) == "end_minigame") && !manuallyCanceled)
        {
            manuallyCanceled = true;
            endButton.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    bool AlreadyCollected(int id)
    {
        bool returnValue = false;
        for (int i = 0; i < alreadyCollectedIngredients.Count; i++)
        {
            if (alreadyCollectedIngredients[i] == id) returnValue = true;
        }
        return returnValue;
    }

    void UpdateScoreText()
    {
        scoreText.text = "\nFood earned: " + (totalReward > 9 ? "" : " ") + totalReward + "\nStorage: " + Mathf.Min(60, startingFoodStock + totalReward) + " / 60";
    }

    IEnumerator CollectFallingIngredients()
    {
        while (!manuallyCanceled && startingFoodStock + totalReward < 60)
        {
            Collider2D collider = Physics2D.OverlapCircle(new Vector3(transform.position.x, transform.position.y - 0.2f, 0f), 0.2f, layerMask);
            if (collider != null && collider.GetComponent<DigitCounter>() != null)
            {
                int newID = collider.GetComponent<DigitCounter>().index;
                if (layerIndex < 5)
                {
                    if (newID < 5 && !AlreadyCollected(newID))
                    {
                        layers[layerIndex].GetComponent<DigitCounter>().SetCounterTo(newID);
                        alreadyCollectedIngredients.Add(newID);
                        layerIndex++;
                        alreadySpawnedIngredients.Remove(collider.transform);
                        Destroy(collider.gameObject);
                    }
                    else
                    {
                        collider.GetComponent<Collider2D>().enabled = false;
                    }
                }
                else
                {
                    if (newID == 5)
                    {
                        layers[layerIndex].GetComponent<DigitCounter>().SetCounterTo(newID);
                        layerIndex++;
                        alreadySpawnedIngredients.Remove(collider.transform);
                        Destroy(collider.gameObject);
                    }
                    else
                    {
                        collider.GetComponent<Collider2D>().enabled = false;
                    }
                }

                if (layerIndex > 5)
                {
                    scoreText.text = "                   +" + Mathf.Max(1, 5 - amountCompleted) + "\nFood earned: " + (totalReward > 9 ? "" : " ") + totalReward + "\nStorage: " + Mathf.Min(60, startingFoodStock + totalReward) + " / 60";
                    totalReward += Mathf.Max(1, 6 - amountCompleted);
                    amountCompleted++;
                    if (startingFoodStock + totalReward >= 60) endButton.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    yield return new WaitForSeconds(0.6f);
                    UpdateScoreText();
                    alreadyCollectedIngredients.Clear();
                    layerIndex = 0;
                    for (int i = 0; i < layers.Length; i++)
                    {
                        layers[i].GetComponent<DigitCounter>().SetCounterTo(6);
                    }
                }
            }

            int startingIndex = 0;
            while (alreadySpawnedIngredients.Count > 0 && startingIndex < alreadySpawnedIngredients.Count)
            {
                if (alreadySpawnedIngredients[startingIndex].position.y < -6f)
                {
                    Destroy(alreadySpawnedIngredients[startingIndex].gameObject, 0.1f);
                    alreadySpawnedIngredients.Remove(alreadySpawnedIngredients[startingIndex]);
                    //currentStreak = 0;
                    UpdateScoreText();
                }
                startingIndex++;
            }
            yield return null;
        }

    }

    void Reshuffle()
    {
        for (int i = 0; i < spawnedIDs.Length - 1; i++)
        {
            int n = spawnedIDs[i];
            int r = Random.Range(i, spawnedIDs.Length - 1);
            spawnedIDs[i] = spawnedIDs[r];
            spawnedIDs[r] = n;
        }
    }

    void SpawnIngredient(int id)
    {
        Vector3 spawnLocation = new Vector3(8.5f, 3.5f, 0f);
        GameObject newIngredient = Instantiate(ingredient, spawnLocation, Quaternion.identity);
        newIngredient.transform.parent = this.transform.parent;
        newIngredient.GetComponent<DigitCounter>().SetCounterTo(id);
        newIngredient.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-minXvelocity, -maxXvelocity), (id == 5 ? maxYvelocity : Random.Range(minYvelocity, maxYvelocity)), 0f);
        //newIngredient.GetComponent<Rigidbody2D>().velocity = new Vector3(-maxXvelocity, maxYvelocity, 0f);
        newIngredient.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-30f, 30f);
        //Destroy(newIngredient, 2f);
        alreadySpawnedIngredients.Add(newIngredient.transform);

    }

    public IEnumerator ConstantlySpawnIngredients()
    {
        Reshuffle();
        endButton.color = Color.white;
        amountCompleted = 0;
        totalReward = 0;
        manuallyCanceled = false;
        alreadyCollectedIngredients.Clear();
        alreadySpawnedIngredients.Clear();
        layerIndex = 0;
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].GetComponent<DigitCounter>().SetCounterTo(6);
        }
        UpdateScoreText();
        StartCoroutine(CollectFallingIngredients());
        while (!manuallyCanceled && startingFoodStock + totalReward < 60)
        {
            for (int i = 0; i < spawnedIDs.Length;i++)
            {
                SpawnIngredient(spawnedIDs[i]);
                if (manuallyCanceled || startingFoodStock + totalReward >= 60)
                {
                    //endButton.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    break;
                }
                float randomizedDelay = Random.Range(minDelay, maxDelay);
                if (randomizedDelay > minDelay + (maxDelay - minDelay) / 3) randomizedDelay = Random.Range(minDelay, maxDelay);
                if (randomizedDelay > minDelay + (maxDelay - minDelay) / 3) randomizedDelay = Random.Range(minDelay, maxDelay);
                yield return new WaitForSeconds(randomizedDelay);
            }
            //if (startingFoodStock + totalReward >= 60) endButton.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            yield return new WaitForSeconds(1f);
            Reshuffle();
        }

        //scoreText.text += "        Results: +" + totalReward;
        yield return new WaitForSeconds(1.5f);
        for(int i = 0; i < alreadySpawnedIngredients.Count; i++)
        {
            Destroy(alreadySpawnedIngredients[i].gameObject);
        }
        this.transform.parent.gameObject.SetActive(false);
    }
}
