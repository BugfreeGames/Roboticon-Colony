﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// New for Assessment 4 - Script to describe the action of a random event
/// </summary>
public class handScript : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject bulletGameObject;
    public BoxCollider damageRegion;

    private static string ANIM_SHOOT_CLIP_NAME = "shoot";
    private static ResourceGroup HIT_RESOURCE_EFFECT = new ResourceGroup(-1, -2, -1);
    private static int HIT_EFFECT_TURNS = 2;
    private static int EVENT_TIMEOUT = 15;
    private static float SHOOT_ANIM_BULLET_FIRE_TIME = 1.6f;
    private static bool doDestroyRoboticons = true;

    private bool bulletFired = false;

	// Use this for initialization
	void Start ()
    {
        damagePointScript damageScript = damageRegion.gameObject.AddComponent<damagePointScript>();
        damageScript.SetResourceEffects(new RandomEventEffect(HIT_RESOURCE_EFFECT, HIT_EFFECT_TURNS, doDestroyRoboticons));
        damageScript.SetHitEffect(hitEffect);

        transform.parent.Rotate(new Vector3(0, Random.Range(0, 360.0f), 0));
        Destroy(transform.parent.gameObject, EVENT_TIMEOUT);
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckAnimationState();
    }

    void CheckAnimationState()
    {
        string currentState = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;  //Gets the name of the current clip playing in the Animator.

        if(currentState == ANIM_SHOOT_CLIP_NAME)
        {
            damageRegion.enabled = false;       //Do not detect tile collisions when the shoot animation is playing as no contact should be made with tiles.

            if(!bulletFired)
            {
                bulletFired = true;
                StartCoroutine(DelayBulletFire());  //Used to ensure that the bullet is fired at the correct point in the animation.
            }
        }
        else
        {
            damageRegion.enabled = true;
        }
    }

    private IEnumerator DelayBulletFire()
    {
        yield return new WaitForSeconds(SHOOT_ANIM_BULLET_FIRE_TIME);
        bulletGameObject.SetActive(true);
        bulletGameObject.transform.parent = null;
    }
}