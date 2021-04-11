using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickUp : MonoBehaviour
{
    public enum weaponType { normalGun, cannon, Bomb };

    public weaponType weapons;
    int weaponToUI;
    GameObject UI;
    public bool isCoin = false;
    public float powerUpLifeTime = 15.0f;
    public float rotSpeed = 0.1f;

    public GameObject pEffects;

    public AudioClip powerUp;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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

        transform.Rotate(0, (rotSpeed * Time.deltaTime), 0, Space.Self);
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

        audioSource.PlayOneShot(powerUp, 1.0f);

        GameObject effectP = Instantiate(pEffects, transform.position, transform.rotation);

        effectP.transform.SetParent(transform);
    }
    void Death()
    {
        Destroy(gameObject, 1.0f);
    }
}
