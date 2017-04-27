using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// New for Assessment 4 - Script to describe the action of a random event
/// </summary>
public class meteorScript : MonoBehaviour
{
    public float detectRadius = 2.4f;
    public float blastRadius = 5;
    public GameObject explosionGameObject;
    public GameObject meteorDestroyedGameObject;
    public GameObject meteorModelGameObject;

    private static ResourceGroup METEOR_STRIKE_EFFECT = new ResourceGroup(-50,-50, 13);
    private static int METEOR_STRIKE_TURNS = 3;

	// Use this for initialization
	void Start ()
    {
        GetComponent<Rigidbody>().AddForce(Random.Range(-300, 300), 0, Random.Range(-300, 300));    //Randomises the hit area of the meteor by applying some horizontal force.
	}
	
	// Update is called once per frame
	void Update ()
    {
        Collider[] detectedColliders = Physics.OverlapSphere(transform.position, detectRadius);
        if (detectedColliders.Length > 0) //Detected colliders within detect radius. Meteor explodes.
        {
            bool onlyHitSelf = true; 

            foreach(Collider collider in detectedColliders)
            {
                if(collider.gameObject != gameObject)
                {
                    onlyHitSelf = false;
                }
            }

            if(onlyHitSelf)
            {
                return;
            }

            Collider[] damagedTiles = Physics.OverlapSphere(transform.position, blastRadius);

            foreach(Collider collider in damagedTiles)
            {
                if(collider.tag == TagManager.mapTile)
                {
                    Tile hitTile = GameHandler.GetGameManager().GetMap().GetTile(collider.GetComponent<mapTileScript>().GetTileId());
                    RandomEventEffect effect = new RandomEventEffect(METEOR_STRIKE_EFFECT, METEOR_STRIKE_TURNS);
                    effect.SetVisualEffectInWorld(gameObject);
                    hitTile.ApplyEventEffect(effect);
                }
            }

            Destroy(meteorModelGameObject);
            GameObject.Instantiate(explosionGameObject, transform.position, Quaternion.identity, transform);
            GameObject.Instantiate(meteorDestroyedGameObject, transform.position, Quaternion.identity, transform);
            Destroy(this.GetComponent<SphereCollider>());
            Destroy(this.GetComponent<Rigidbody>());
            Destroy(this);
        }
	}
}
