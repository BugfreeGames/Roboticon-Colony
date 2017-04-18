using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chancellorScript : MonoBehaviour
{
    public int health = 3;
    public int defaultWalkSpeed = 5;
    public int minTurnSpeed = 30;
    public int maxTurnSpeed = 100;
    public float timeBetweenClicks = 1;
    public int timeToCapture = 15;  //Time given to allow the player to capture the chancellor
    public GameObject hitEffect;

    public Collider hitRegion;

    private ResourceGroup chancellorCaptureBonus = new ResourceGroup(50, 25, 25);

    private static string ANIM_STATE_RUNNING = "runnin";
    private static string ANIM_STATE_LEFT_HIT = "left_arm_hit";
    private static string ANIM_STATE_RIGHT_HIT = "right_arm_hit";

    private static string ANIM_TRIGGER_LAND = "Hit Ground";
    private static string ANIM_TRIGGER_ATTACK = "Attack";
    private static string ANIM_TRIGGER_TAKE_HIT = "Take Hit";
    private static string ANIM_TRIGGER_DIE = "Die";
    private static int LEFT_MOUSE_BUTTON = 0;

    private bool isDead = false;

    private int walkSpeed = 0;
    private int turnSpeed;

    private bool attackFlagSet = false;
    private bool canClick = true;
    private bool eventTimedOut = false;
    private bool hasLanded = false;

    private Vector3 hitRegionStartPosition;

    // Use this for initialization
    void Start ()
    {
        turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);

        hitRegion.enabled = false;
        hitRegion.GetComponent<Rigidbody>().isKinematic = true;

        damagePointScript damageScript = hitRegion.gameObject.AddComponent<damagePointScript>();
        damageScript.SetHitEffect(hitEffect);
        RandomEventEffect eventEffect = new RandomEventEffect(new ResourceGroup(-1, -1, 0), 1);
        damageScript.SetResourceEffects(eventEffect);

        hitRegionStartPosition = hitRegion.transform.localPosition;

        StartCoroutine(DestroyAfterDelay());
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (walkSpeed > 0)
        {
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
        }

        if (eventTimedOut)
        {
            return; //If event has timed out, only apply motion to allow the chancellor to exit the scene.
        }

		if(health <= 0 && !isDead)
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_DIE);
            GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(RemoveAnimatorAfterDone());

            Player currentPlayer = GameHandler.GetGameManager().GetCurrentPlayer();
            currentPlayer.SetResources(currentPlayer.GetResources() + chancellorCaptureBonus);
            HumanGui humanGui = GameHandler.GetGameManager().GetHumanGui();
            humanGui.UpdateResourceBar(false);
            humanGui.GetCanvas().SetPhaseTimeout(new Timeout(3));   //Set the timer to 3 seconds after the chancellor has been "captured"
        }
        
        if(Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON) && canClick)
        {
            CheckPlayerClick();
        }

        CheckAnimationState();
    }

    private void CheckAnimationState()
    {
        string currentState = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if(currentState == ANIM_STATE_RUNNING)
        {
            walkSpeed = defaultWalkSpeed;

            if(!attackFlagSet)
            {
                hitRegion.enabled = false;
                hitRegion.GetComponent<Rigidbody>().isKinematic = true;

                attackFlagSet = true;
                StartCoroutine(AttackAfterDelay());
            }
        }
        else
        {
            walkSpeed = 0;
        }

        if(currentState == ANIM_STATE_LEFT_HIT
            || currentState == ANIM_STATE_RIGHT_HIT)
        {
            hitRegion.enabled = true;
            hitRegion.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            hitRegion.enabled = false;
            hitRegion.GetComponent<Rigidbody>().isKinematic = true;
            hitRegion.transform.localPosition = hitRegionStartPosition;
            hitRegion.transform.localRotation = Quaternion.identity;
        }
    }

    private void CheckPlayerClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int mask = 1 << 9;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_TAKE_HIT);
            health--;
            canClick = false;
            StartCoroutine(DelayNextClick());
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (!hasLanded && other.tag == TagManager.mapTile)
        {
            hasLanded = true;
            Destroy(GetComponent<Rigidbody>());
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_LAND);
            GetComponent<AudioSource>().Play();
        }
    }

    private IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(2, 5));

        if (!eventTimedOut)
        {
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_ATTACK);
            attackFlagSet = false;
            turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
        }
    }

    private IEnumerator DelayNextClick()
    {
        yield return new WaitForSeconds(timeBetweenClicks);
        canClick = true;
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(timeToCapture);
        eventTimedOut = true;

        if (!isDead)        //The event has timed out and the player has not been successful. Chancellor exits scene.
        {
            turnSpeed = 0;
            walkSpeed = 40;
            GetComponent<Animator>().speed = 4;
            Destroy(transform.parent.gameObject, 5);    //Destroy event after chancellor has left
        }
        else
        {
            Destroy(transform.parent.gameObject, 30);   //Chancellor was "captured", dispose of it after some time.
        }
    }
    
    private IEnumerator RemoveAnimatorAfterDone()
    {
        yield return new WaitForSeconds(3);
        GetComponent<Animator>().enabled = false;
    }
}
