using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private MainLoop mainLoop;
    // Start is called before the first frame update
    void Start()
    {
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
    }
}
