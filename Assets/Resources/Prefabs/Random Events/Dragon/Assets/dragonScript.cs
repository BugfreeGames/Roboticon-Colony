using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonScript : MonoBehaviour
{
    public BoxCollider fireHitRegion;
    public BoxCollider landingHitRegion;

    public GameObject landingHitEffect;
    public GameObject fireAfterGlowEffect;

    private static ResourceGroup TILE_FIRE_EFFECT = new ResourceGroup(-1, -1, -1);
    private static int TLE_FIRE_TURNS = 1;

    private static ResourceGroup TILE_LANDING_EFFECT = new ResourceGroup(-3, -3, -3);
    private static int TLE_LANDING_TURNS = 3;

    public float fireStartTime = 11.2f; //Time after animation start when the fire particles begin. Match with delay in particle systems.

    // Use this for initialization
    void Start()
    {
        transform.parent.Rotate(0, Random.Range(0, 360), 0);

        damagePointScript fireHitDamagePointScript = fireHitRegion.gameObject.AddComponent<damagePointScript>();
        RandomEventEffect fireDamageEffect = new RandomEventEffect(TILE_FIRE_EFFECT, TLE_FIRE_TURNS);
        fireDamageEffect.SetVisualEffect(fireAfterGlowEffect);
        fireHitDamagePointScript.SetResourceEffects(fireDamageEffect);
        fireHitDamagePointScript.SetHitStartDelay(fireStartTime);
        fireHitDamagePointScript.SetHitDelay(0);
        fireHitDamagePointScript.SetDoActivateRigidbody(true);

        damagePointScript landingDamagePointScript = landingHitRegion.gameObject.AddComponent<damagePointScript>();
        landingDamagePointScript.SetResourceEffects(new RandomEventEffect(TILE_LANDING_EFFECT, TLE_LANDING_TURNS));
        landingDamagePointScript.SetHitEffect(landingHitEffect);
        landingDamagePointScript.SetHitDelay(0);
        landingDamagePointScript.SetOneShotEffect(true);
        landingDamagePointScript.SetHitStartDelay(Time.deltaTime * 2);

        fireHitRegion.enabled = false;
        landingHitRegion.enabled = false;

        Destroy(transform.parent.gameObject, 30);
    }
}
