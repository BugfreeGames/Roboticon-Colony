using UnityEngine;
using System.Collections;

public class lookAtTarget : MonoBehaviour 
{

    public Transform target;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {

	    if(target != null)
            transform.LookAt(target);

    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
