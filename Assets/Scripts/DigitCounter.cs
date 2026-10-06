using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitCounter : MonoBehaviour
{
    [SerializeField] private Sprite[] digits;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public int index = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCounterTo(int newIndex)
    {
        spriteRenderer.sprite = digits[newIndex];
        index = newIndex;
    }

    public void SetAltColor(bool isMaxed)
    {
        spriteRenderer.color = isMaxed ? Color.yellow : Color.white;
    }

    public void Increment()
    {
        index++;
        if (index >= digits.Length) index = 0;
        spriteRenderer.sprite = digits[index];
    }
}
