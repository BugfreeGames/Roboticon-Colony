﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by JBT to rotate a direction light in the scene
/// <summary>
/// Attached to main manu light, Used to rotate the light at constant speed to imitate a day night cycle
/// </summary>
public class mainMenuDayNightCylce : MonoBehaviour {
    public Light sun;
	
	// Update is called once per frame
	void Update () {
        //Rotate the light
        sun.transform.Rotate(new Vector3(0.5f, 0.0f));
    }
}
