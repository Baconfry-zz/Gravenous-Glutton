using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentGenerator : MonoBehaviour
{
    [SerializeField] private GameObject comment;
    private float[] yPositions = new float[41];
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        for (int i = 0; i < yPositions.Length; i++)
        {
            yPositions[i] = -1f + 0.05f * i;
        }
        Reshuffle();
    }

    void Reshuffle()
    {
        for (int i = 0; i < yPositions.Length; i++)
        {
            float n = yPositions[i];
            int r = Random.Range(i, yPositions.Length);
            yPositions[i] = yPositions[r];
            yPositions[r] = n;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateComment(string contents)
    {
        GameObject newComment = Instantiate(comment, transform.position, Quaternion.identity);
        newComment.transform.parent = this.transform;
        newComment.transform.localPosition = new Vector3(2000f + (Random.Range(0f, 1000f)), 450f * yPositions[index], 0f);
        newComment.transform.localScale = new Vector3(1f, 1f, 1f);
        newComment.GetComponentInChildren<Text>().text = contents;

        index++;
        if (index == yPositions.Length)
        {
            index = 0;
            Reshuffle();
        }
    }
}
