using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderScript : MonoBehaviour
{
    public float spiderDeathEffectRadius = 10;
    public float defaultWalkSpeed = 10;
    public AudioSource spiderDieAudioSource;
    public List<BoxCollider> legColliderHitPoints = new List<BoxCollider>();

    private static string ANIM_WALK_TRIGGER = "Walk";
    private static string ANIM_ATTACK_TRIGGER = "Attack";
    private static string ANIM_DIE_1_TRIGGER = "Die1";
    private static string ANIM_DIE_2_TRIGGER = "Die2";
    private static string ANIM_NAME_WALK = "run_ani_vor";
    private static string ANIM_NAME_DIE_1 = "die";
    private static string ANIM_NAME_DIE_2 = "die_2";

    Animator animator;

    float walkSpeed = 0;
    bool doAddDamageScripts = false;
    bool damageScriptsAdded = false;
    bool deathEffectApplied = false;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();

        DisableHitPointColliders();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(walkSpeed > 0)
        {
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        }

        CheckCurrentAnimStateForAction();

        if (doAddDamageScripts)
        {
            doAddDamageScripts = false;

            foreach (BoxCollider col in legColliderHitPoints)
            {
                damagePointScript damageScript = col.gameObject.AddComponent<damagePointScript>();
                damageScript.SetResourceEffects(new RandomEventEffect(new ResourceGroup(-1, -1, -1), 3));
            }

            damageScriptsAdded = true;
        }
	}

    private void CheckCurrentAnimStateForAction()
    {
        string currentClipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if (currentClipName == ANIM_NAME_WALK)
        {
            walkSpeed = defaultWalkSpeed;
        }
        else
        {
            walkSpeed = 0;
        }

        if (!damageScriptsAdded && currentClipName == ANIM_ATTACK_TRIGGER)
        {
            doAddDamageScripts = true;
            GetComponent<AudioSource>().Play();
        }

        if (currentClipName == ANIM_NAME_DIE_1
            || currentClipName == ANIM_NAME_DIE_2)
        {
            DisableHitPointColliders();

            if (!deathEffectApplied)
            {
                deathEffectApplied = true;
                ApplyDeadSpiderEffect();
            }
        }
    }

    private void ApplyDeadSpiderEffect()
    {
        spiderDieAudioSource.Play();

        Collider[] hitObjects = Physics.OverlapSphere(transform.position, spiderDeathEffectRadius);

        foreach(Collider col in hitObjects)
        {
            if(col.tag == TagManager.mapTile)
            {
                Tile tile = GameHandler.GetGameManager().GetMap().GetTile(col.GetComponent<mapTileScript>().GetTileId());
                RandomEventEffect deathEffect = new RandomEventEffect(new ResourceGroup(10, -3, -3), 3);
                deathEffect.SetVisualEffectInWorld(gameObject);
                tile.ApplyEventEffect(deathEffect);
            }
        }
    }

    private void DisableHitPointColliders()
    {
        foreach (BoxCollider col in legColliderHitPoints)
        {
            col.enabled = false;
        }
    }

    /// <summary>
    /// Spider has dropped onto a trigger tile
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<BoxCollider>().enabled = false;
        Destroy(GetComponent<Rigidbody>());

        Vector3 zeroxzRotation = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(zeroxzRotation);

        Vector3 zeroyPosition = new Vector3(transform.position.x, 0, transform.position.z);
        transform.position = zeroyPosition;

        animator.SetTrigger(ANIM_WALK_TRIGGER);
        StartCoroutine(DelaySpiderAttack());
    }

    private IEnumerator DelaySpiderAttack()
    {
        yield return new WaitForSeconds(0.625f * Random.Range(1, 3.0f));
        animator.SetTrigger(ANIM_ATTACK_TRIGGER);
        StartCoroutine(DelaySpiderDie());
    }

    private IEnumerator DelaySpiderDie()
    {
        yield return new WaitForSeconds(1.125f * Random.Range(1, 3.0f));
        if (Random.Range(0, 2) == 0)
        {
            animator.SetTrigger(ANIM_DIE_1_TRIGGER);
        }
        else
        {
            animator.SetTrigger(ANIM_DIE_2_TRIGGER);
        }
    }
}
