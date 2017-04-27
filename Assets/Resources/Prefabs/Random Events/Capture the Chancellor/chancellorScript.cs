using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// New for Assessment 4 - Script to describe the action of a random event
/// Implements the capture the Chancellor system as per the revised client's requirements. See Implementation
/// report for details.
/// </summary>
public class chancellorScript : MonoBehaviour
{
    public int health = 3;                  //Number of clicks to capture the chancellor
    public int defaultWalkSpeed = 5;
    public int minTurnSpeed = 30;           //Speeds at which the chancellor may turn
    public int maxTurnSpeed = 100;
    public float timeBetweenClicks = 1;     //Cool down time to prevent spam clicking
    public int timeToCapture = 15;          //Time given to allow the player to capture the chancellor (seconds)
    public int phaseTimerAfterCapture = 3;  //Give the player this amount of time after the chancellor is captured before moving to the next phase.
    public GameObject hitEffect;            //GameObject effect to Instantiate when the chancellor hits a tile.

    public Collider hitRegion;              //Set in the Unity editor, the collider which detects Tile collisions when the Chancellor attacks a tile.

    private static ResourceGroup CHANCELLOR_CAPTURE_BONUS = new ResourceGroup(50, 25, 25);    //Resources given to the player when they succeed in capturing the Chancellor
    private static ResourceGroup CHANCELLOR_TILE_DAMAGE = new ResourceGroup(-1, -1, 0);       //Resource damage on the tiles hit by the Chancellor.
    private static int CHANCELLOR_DAMAGE_TURNS = 1;                                           //How long the tile resource damage should last for.
    private static int CHANCELLOR_LAYER = 9;                                                  //Layer of the chancellor GameObject. Used to mask a raycast to hit only the Chancellor.
    private static int LEFT_MOUSE_BUTTON = 0;
    private static int MIN_ATTACK_DELAY = 2;
    private static int MAX_ATTACK_DELAY = 5;

    private static string ANIM_STATE_RUNNING = "runnin";            //These strings should match the names of the respective clips from the 3D model
    private static string ANIM_STATE_LEFT_HIT = "left_arm_hit";     //Used to 
    private static string ANIM_STATE_RIGHT_HIT = "right_arm_hit";
    private static string ANIM_TRIGGER_LAND = "Hit Ground";
    private static string ANIM_TRIGGER_ATTACK = "Attack";
    private static string ANIM_TRIGGER_TAKE_HIT = "Take Hit";
    private static string ANIM_TRIGGER_DIE = "Die";

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
        hitRegion.GetComponent<Rigidbody>().isKinematic = true;     //In order to provide hit detection at a specific time, the hitRegion box is suspended above
                                                                    //the tiles in front of the chancellor, and dropped down when he attacks to detect which tiles are affected.
        damagePointScript damageScript = hitRegion.gameObject.AddComponent<damagePointScript>();
        damageScript.SetHitEffect(hitEffect);
        RandomEventEffect eventEffect = new RandomEventEffect(CHANCELLOR_TILE_DAMAGE, CHANCELLOR_DAMAGE_TURNS);
        damageScript.SetResourceEffects(eventEffect);

        hitRegionStartPosition = hitRegion.transform.localPosition; //Cache the position of the hitRegion so that it may be returned to the starting position between attacsks.

        StartCoroutine(DestroyAfterDelay());    //Clean up the event after it is finished.
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (walkSpeed > 0)
        {
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);  //Move forward by walkSpeed. Scaled to approximate current framerate by multiplying by the time between the last two frames.
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0);     //Turn by the current turn speed. Again scaled to framerate.
        }

        if (eventTimedOut)
        {
            return; //If event has timed out, only apply motion to allow the chancellor to exit the scene.
        }

		if(health <= 0 && !isDead)      //Chancellor has just been "captured"
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_DIE);  //Trigger death animation.
            GetComponent<BoxCollider>().enabled = false;            
            StartCoroutine(RemoveAnimatorAfterDone());              //Removes the Animator to prevent aliasing if another Chancellor event is created before this dead chancellor object is removed.
                                                                    //Prevents this chancellor responding to animation triggers given to the new one due to Animators being static
            Player currentPlayer = GameHandler.GetGameManager().GetCurrentPlayer();
            currentPlayer.SetResources(currentPlayer.GetResources() + CHANCELLOR_CAPTURE_BONUS);    //Give the current player the bonus resources.
            HumanGui humanGui = GameHandler.GetGameManager().GetHumanGui();
            humanGui.UpdateResourceBar(aiTurn: false);  
            humanGui.GetCanvas().SetPhaseTimeout(new Timeout(phaseTimerAfterCapture));   //Set the phase timer to phaseTimerAfterCapture seconds after the chancellor has been "captured"
        }
        
        if(Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON) && canClick)
        {
            CheckPlayerClick();
        }

        CheckAnimationState();
    }

    private void CheckAnimationState()
    {
        string currentState = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;  //Gets the name of the current clip being played by the Animator.

        if(currentState == ANIM_STATE_RUNNING)
        {
            walkSpeed = defaultWalkSpeed;

            if(!attackFlagSet)      //attackFlag is set after an attack has finished. Start another attack after a delay.
            {
                attackFlagSet = true;
                StartCoroutine(AttackAfterDelay());
            }
        }
        else
        {
            walkSpeed = 0;      //If not running, do not move the Chancellor in the game world.
        }

        if(currentState == ANIM_STATE_LEFT_HIT
            || currentState == ANIM_STATE_RIGHT_HIT)   
        {
            hitRegion.enabled = true;
            hitRegion.GetComponent<Rigidbody>().isKinematic = false;    //Allow the hitRegion box to collide with the tiles to be affected.
        }
        else
        {
            hitRegion.enabled = false;                                  //If not attacking, reset the hitRegion.
            hitRegion.GetComponent<Rigidbody>().isKinematic = true;     
            hitRegion.transform.localPosition = hitRegionStartPosition;
            hitRegion.transform.localRotation = Quaternion.identity;
        }
    }

    private void CheckPlayerClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int mask = 1 << CHANCELLOR_LAYER;     //Create a bitmask to apply to the Raycast so that it only detects hits against the chancellor GameObject.

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_TAKE_HIT); //Play the take hit animation
            health--;
            canClick = false;
            StartCoroutine(DelayNextClick());
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (!hasLanded && other.tag == TagManager.mapTile)  //Other collider belongs to a mapTile
        {
            hasLanded = true;
            Destroy(GetComponent<Rigidbody>());             //Rigibody not needed after landing - only controls physics.
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_LAND);
            GetComponent<AudioSource>().Play();             //Play landing crash sound
        }
    }

    private IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(MIN_ATTACK_DELAY, MAX_ATTACK_DELAY));

        if (!eventTimedOut)     //If the event has timed out already, do not play the attack animation.
        {
            GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_ATTACK);
            attackFlagSet = false;
            turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
        }
    }

    /// <summary>
    /// Prevents the player simply spam clicking and instantly killing the chancellor.
    /// </summary>
    /// <returns></returns>
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
            Destroy(transform.parent.gameObject, 30);   //Chancellor was "captured", "dispose" of it after some time.
        }
    }
    
    private IEnumerator RemoveAnimatorAfterDone()
    {
        yield return new WaitForSeconds(phaseTimerAfterCapture);
        GetComponent<Animator>().enabled = false;
    }
}
