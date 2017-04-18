using UnityEngine;
using System.Collections;

public class planetRotateScript : MonoBehaviour 
{
    public float speed = 1;
    public bool generateRandomSpeed = false;
	
    void Start()
    {
        if(generateRandomSpeed)
        {
            speed = Random.Range(0.2f, 1);
        }
    }

	// Update is called once per frame
	void Update () 
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
	}
}
