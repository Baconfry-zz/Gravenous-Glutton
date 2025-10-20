using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] private MainLoop mainLoop;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isActive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mainLoop.clickedButtonName == this.gameObject.name)
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
        }
    }
}
