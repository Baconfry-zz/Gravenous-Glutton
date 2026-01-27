using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource source;

    private int previousClipIndex = -1;
    
    // Start is called before the first frame update
    public float volumeMultiplier = 1f;

    private float internalVolume = 1f;
    private float startingVolume = 1f;

    private bool isTransitioning;
    public bool constantPlaying = false;
    
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        startingVolume = source.volume;
        if (constantPlaying) StartCoroutine(PlayConstantly());
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Q)) volumeMultiplier = 0f;
        if (Input.GetKeyDown(KeyCode.W)) volumeMultiplier = 0.5f;
        if (Input.GetKeyDown(KeyCode.E)) volumeMultiplier = 1f;
        if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(ChangeTrackTo(1, 5f));
        if (Input.GetKeyDown(KeyCode.A)) StartCoroutine(ChangeTrackTo(0, 5f));*/
        source.volume = internalVolume * volumeMultiplier;

    }

    public void Mute(bool state)
    {
        source.mute = state;
    }

    public void PlayAtIndex(int index)
    {
        internalVolume = startingVolume;
        previousClipIndex = index;
        source.clip = audioClips[index];
        source.Play();
    }

    public IEnumerator PlayConstantly()
    {
        while (true)
        {
            PlayRandom();
            while (source.isPlaying) yield return null;
            yield return new WaitForSeconds(1f);
        }
    }

    public void PlayRandom()
    {
        internalVolume = startingVolume;
        int clipIndex = Random.Range(0, audioClips.Length);
        for (int i = 0; i < 8; i++)
        {
            int newIndex = Random.Range(0, audioClips.Length);
            if (newIndex != previousClipIndex) clipIndex = newIndex;
        }
        previousClipIndex = clipIndex;
        source.clip = audioClips[clipIndex];
        source.Play();
        /*if (!source.isPlaying)
        {
            
        }*/
    }

    public IEnumerator PlayCustomWaitFor(AudioClip clip, AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        yield return null;
        internalVolume = 1f;
        this.source.clip = clip;
        this.source.Play();
    }

    public IEnumerator PlayCustomInterrupt(AudioClip clip, AudioSource source)
    {
        bool initialState = source.mute;
        source.mute = true;
        internalVolume = 1f;
        this.source.clip = clip;
        this.source.Play();
        while (this.source.isPlaying)
        {
            yield return null;
        }
        source.mute = initialState;
    }

    public void PlayCustom(AudioClip clip)
    {
        
        internalVolume = 1f;
        source.clip = clip;
        source.Play();
    }

    public void PlayCustom(AudioClip clip, float volume)
    {
        internalVolume = volume;
        source.clip = clip;
        source.Play();
    }

    public IEnumerator ChangeTrackTo(int index, float duration)
    {
        while (isTransitioning) yield return null;
        if (source.clip != audioClips[index])
        {
            float timer = 0f;
            isTransitioning = true;
            if (internalVolume > 0f)
            {
                while (timer < duration)
                {
                    internalVolume -= Time.deltaTime / duration;
                    source.volume = internalVolume * volumeMultiplier;
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            internalVolume = 0f;
            source.clip = audioClips[index];
            source.Play();
            timer = 0f;
            while (timer < duration)
            {
                internalVolume += Time.deltaTime / duration;
                source.volume = internalVolume * volumeMultiplier;
                timer += Time.deltaTime;
                yield return null;
            }
            isTransitioning = false;
        }
        else
        {
            yield return null;
        }
    }

    public IEnumerator GraduallyUnmute(float duration)
    {
        
        internalVolume = 0f;
        float timer = 0f;
        while (timer < duration)
        {
            internalVolume += Time.deltaTime / duration;
            source.volume = internalVolume * volumeMultiplier;
            timer += Time.deltaTime;
            yield return null;
        }
 
    }

    public IEnumerator GraduallyMute(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            internalVolume -= Time.deltaTime / duration;
            source.volume = internalVolume * volumeMultiplier;
            timer += Time.deltaTime;
            yield return null;
        }
        internalVolume = 0f;
    }

}
