using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class godzillaScript : MonoBehaviour
{
    public GameObject hitEffect;
    public List<BoxCollider> damagePointColliders = new List<BoxCollider>();

    public float defaultWalkSpeed = 1;
    public float walkStopFalloff = 1;

    private float walkSpeed;

    private static string TRIGGER_BREAK_THINGS = "breakThings";
    private static string TRIGGER_WALK = "startWalking";
    private static float WALK_ANIM_LENGTH = 3.25f;
    private static float BREAK_THINGS_ANIM_LENGTH = 22.3f;
    private static int MIN_WALK_CYCLES = 2;
    private static int MAX_WALK_CYCLES = 3;
    private static int WALK_AWAY_TIMEOUT = 15;
    private static ResourceGroup tileHitEffect = new ResourceGroup(-1, -1, -1);

    private bool breakingThings = false;

	// Use this for initialization
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
	
	// Update is called once per frame
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

        transform.Translate(new Vector3(0, 0, -walkSpeed * Time.deltaTime), Space.Self);
	}

    private IEnumerator DelayBreakThings()
    {
        yield return new WaitForSeconds(Random.Range(MIN_WALK_CYCLES, MAX_WALK_CYCLES) * WALK_ANIM_LENGTH);
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
