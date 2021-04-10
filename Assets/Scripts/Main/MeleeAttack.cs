using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public bool player;
    GameObject target;
    public int damaged = 1;


    public void OnTriggerEnter(Collider other)
    {
        if (player)
        {
            if(other.gameObject.tag == ("EnemyHitBox"))
            {
                GameObject target = other.gameObject;

                EnemyAI enemyData = target.GetComponentInParent<EnemyAI>();

                enemyData.TakeDamage(damaged);
            }
        }
        else
        {
            if(other.gameObject.tag == ("Player"))
            {
                GameObject target = other.gameObject;

                PlayerController playerData = target.GetComponent<PlayerController>();

                playerData.TakeDamage(damaged);
            }
        }
    }


}
