using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Simple Bullet Logic")]
    public int bulletDamage = 1;                // the amount of damage the enamy can do
    public float bulletLifeSpan = 6.0f;         //how long the bullet stays on screen
    float deathTimer;                           //stores the amount of time left for the bullet

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("EnemyHitBox"))
        {
            //stores the player object
            GameObject target = other.gameObject;

            //gets the player's current script and stores it
            EnemyAI enemyData = target.GetComponentInParent<EnemyAI>();

            //calls th player's take damage function
            enemyData.TakeDamage(bulletDamage);

            //Destroys bullet
            Death();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //counts down until the bullets lifespan is over
        deathTimer += Time.deltaTime;
        if (deathTimer >= bulletLifeSpan)
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
