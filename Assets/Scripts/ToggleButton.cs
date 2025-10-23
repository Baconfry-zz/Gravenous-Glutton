using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] private MainLoop mainLoop;

    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool isActive = true;
    public float delayUntilNextPress = 0f;

    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, isActive ? 1f : 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (GetComponent<Collider2D>().enabled)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
        }
        else
        {
            spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f, spriteRenderer.color.a);
        }*/

        if (mainLoop.clickedButtonName == this.gameObject.name && timer <= 0f)
        {
            isActive = !isActive;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, isActive ? 1f : 0.2f);
            if (objectToToggle.GetComponentsInChildren<AudioSource>().Length > 0)
            {
                AudioSource[] sources = objectToToggle.GetComponentsInChildren<AudioSource>();
                foreach (AudioSource source in sources)
                {
                    source.mute = !isActive;
                }
                //objectToToggle.GetComponent<AudioSource>().mute = !isActive;
            }
            else
            {
                objectToToggle.SetActive(isActive);
            }
            timer += delayUntilNextPress;
        }
        if (timer > 0) timer -= Time.deltaTime;
    }

    public void Brighten(bool newState)
    {
        if (newState)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
        }
        else
        {
            spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f, spriteRenderer.color.a);
        }
    }

    public void ForceState(bool newState)
    {
        isActive = newState;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, isActive ? 1f : 0.2f);
        if (objectToToggle.GetComponentsInChildren<AudioSource>().Length > 0)
        {
            AudioSource[] sources = objectToToggle.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource source in sources)
            {
                source.mute = !isActive;
            }
            //objectToToggle.GetComponent<AudioSource>().mute = !isActive;
        }
        else
        {
            objectToToggle.SetActive(isActive);
        }
    }
}
