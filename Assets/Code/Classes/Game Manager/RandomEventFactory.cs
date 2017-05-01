//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System.Collections;
using System;

public class RandomEventFactory
{
    private static float RANDOM_EVENT_PROBABILITY = 0.5f;

    public GameObject Create(RandomEventStore randomEventStore)
    {
        if (UnityEngine.Random.Range(0, 1.0f) <= RANDOM_EVENT_PROBABILITY) //Chance that a random event takes place.
        {
            return randomEventStore.chooseEvent();
        }
        else
        {
            return null;    // Return null indicating no event should take place
        }
    }
}