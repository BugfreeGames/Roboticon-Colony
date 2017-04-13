// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System.Collections;

public class RandomEvent
{
    private GameObject eventGameObject;
    private string eventTitle;
    private string eventDescription;

    public bool isNullEvent = false;

    public RandomEvent(RandomEventStore randomEventStore)
    {
        eventGameObject = new RandomEventFactory().Create(randomEventStore);

        if (eventGameObject != null)
        {
            EventInfo eventInfo = eventGameObject.GetComponent<EventInfo>();

            eventTitle = eventInfo.title;
            eventDescription = eventInfo.description;
        }
        else
        {
            isNullEvent = true;
        }
    }

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
}