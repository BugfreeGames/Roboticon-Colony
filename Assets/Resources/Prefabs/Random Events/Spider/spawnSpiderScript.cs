using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnSpiderScript : MonoBehaviour
{
    public float spiderSpawnHeight = 50;
    public float spiderSpawnRadius = 5;
    public GameObject spiderGameObject;

	// Use this for initialization
	void Start ()
    {
        float theta = Random.Range(0, 360.0f);
        Vector3 spiderPos = new Vector3(spiderSpawnRadius * Mathf.Cos(theta), spiderSpawnHeight, spiderSpawnRadius * Mathf.Sin(theta));
        GameObject.Instantiate(spiderGameObject, spiderPos, Quaternion.identity, transform).transform.LookAt(Vector3.zero + Vector3.up * spiderSpawnHeight);
	}
}
