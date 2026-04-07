using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private MainLoop mainLoop;
    private GameObject sprites;
    private GameObject UIobject;
    private Cursor cursor;
    private Collider2D hitbox;
    [SerializeField] private Sprite[] walkingSprites;
    [SerializeField] private Sprite[] eatenSprites;
    [SerializeField] private float frameDelay = 0.2f;
    [SerializeField] private float velocityScale = 2f;
    public bool pickedUp;
    public bool wasEaten;
    public int health = 40;
    public float velocityDamageThreshold = 0.25f;
    public float dampeningMultiplier = 0.3f;
    public float gravityFactor = 0.5f;
    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform maxHealthBar;
    // Start is called before the first frame update
    public Vector2 velocity = Vector2.zero;
    public int swallowingStage = 0;
    public float pickupTimer = 0f;
    private bool isWalking = false;
    
    void Awake()
    {
        mainLoop = GameObject.FindAnyObjectByType<MainLoop>();
        cursor = GameObject.FindAnyObjectByType<Cursor>();
        sprites = transform.Find("sprites").gameObject;
        UIobject = GameObject.Find("UI");
        hitbox = GetComponent<Collider2D>();
    }

    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //Application.targetFrameRate = 20;
        StartCoroutine(AnimateWalking());
        StartCoroutine(Ragdoll());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && cursor.heldPrey == null && cursor.GetAllGameObjectsWithColliders(5).Contains(this.gameObject) && !pickedUp && pickupTimer <= 0f)
        {
            StartCoroutine(GetDragged());
        }
        if (pickupTimer > 0f) pickupTimer -= Time.deltaTime;
        sprites.SetActive(UIobject.activeInHierarchy);
        hitbox.enabled = UIobject.activeInHierarchy;

    }
    void FixedUpdate()
    {
        /*if (pickedUp)
        {
            //transform.position = cursor.transform.position + new Vector3(0f, -0.7f, 0f);
            float newVelocityX = (cursor.transform.position.x - transform.position.x) * velocityScale;
            float newVelocityY = (cursor.transform.position.y - transform.position.y - 0.7f) * velocityScale;

            //if (Mathf.Abs(transform.position.x) > 7.385f) newVelocityX = 0f;
            //if (transform.position.y < -4.5f || transform.position.y > 4.5f) newVelocityY = 0f;
            //rig.velocity = new Vector2(newVelocityX, newVelocityY);
            //rig.gravityScale = 0f;
            transform.Translate(new Vector3(newVelocityX * Time.deltaTime, newVelocityY * Time.deltaTime));
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7.385f, 7.385f), Mathf.Clamp(transform.position.y, -4.7f, 3.5f));
        }
        else
        {
            //rig.gravityScale = 1f;
        }
        if (Input.GetMouseButtonDown(0) && cursor.GetAllColliderNames(5).Contains(this.gameObject.name) && !pickedUp)
        {
            pickedUp = true;
        }
        if (Input.GetMouseButtonUp(0) && pickedUp)
        {
            pickedUp = false;
        }
        if (Input.GetKey(KeyCode.F)) wasEaten = true;*/
    }

    IEnumerator GetDragged()
    {
        pickedUp = true;
        isWalking = false;
        cursor.heldPrey = this;
        while (!Input.GetMouseButtonUp(0))
        {
            float newVelocityX = (cursor.transform.position.x - transform.position.x) * velocityScale;           
            float newVelocityY = (cursor.transform.position.y - transform.position.y - 0.7f) * velocityScale;
            velocity = new Vector2(newVelocityX * Time.deltaTime, newVelocityY * Time.deltaTime);
            //if (Mathf.Abs(transform.position.x) > 7.385f) newVelocityX = 0f;
            //if (transform.position.y < -4.5f || transform.position.y > 4.5f) newVelocityY = 0f;
            //rig.velocity = new Vector2(newVelocityX, newVelocityY);
            //rig.gravityScale = 0f;
            spriteRenderer.flipX = Mathf.Sign(velocity.x) > 0;
            transform.Translate(velocity * 125 * Time.deltaTime);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7.385f, 7.385f), Mathf.Clamp(transform.position.y, -4.7f, 3.5f));
            yield return null;
        }
        pickedUp = false;
        pickupTimer = 0.6f;
        yield return null;
        cursor.heldPrey = null;
    }

    IEnumerator Ragdoll()
    {
        while (!wasEaten)
        {
            if (!pickedUp)
            {
                velocity = new Vector2(velocity.x, velocity.y - (isWalking ? 0 : gravityFactor * Time.deltaTime));

                transform.Translate(velocity * 125 * Time.deltaTime);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7.385f, 7.385f), Mathf.Clamp(transform.position.y, -4.7f, 3.5f));

                if (transform.position.x >= 7.385f && Mathf.Sign(velocity.x) >= 0)
                {
                    transform.position = new Vector3(7.385f, transform.position.y, transform.position.z);
                    if (Mathf.Abs(velocity.x) > velocityDamageThreshold) health -= 2;
                    velocity = new Vector2(Mathf.Abs(velocity.x) * (isWalking ? -1 : -dampeningMultiplier), velocity.y);
                }
                if (transform.position.x <= -7.385f && Mathf.Sign(velocity.x) < 0)
                {
                    transform.position = new Vector3(-7.385f, transform.position.y, transform.position.z);
                    if (Mathf.Abs(velocity.x) > velocityDamageThreshold) health -=2;
                    velocity = new Vector2(Mathf.Abs(velocity.x) * (isWalking ? 1 : dampeningMultiplier), velocity.y);                
                }
                if (transform.position.y >= 3.5f && Mathf.Sign(velocity.y) >= 0)
                {
                    transform.position = new Vector3(transform.position.x, 3.5f, transform.position.z);
                    if (Mathf.Abs(velocity.y) > velocityDamageThreshold) health -= 2;
                    velocity = new Vector2(velocity.x, Mathf.Abs(velocity.y) * -dampeningMultiplier);
                }
                if (transform.position.y <= -4.7f && Mathf.Sign(velocity.y) < 0)
                {
                    transform.position = new Vector3(transform.position.x, -4.7f, transform.position.z);
                    if (Mathf.Abs(velocity.y) > velocityDamageThreshold) health -= 2;
                    velocity = new Vector2(velocity.x, Mathf.Abs(velocity.y) * dampeningMultiplier);
                }
                if (health < 0) health = 0;
                UpdateHealthBar();
                if (Mathf.Abs(velocity.y) < 0.01f && transform.position.y <= -4.695f)
                {
                    if (!isWalking)
                    {
                        float xVelocity = velocity.x * Mathf.Pow(0.001f, Time.deltaTime);
                        if (xVelocity > 0.001f)
                        {
                            velocity = new Vector2(xVelocity, 0f);
                        }
                        else
                        {
                            isWalking = true;
                            velocity = new Vector2(Random.Range(0.01f * health / 40, 0.03f * health / 40) * (Random.Range(0, 2) == 0 ? -1 : 1), 0f);
                        }
                    }
                    else
                    {

                    }

                }
                spriteRenderer.flipX = Mathf.Sign(velocity.x) > 0;

            }
            yield return null;
        }
    }

    public void TossInRandomDirection(float magnitude)
    {
        float angle = Random.Range(Mathf.PI * 0.25f, Mathf.PI * 0.75f);
        velocity = new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
    }

    IEnumerator AnimateWalking()
    {
        while (!wasEaten)
        {
            while (health > 0)
            {
                if (wasEaten)
                {
                    //spriteRenderer.transform.localPosition = new Vector3(0f, 0.22f, 0f);
                    break;
                }
                for (int i = 0; i < walkingSprites.Length; i++)
                {
                    spriteRenderer.sprite = walkingSprites[i];
                    yield return new WaitForSeconds(frameDelay * 80 / (health + 40));                   
                }
            }
            spriteRenderer.sprite = walkingSprites[1];
            yield return null;
        }
        //GetComponent<Collider2D>().enabled = false;
        //rig.gravityScale = 0f;
        //spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        //spriteRenderer.transform.localPosition = new Vector3(0f, 0.22f, 0f);

        /*for (int i = 0; i < eatenSprites.Length; i++)
        {
            yield return new WaitForSeconds(1f);
            spriteRenderer.transform.localScale = new Vector3(0.6f, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
            spriteRenderer.sprite = eatenSprites[i];
        }*/
    }

    public void IncrementSwallowingStage()
    {
        //spriteRenderer.transform.localScale = new Vector3(0.6f, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
        if (swallowingStage < eatenSprites.Length)
        {
            spriteRenderer.sprite = eatenSprites[swallowingStage];
            swallowingStage++;
        }
        else
        {
            transform.Find("sprites").gameObject.SetActive(false);
        }
    }

    void UpdateHealthBar()
    {
        healthBar.localScale = new Vector3(health / 40f, healthBar.localScale.y, healthBar.localScale.z);
    }
}
