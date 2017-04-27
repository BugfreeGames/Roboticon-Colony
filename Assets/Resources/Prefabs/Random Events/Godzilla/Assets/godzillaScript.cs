using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// New for Assessment 4 - Script to describe the action of a random event
/// </summary>
public class godzillaScript : MonoBehaviour
{
    public GameObject hitEffect;
    public List<BoxCollider> damagePointColliders = new List<BoxCollider>();

    public float defaultWalkSpeed = 1;
    public float walkStopFalloff = 1;

    private float walkSpeed;

    private static string TRIGGER_BREAK_THINGS = "breakThings";
    private static string TRIGGER_WALK = "startWalking";
    private static float WALK_ANIM_LENGTH = 3.25f;      //ANIM_LENGTH s should match the length in seconds of the animation clips used.
    private static float BREAK_THINGS_ANIM_LENGTH = 22.3f;
    private static int MIN_WALK_CYCLES = 2;
    private static int MAX_WALK_CYCLES = 3;
    private static int WALK_AWAY_TIMEOUT = 15;          //Time after which to destroy Godzilla after it starts walking away
    private static ResourceGroup tileHitEffect = new ResourceGroup(-1, -1, -1);

    private bool breakingThings = false;
    
	void Start ()
    {
        foreach (BoxCollider collider in damagePointColliders)
        {
            damagePointScript damagePointScript = collider.gameObject.AddComponent<damagePointScript>();
            damagePointScript.SetResourceEffects(new RandomEventEffect(tileHitEffect, 0));  //0 turn effect = permanent
            damagePointScript.SetHitEffect(hitEffect);
        }

        walkSpeed = defaultWalkSpeed;

        StartCoroutine(DelayBreakThings());
	}
	
	void Update ()
    {
        if (breakingThings)
        {
            walkSpeed -= Time.deltaTime * walkStopFalloff;

            if(walkSpeed < 0)
            {
                walkSpeed = 0;
                breakingThings = false;
            }
        }

        transform.Translate(new Vector3(0, 0, -walkSpeed * Time.deltaTime), Space.Self);    //Walk forwards
	}

    private IEnumerator DelayBreakThings()
    {
        yield return new WaitForSeconds(Random.Range(MIN_WALK_CYCLES, MAX_WALK_CYCLES) * WALK_ANIM_LENGTH); //Pick a random amount of walk cycles to perform before beginning the breakThings animation
        breakingThings = true;
        GetComponent<Animator>().SetTrigger(TRIGGER_BREAK_THINGS);
        GetComponent<AudioSource>().PlayDelayed(WALK_ANIM_LENGTH);
        StartCoroutine(WalkAway());
    }

    private IEnumerator WalkAway()
    {
        yield return new WaitForSeconds(BREAK_THINGS_ANIM_LENGTH / 2);
        GetComponent<Animator>().SetTrigger(TRIGGER_WALK);
        yield return new WaitForSeconds(BREAK_THINGS_ANIM_LENGTH / 2);
        walkSpeed = defaultWalkSpeed;
        yield return new WaitForSeconds(WALK_AWAY_TIMEOUT);
        Destroy(transform.parent.gameObject);
    }
}
