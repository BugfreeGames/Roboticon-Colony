using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scorpionScript : MonoBehaviour
{
    public float defaultMoveSpeed = 5;
    public float runAttackSpeed = 8;
    public float runAttackTime = 1;
    public float startDelay = 0.5f;

    public List<BoxCollider> hitRegionColliders = new List<BoxCollider>();

    public GameObject poisonHitEffect;
    public GameObject poisonLastingEffect;

    private float moveSpeed = 0;
    private static string ANIM_STATE_WALK = "walk";
    private static string ANIM_STATE_RUN_ATTACK = "run to attack";

    private static string ANIM_TRIGGER_GROUND_ATTACK = "Attack Ground";
    private static string ANIM_TRIGGER_BIG_ATTACK = "Big Attack";
    private static string ANIM_TRIGGER_RUN_ATTACK = "Run Attack";

    bool setRunAttackSpeedOnce = false;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(DelayStart());
        gameObject.GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
		if(moveSpeed > 0)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
        }

        CheckAnimationState();
	}

    void CheckAnimationState()
    {
        string clipName = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if(clipName == ANIM_STATE_WALK)
        {
            moveSpeed = defaultMoveSpeed;
        }
        else if(clipName == ANIM_STATE_RUN_ATTACK && !setRunAttackSpeedOnce)
        {
            moveSpeed = runAttackSpeed;
            StartCoroutine(DelayStopRun());
        }
        else
        {
            moveSpeed = 0;
        }
    }

    private IEnumerator DelayStopRun()
    {
        yield return new WaitForSeconds(runAttackTime);
        setRunAttackSpeedOnce = true;
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(Random.Range(1, 6) * 0.8f);
        string triggerToSet;

        switch(Random.Range(0, 3))
        {
            case 0:
                triggerToSet = ANIM_TRIGGER_BIG_ATTACK;
                break;

            case 1:
                triggerToSet = ANIM_TRIGGER_GROUND_ATTACK;
                break;

            default:
                triggerToSet = ANIM_TRIGGER_RUN_ATTACK;

                if(Random.Range(0, 2) == 0)
                {
                    GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_GROUND_ATTACK);
                }
                else
                {
                    GetComponent<Animator>().SetTrigger(ANIM_TRIGGER_BIG_ATTACK);
                }

                break;
        }

        GetComponent<Animator>().SetTrigger(triggerToSet);
    }

    private IEnumerator DelayStart()
    {
        print("delaying start");
        yield return new WaitForSeconds(startDelay);
        print("delayed start");
        gameObject.GetComponent<Animator>().enabled = true;

        StartCoroutine(DelayAttack());

        foreach (BoxCollider collider in hitRegionColliders)
        {
            damagePointScript damageScript = collider.gameObject.AddComponent<damagePointScript>();
            RandomEventEffect effect = new RandomEventEffect(new ResourceGroup(-5, 0, 0), 2);
            effect.SetVisualEffect(poisonLastingEffect);
            damageScript.SetResourceEffects(effect);
            damageScript.SetHitEffect(poisonHitEffect);
        }
    }
}
