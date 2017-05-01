using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assessment 4: Added to create a fade-to-black transition.
/// </summary>
public class fadeToBlackScript : MonoBehaviour
{
    public float fadeSpeed = 1;

    private Material fadeMaterial;
    private bool doFade = false;

	// Use this for initialization
	void Start ()
    {
        fadeMaterial = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(doFade)
        {
            Color newColour = fadeMaterial.color;
            newColour.a += Time.deltaTime * fadeSpeed;

            if(newColour.a <= 0)
            {
                doFade = false;     //Fade done, stop.
            }

            fadeMaterial.color = newColour; 
        }
	}

    public void BeginFadeToBlack()
    {
        doFade = true;
    }
}
