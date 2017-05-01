//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System.Collections;

/// <summary>
/// Assessment 4 - Reverted and built upon original BugFree RandomEvent system from Assessment 2.
/// </summary>
public class RandomEvent
{
    private GameObject eventGameObject;
    private string eventTitle;
    private string eventDescription;
    private int eventTime;
    private Sprite eventImage;

    private static int DEFAULT_TIMEOUT_NO_EVENT = 3;

    public bool isNullEvent = false;

    /// <summary>
    /// Create a new RandomEvent. May create a null event depending on the probabilities in RandomEventStore.
    /// </summary>
    /// <param name="randomEventStore"></param>
    public RandomEvent(RandomEventStore randomEventStore)
    {
        eventGameObject = new RandomEventFactory().Create(randomEventStore);

        if (eventGameObject != null)
        {
            EventInfo eventInfo = eventGameObject.GetComponent<EventInfo>();

            eventTitle = eventInfo.title;
            eventDescription = eventInfo.description;
            eventImage = eventInfo.image;
            eventTime = eventInfo.eventTime;
        }
        else
        {
            isNullEvent = true;
        }
    }

    /// <summary>
    /// Instantiate the random event in the game world.
    /// </summary>
    public void Instantiate()
    {
        if(isNullEvent)
        {
            return;
        }

        GameObject.Instantiate(eventGameObject, Vector3.zero, Quaternion.identity);
    }

    public string getTitle()
    {
        if (isNullEvent)
        {
            return "NULL_EVENT";
        }

        return eventTitle;
    }

    public string getDescription()
    {
        if (isNullEvent)
        {
            return "NULL_EVENT";
        }

        return eventDescription;
    }

    public int getEventTime()
    {
        if(isNullEvent)
        {
            return DEFAULT_TIMEOUT_NO_EVENT;   
        }

        return eventTime;
    }

    public Sprite getImage()
    {
        if(isNullEvent)
        {
            return new Sprite();
        }

        return eventImage;
    }
}