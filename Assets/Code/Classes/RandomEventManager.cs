//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip
// This file was created by JBT

using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

/// <summary>
/// Handles game-wide manipulation of RandomEvents
/// </summary>
public static class RandomEventManager
{
    private static System.Random Random = new System.Random();

    private static List<RandomEvent> InactiveEvents = new List<RandomEvent>();

    private static List<RandomEvent> ActiveEvents = new List<RandomEvent>();

    /// <summary>
    /// Number between 0 and 1 (exclusive) for how often a random event should happen 
    /// </summary>
    public static float EventFrequency { get; private set; }
    public const float DefaultEventFrequency = 0.999f;
    
    /// <summary>
    /// Maximum number of events that can be active at the same time
    /// min = 1
    /// </summary>
    public static int MaxSimultaneousEvents { get; private set; }
    public const int DefaultMaxSimultaneousEvents = 2;
    
    /// <summary>
    /// Initialise the randomevents available in the game
    /// </summary>
    public static void InitialiseEvents()
    {
        if (InactiveEvents.Count + ActiveEvents.Count > 0)
        {
            Debug.Log("Events have already been initialised");
        }

        MaxSimultaneousEvents = DefaultMaxSimultaneousEvents;
        EventFrequency = DefaultEventFrequency;

        TextAsset LoadedJson = (TextAsset)Resources.Load("Events");
        var ConfigurationsFile = JSON.Parse(LoadedJson.text);
        for (int i = 0; i < ConfigurationsFile["configurations"].Count; i++)
        {
            var EventConfiguration = ConfigurationsFile["configurations"][i];
            InactiveEvents.Add(
                new RandomEvent(
                    EventConfiguration["title"].Value,
                    EventConfiguration["description"].Value,
                    EventConfiguration["duration"].AsInt,
                    EventConfiguration["numberOfTilesToAffect"].AsInt,
                    EventConfiguration["connectedTilesOnly"].AsBool,
                    EventConfiguration["roboticonInstalled"].AsBool,
                    EventConfiguration["noRoboticonInstalled"].AsBool,
                    EventConfiguration["foodMultiplier"].AsFloat,
                    EventConfiguration["energyMultiplier"].AsFloat,
                    EventConfiguration["oreMultiplier"].AsFloat,
                    EventConfiguration["texture"].Value
                ));
        }
    }

    /// <summary>
    /// Manage current random events and trigger new ones
    /// Should only be called at the of both players production phase so that events are applied to both
    /// </summary>
    public static void ManageAndTriggerEvents()
    {
        Debug.Log("ManageAndTriggerEvents called");
        List<RandomEvent> EventsToRemoveFromActive = new List<RandomEvent>();
        // Update currently active events
        foreach (RandomEvent activeEvent in ActiveEvents)
        {
            if (activeEvent.Finished)
            {
                Debug.Log(String.Format("Removing event {0}", activeEvent.Title));
                // the event is finished, move it to the inactive event list
                EventsToRemoveFromActive.Add(activeEvent);
                InactiveEvents.Add(activeEvent);
                
                // TODO Check if we need to update the map
            }
            else
            {
                activeEvent.TurnCompleted();
            } 
        }

        // have to remove these events seperately as the above loop cant modify the list its iterating over
        foreach (RandomEvent eventToRemove in EventsToRemoveFromActive)
        {
            ActiveEvents.Remove(eventToRemove);
        }
        
        // Trigger new events
        if (ActiveEvents.Count < MaxSimultaneousEvents)
        {
            // 0 <= Random.NextDouble() < 1
            // so the higher EventFrequency is the more chance next double has of being < EventFrequency
            if (Random.NextDouble() < EventFrequency)
            {
                TryTriggerNewEvent();
            }
            else
            {
                Debug.Log("Not triggering a new event this turn");
            }
        }
    }

    /// <summary>
    /// Try to trigger a randomly selected event
    /// Might fail if the correct tiles cant be found but state wont be affected if that happens
    /// </summary>
    private static void TryTriggerNewEvent()
    {
        Debug.Log("Trying to trigger a new event");
        RandomEvent NewEvent = InactiveEvents[Random.Next(InactiveEvents.Count)];
        try {
            NewEvent.Start(GameHandler.GetGameManager().GetMap());
        }
        catch (InvalidOperationException)
        {
            Debug.Log("Failed to start the event this turn, continuing anyway");
            // the event couldnt be started, reset and continue without having started the event
            return;
        }
        InactiveEvents.Remove(NewEvent);
        ActiveEvents.Add(NewEvent);
        GameHandler.GetGameManager().GetHumanGui().GetCanvas().DisplayNewEventMessage(NewEvent);
    }
}
