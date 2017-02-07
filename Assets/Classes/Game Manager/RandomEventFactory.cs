﻿// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System.Collections;
using System;

public class RandomEventFactory
{
    private RandomEventStore randomEventStore = new RandomEventStore();

    public GameObject Create(bool crazy)
    {
        if (UnityEngine.Random.Range(0, 2) == 0)           //TODO correct percentage chance of event occuring - 50% currently
        {
            return randomEventStore.chooseEvent(crazy);    
        }
        else
        {
            return null;    // Return null indicating no event should take place
        }
    }
}
