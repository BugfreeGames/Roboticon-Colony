//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assessment 4:- Used in random events to play audio clips at specific times.
/// </summary>
public class playAudioAfterDelay : MonoBehaviour
{
    public float delay = 1;

	// Use this for initialization
	void Start ()
    {
        try
        {
            GetComponent<AudioSource>().PlayDelayed(delay);
        }
        catch (System.NullReferenceException)
        {
            throw new System.NullReferenceException("No AudioSource attached to playAudioAfterDelay script.");
        }
	}
}
