//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System.Collections;

public class lookAtTarget : MonoBehaviour 
{
    public Transform target;
	
	// Update is called once per frame
	void Update () 
    {
        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
