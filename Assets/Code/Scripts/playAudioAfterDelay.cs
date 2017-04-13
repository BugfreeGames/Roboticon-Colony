using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
