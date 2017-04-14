using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnScorpionsScript : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.5f;
    public int maxScorpions = 10;
    public int minScorpions = 2;
    public int maxRadius = 10;
    public GameObject scorpionGameObject;

	// Use this for initialization
	void Start ()
    {
        int numScorpions = Random.Range(minScorpions, maxScorpions);

        for(int i = 0; i < numScorpions; i ++)
        {
            float theta = Random.Range(0, 360.0f);
            float radius = Random.Range(2, maxRadius);
            Vector3 scorpionPosition = new Vector3( radius * Mathf.Cos(theta), 0, radius * Mathf.Sin(theta));
            GameObject scorpion = (GameObject)GameObject.Instantiate(scorpionGameObject, scorpionPosition, Quaternion.LookRotation(scorpionPosition), transform);
            scorpion.transform.localScale *= Random.Range(minSize, maxSize);
            scorpion.GetComponent<scorpionScript>().startDelay = Random.Range(0, 1.0f);
        }
	}
}
