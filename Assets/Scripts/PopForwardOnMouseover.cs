using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopForwardOnMouseover : MonoBehaviour
{
    private int startingOrder;
    public int newOrder;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Cursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        startingOrder = spriteRenderer.sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {

        if (cursor.GetAllColliderNames(5).Contains(this.gameObject.name))
        {
            spriteRenderer.sortingOrder = newOrder;
        }
        else
        {
            spriteRenderer.sortingOrder = startingOrder;
        }
    }
}
