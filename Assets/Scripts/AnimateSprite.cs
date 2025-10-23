using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    private enum animationState { blinking, glowing };
    [SerializeField] private animationState currentState;
    private SpriteRenderer spriteRenderer;
    public float animationDuration = 1f;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
        }
        else if (currentState == animationState.glowing)
        {

        }

    }

    void OnDisable()
    {
        spriteRenderer.enabled = true;
        timer = 0f;
    }
}
