using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Simple Bullet Logic")]
    public int bulletDamage = 1;                // the amount of damage the enamy can do
    public float bulletLifeSpan = 6.0f;         //how long the bullet stays on screen
    float deathTimer;                           //stores the amount of time left for the bullet

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            //stores the player object
            GameObject target = other.gameObject;

            //gets the player's current script and stores it
            PlayerController playerData = target.GetComponent<PlayerController>();

            //calls th player's take damage function
            playerData.TakeDamage(bulletDamage);

            //Destroys bullet
            Death();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //counts down until the bullets lifespan is over
        deathTimer += Time.deltaTime;
        if(deathTimer>= bulletLifeSpan)
        {
            Death();
        }
    }

    void Death()
    {
        //DEstroys the game object
        Destroy(gameObject, 0.1f);
    }
}
