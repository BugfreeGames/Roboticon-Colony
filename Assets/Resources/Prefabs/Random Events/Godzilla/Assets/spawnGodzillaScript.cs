using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnGodzillaScript : MonoBehaviour
{
    public GameObject godzillaGameObject;
    public float spawnRadiusFromOrigin;

	// Use this for initialization
	void Start ()
    {
        float theta = Random.Range(0, 360.0f);
        Vector3 spawnPos = new Vector3(spawnRadiusFromOrigin * Mathf.Cos(theta), 0, spawnRadiusFromOrigin * Mathf.Sin(theta));
        GameObject.Instantiate(godzillaGameObject, spawnPos, Quaternion.LookRotation(spawnPos), transform);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
