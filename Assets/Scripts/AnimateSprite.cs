using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    private enum animationState { blinking, glowing };
    [SerializeField] private animationState currentState;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    public float animationDuration = 1f;
    private float timer;

    private Color newColor;
    //[SerializeField] private Color minColor;
    //[SerializeField] private Color maxColor;
    //[SerializeField] private float pulseRate;
    // Start is called before the first frame update
    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == animationState.blinking)
        {
            timer += Time.deltaTime;
            if (timer > animationDuration)
            {
                timer = 0f;
                foreach (SpriteRenderer spr in spriteRenderers)
                {
                    spr.enabled = !spr.enabled;
                }
                
            }
        }
        else if (currentState == animationState.glowing)
        {
            foreach (SpriteRenderer spr in spriteRenderers)
            {
                if (spr.enabled)
                {
                    newColor = Color.Lerp(new Color(spr.color.r, spr.color.g, spr.color.b, 0f), new Color(spr.color.r, spr.color.g, spr.color.b, 1f), Mathf.PingPong(Time.time, animationDuration));
                    spr.color = newColor;
                }
            }
        }

    }

    void OnDisable()
    {
        foreach (SpriteRenderer spr in spriteRenderers)
        {
            spr.enabled = true;
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
        }
        
        timer = 0f;
    }
}
