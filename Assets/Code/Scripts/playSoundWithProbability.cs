using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assessment 4:- Used to reduce the frequency of sound clips for events which have a lot of tile effects.
/// </summary>
public class playSoundWithProbability : MonoBehaviour
{
    public float probability = 0.5f;

	// Use this for initialization
	void Start ()
    {
		if(Random.Range(0, 1.0f) < probability)
        {
            GetComponent<AudioSource>().Play();
        }
	}
}
