using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum weaponType { normalGun, cannon, Bomb };

    public weaponType weapons;
    int weaponToUI;
    GameObject UI;
    public bool isCoin = false;
    public float powerUpLifeTime = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        UI = GameObject.FindGameObjectWithTag("Manager");
    }

    void Update()
    {
        float timer = 0.0f;
        timer += Time.deltaTime;
        if(timer > powerUpLifeTime)
        {
            Death();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.tag == ("Player"))
        {
            if (!isCoin)
            {
                switch (weapons)
                {
                    case weaponType.normalGun:
                        weaponToUI = 1;
                        break;
                    case weaponType.cannon:
                        weaponToUI = 2;
                        break;
                    case weaponType.Bomb:
                        weaponToUI = 3;
                        break;

                }
            }


            GameUI newWeapon = UI.GetComponent<GameUI>();

            newWeapon.WeaponsInt(weaponToUI);

          
        }
        else
        {
            int bonusPoints = Random.Range(1, 5);

            GameUI newWeapon = UI.GetComponent<GameUI>();

            newWeapon.UpdateScore(bonusPoints);
        }

        
    }
    void Death()
    {
        Destroy(gameObject, 0.01f);
    }
}
