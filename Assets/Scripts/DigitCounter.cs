using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitCounter : MonoBehaviour
{
    [SerializeField] private Sprite[] digits;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCounterTo(int index)
    {
        spriteRenderer.sprite = digits[index];
    }

    public void SetAltColor(bool isMaxed)
    {
        spriteRenderer.color = isMaxed ? Color.yellow : Color.white;
    }

}
