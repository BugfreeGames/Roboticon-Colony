using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonScript : MonoBehaviour
{
    public BoxCollider fireHitRegion;
    public BoxCollider landingHitRegion;

    public GameObject landingHitEffect;
    public GameObject fireAfterGlowEffect;

    public float fireStartTime = 11.2f; //Time after animation start when the fire particles begin. Match with delay in particle systems.

    // Use this for initialization
    void Start()
    {
        transform.parent.Rotate(0, Random.Range(0, 360), 0);

        damagePointScript fireHitDamagePointScript = fireHitRegion.gameObject.AddComponent<damagePointScript>();
        RandomEventEffect fireDamageEffect = new RandomEventEffect(new ResourceGroup(-1, -1, -1), 2);
        fireDamageEffect.SetVisualEffect(fireAfterGlowEffect);
        fireHitDamagePointScript.SetResourceEffects(fireDamageEffect);
        fireHitDamagePointScript.SetHitStartDelay(fireStartTime);
        fireHitDamagePointScript.SetHitDelay(0);
        fireHitDamagePointScript.SetDoActivateRigidbody(true);

        damagePointScript landingDamagePointScript = landingHitRegion.gameObject.AddComponent<damagePointScript>();
        landingDamagePointScript.SetResourceEffects(new RandomEventEffect(new ResourceGroup(-3, -3, -3), 3));
        landingDamagePointScript.SetHitEffect(landingHitEffect);
        landingDamagePointScript.SetHitDelay(0);
        landingDamagePointScript.SetOneShotEffect(true);
        landingDamagePointScript.SetHitStartDelay(Time.deltaTime * 2);  //Animation takes 2 frames to start

        fireHitRegion.enabled = false;
        landingHitRegion.enabled = false;

        Destroy(transform.parent.gameObject, 30);
    }
}
