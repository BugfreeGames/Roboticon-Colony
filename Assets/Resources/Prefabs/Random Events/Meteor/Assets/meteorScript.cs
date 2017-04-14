using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorScript : MonoBehaviour
{
    public float detectRadius = 2.4f;
    public float blastRadius = 5;
    public GameObject explosionGameObject;
    public GameObject meteorDestroyedGameObject;
    public GameObject meteorModelGameObject;

    private ResourceGroup eventEffect = new ResourceGroup(-50,-50, 13);

	// Use this for initialization
	void Start ()
    {
        GetComponent<Rigidbody>().AddForce(Random.Range(-500, 500), 0, Random.Range(-500, 500));
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
                    RandomEventEffect effect = new RandomEventEffect(eventEffect, 3);
                    effect.SetVisualEffectInWorld(gameObject);
                    hitTile.ApplyEventEffect(effect);
                }
            }

            Destroy(meteorModelGameObject);
            GameObject.Instantiate(explosionGameObject, transform.position, Quaternion.identity, transform);
            GameObject.Instantiate(meteorDestroyedGameObject, transform.position, Quaternion.identity, transform);
            Destroy(this.GetComponent<SphereCollider>());
            Destroy(this.GetComponent<Rigidbody>());
            transform.Translate(0, -2, 0);
            Destroy(this);
        }
	}
}
