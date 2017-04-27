using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handBulletScript : MonoBehaviour
{
    public float bulletSpeed = 1;
    public GameObject hitEffect;

    public static ResourceGroup HIT_RESOURCE_EFFECT = new ResourceGroup(-50, -50, -50);
    public static int HIT_TURNS_EFFECT = 0; //0 -> permanent damage to tile
    public static int bulletLifetime = 15;  //Time in seconds before bullet object is destroyed
    
	void Start ()
    {
        damagePointScript damageScript = gameObject.AddComponent<damagePointScript>();
        damageScript.SetResourceEffects(new RandomEventEffect(HIT_RESOURCE_EFFECT, HIT_TURNS_EFFECT));
        damageScript.SetHitEffect(hitEffect);
        damageScript.SetDoDestroyAfterHit(true);

        Destroy(gameObject, bulletLifetime); //Destroy after timeout in case no tile is hit.
    }
	
	void Update ()
    {
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime, Space.Self);    //Move the bullet forward each frame (frame-rate adjusted)
	}
}
