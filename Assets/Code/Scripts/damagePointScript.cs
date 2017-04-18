using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagePointScript : MonoBehaviour
{
    RandomEventEffect tileEffect;
    Collider damagePointCollider;
    GameObject hitEffect;
    float delayBetweenHits = 0.8f;
    float hitStartDelay = 0;

    bool readyToHit = true;
    bool oneShotEffect = false;
    bool doActivateRigidbody = false;
    bool doDestroyAfterHit = false;
    float rigidbodyTimeout = 5;

	// Use this for initialization
	void Start ()
    {
        damagePointCollider = GetComponent<Collider>();

        if(damagePointCollider == null)
        {
            throw new System.Exception("Damage Point Script attached to GameObject with no collider. This script needs a collider to work properly.");
        }

        if(hitStartDelay > 0)
        {
            StartCoroutine(SetColliderActiveAfterDelay());
            damagePointCollider.enabled = false;
        }
        else
        {
            damagePointCollider.enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    /// <summary>
    /// This is the effect that will be added to each tile within the hit collider at every hit.
    /// </summary>
    /// <param name="effect"></param>
    public void SetResourceEffects(RandomEventEffect effect)
    {
        tileEffect = effect;
    }

    /// <summary>
    /// This GameObject will be instantiated every time a hit is incurred
    /// </summary>
    /// <param name="effect"></param>
    public void SetHitEffect(GameObject effect)
    {
        hitEffect = effect;
    }
    
    /// <summary>
    /// Sets the minimum time delay between two successive hits.
    /// </summary>
    /// <param name="delay"></param>
    public void SetHitDelay(float delay)
    {
        delayBetweenHits = delay;
    }

    /// <summary>
    /// Sets the time after instantiation to wait before activating the damage point collider.
    /// </summary>
    /// <param name="delay"></param>
    public void SetHitStartDelay(float delay)
    {
        hitStartDelay = delay;
    }

    /// <summary>
    /// Set to true to have the hitEffect only ever go off one time, although damage may be incurred multiple times.
    /// </summary>
    public void SetOneShotEffect(bool isOneShot)
    {
        oneShotEffect = isOneShot;
    }

    /// <summary>
    /// Set to true to have the gameobject that this script is attached to be destroyed after the first hit.
    /// </summary>
    public void SetDoDestroyAfterHit(bool doDestroyAfterHit)
    {
        this.doDestroyAfterHit = doDestroyAfterHit;
    }

    /// <summary>
    /// Set to true to enable a rigidbody on the collider object for rigidbodyTimeout seconds.
    /// </summary>
    /// <param name="doActivateRigidbody"></param>
    public void SetDoActivateRigidbody(bool doActivateRigidbody)
    {
        this.doActivateRigidbody = doActivateRigidbody;
    }

    /// <summary>
    /// Used when DoActivateRigidbody is set to true. Enables a rigidbody for timeout seconds
    /// </summary>
    /// <param name="timeout"></param>
    public void SetRigidbodyTimeout(float timeout)
    {
        rigidbodyTimeout = timeout;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagManager.mapTile && readyToHit)
        {
            Tile hitTile = GameHandler.GetGameManager().GetMap().GetTile(other.gameObject.GetComponent<mapTileScript>().GetTileId());
            hitTile.ApplyEventEffect(tileEffect.Copy());

            if(hitEffect != null)
            {
                GameObject.Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            if(doDestroyAfterHit)
            {
                Destroy(gameObject);
            }

            if (oneShotEffect)
            {
                hitEffect = null;
            }

            if (delayBetweenHits > 0)
            {
                readyToHit = false;
                StartCoroutine(DelayNextHit());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    private IEnumerator DelayNextHit()
    {
        yield return new WaitForSeconds(delayBetweenHits);
        readyToHit = true;
    }

    private IEnumerator SetColliderActiveAfterDelay()
    {
        yield return new WaitForSeconds(hitStartDelay);
        damagePointCollider.enabled = true;

        if(doActivateRigidbody)
        {
            damagePointCollider.gameObject.AddComponent<Rigidbody>();
            StartCoroutine(RigidbodyTimeout());
        }
    }

    private IEnumerator RigidbodyTimeout()
    {
        yield return new WaitForSeconds(rigidbodyTimeout);
        Destroy(damagePointCollider.gameObject.GetComponent<Rigidbody>());
    }
}
