using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamComment : MonoBehaviour
{
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(1f, 3f);
        Destroy(this.gameObject, 60f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Time.deltaTime * speed, 0f, 0f);
    }

    void OnDisable()
    {
        Destroy(this.gameObject);
    }
}
