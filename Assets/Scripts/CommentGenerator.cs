using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentGenerator : MonoBehaviour
{
    [SerializeField] private GameObject comment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateComment(string contents, float heightMultiplier)
    {
        GameObject newComment = Instantiate(comment, transform.position, Quaternion.identity);
        newComment.transform.parent = this.transform;
        newComment.transform.localPosition = new Vector3(2000f + (Random.Range(0f, 1000f)), 450f * heightMultiplier, 0f);
        newComment.transform.localScale = new Vector3(1f, 1f, 1f);
        newComment.GetComponentInChildren<Text>().text = contents;
    }
}
