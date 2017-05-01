//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assessment 4 - Reverted and built upon random event system to original BugFree system from Assessment 2 
/// </summary>
public class RandomEventStore
{
    private static string randomEventGameObjectsPath = "Prefabs/Random Events/Event GameObjects"; //Assumes all assets in this folder are GameObjects.
    public List<GameObject> randomEvents = new List<GameObject>();   

    int numEvents;

    public RandomEventStore()
    {
        System.Object[] loadedObjects = Resources.LoadAll(randomEventGameObjectsPath);

        for(int i = 0; i < loadedObjects.Length; i ++)
        {
            randomEvents.Add((GameObject)loadedObjects[i]);
        }

        numEvents = randomEvents.Count;

        foreach (GameObject randomEvent in randomEvents)
        {
            if(randomEvent.GetComponent<EventInfo>() == null)
            { 
                throw new System.NullReferenceException("No EventInfo script found on random event GameObject. Add EventInfo to provide an event title and description.");
            }
        }
    }

    public GameObject chooseEvent()
    {
        if (numEvents == 0)
        {
            Debug.LogWarning("No random events to instantiate.");
            return null;
        }
        
        return this.randomEvents[UnityEngine.Random.Range(0, numEvents)];    // Choose a random event from the randomEvents list
    }
}
