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

    private bool breakingThings = false;

	// Use this for initialization
	void Start ()
    {
        foreach (BoxCollider collider in damagePointColliders)
        {
            damagePointScript damagePointScript = collider.gameObject.AddComponent<damagePointScript>();
            damagePointScript.SetResourceEffects(new RandomEventEffect(new ResourceGroup(-1, -1, -1), 0));
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
        yield return new WaitForSeconds(Random.Range(3, 5) * 3.25f);
        breakingThings = true;
        GetComponent<Animator>().SetTrigger(TRIGGER_BREAK_THINGS);
        GetComponent<AudioSource>().PlayDelayed(3.2f);
        StartCoroutine(WalkAway());
    }

    private IEnumerator WalkAway()
    {
        yield return new WaitForSeconds(10);
        GetComponent<Animator>().SetTrigger(TRIGGER_WALK);
        yield return new WaitForSeconds(12.3f);
        walkSpeed = defaultWalkSpeed;
        yield return new WaitForSeconds(20);
        Destroy(transform.parent.gameObject);
    }
}
