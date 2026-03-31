using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private MainLoop mainLoop;
    [SerializeField] private GameObject droppedFood;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;

        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = 1 << 5;
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
            if (collider != null)
            {
                mainLoop.clickedButtonName = collider.gameObject.name;
            }
            else
            {
                layerMask = 1 << 3;
                collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
                if (collider != null)
                {
                    mainLoop.clickedButtonName = collider.transform.parent.gameObject.name;
                }
            }
        }
        else
        {
            mainLoop.clickedButtonName = "";
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, GetColliderName(5) == "mouth" ? 0.6f : 1f);

    }

    public void DropFood()
    {
        GameObject newFood = Instantiate(droppedFood, transform.position, Quaternion.identity);
        newFood.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        Destroy(newFood, 1f);
    }

    public string GetColliderName(int mask)
    {
        int layerMask = 1 << mask;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        return collider != null ? collider.gameObject.name : "";
    }

    public List<string> GetAllColliderNames(int mask)
    {
        int layerMask = 1 << mask;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.01f, layerMask);
        List<string> names = new List<string>();
        for (int i = 0; i < colliders.Length; i++)
        {
            names.Add(colliders[i].gameObject.name);
        }

        /*layerMask = 1 << 3;
        Collider2D otherCollider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        if (otherCollider != null)
        {
            names.Add(otherCollider.transform.parent.gameObject.name);
        }*/

        return names;
    }
}
