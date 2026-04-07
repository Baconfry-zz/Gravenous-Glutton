using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : MonoBehaviour
{
    [SerializeField] private DigitCounter preyCurrentCounter;
    [SerializeField] private DigitCounter preyMaxCounter;
    [SerializeField] private GameObject preyToSpawn;

    [SerializeField] private Transform spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPrey()
    {
        Instantiate(preyToSpawn, spawnLocation.position, Quaternion.identity);
    }

    public void UpdateValues(int upper, int lower)
    {
        preyCurrentCounter.SetCounterTo(upper);
        preyMaxCounter.SetCounterTo(lower);
    }
}
