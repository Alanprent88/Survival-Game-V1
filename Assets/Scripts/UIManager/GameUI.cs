using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{

    [Header("UI Objects")]
    public GameObject inventoryMenu;
    public GameObject giveUpMenu;
    public GameObject player;
    public GameObject menuButtons;
    public GameObject playerHealthBar;
    public GameObject equItem;
    public GameObject equItemText;

    PlayerController playerInfo;
    public GameObject scoreHolder;
    public int currentScore = 0;
    string scoreText;
    public GameObject pausedScore;

    bool isStopped = false;

    [Header("Normal Gun")]                  //all the info for Gun
    public Sprite gun;
    public int normalGunAmmo;
    public int normalGunAmmoMax;
    public GameObject normalGun;
    public float normalGunSpeed;
    public int normalGunCounter;

    [Header("Cannon")]                      //all the info for the Cannon
    public Sprite cannon;
    public int cannonAmmo;
    public int cannonAmmoMax;
    public GameObject cannonGun;
    public float cannonSpeed;
    public int cannonCounter;

    [Header("Bomb")]                      //all the info for the bomb
    public Sprite bomb;
    public int bombAmmo;
    public int bombAmmoMax;
    public GameObject bombGun;
    public float bombSpeed;
    public int bombCounter;
    
    [Header("No Weapon")]
    public Sprite none;

    [HideInInspector]
    public int playersCurrentAmmo;



    // Start is called before the first frame update
    void Awake()
    {
        playerInfo = player.GetComponent<PlayerController>();

        currentScore = 0;
    }

    public void WeaponsInt(int weaponType)
    {
        switch (weaponType)
        {
            case 1:
                NormalGunEqu();
                break;
            case 2:
                CannonEqu();
                break;
            case 3:
                BombEqu();
                break;
        }
    }

    void CannonEqu()
    {
        WeaponUpdate(cannon, cannonAmmo, cannonAmmoMax, cannonGun, cannonSpeed, cannonCounter);
    }

    void BombEqu()
    {
        WeaponUpdate(bomb, bombAmmo, bombAmmoMax, bombGun, bombSpeed, bombCounter);

    }

    void NormalGunEqu()
    {
        WeaponUpdate(gun, normalGunAmmo, normalGunAmmoMax, normalGun, normalGunSpeed, normalGunCounter);
    }

    void NoWeapon()
    {
        WeaponUpdate(none, 0, 0, null, 0.0f, 0);
    }

    void WeaponUpdate(Sprite weaponPick, int Ammo, int maxAmmo, GameObject eBullet, float ammoSpeed, int weaponCount)
    {
        PlayerController playerData = player.GetComponent<PlayerController>();

        equItem.GetComponent<UnityEngine.UI.Image>().sprite = weaponPick;

        playerData.bulletSpeed = ammoSpeed;
        playerData.maxBullets = maxAmmo;
        playerData.attackCounter = weaponCount;
        playerData.bullet = eBullet;
        playerData.maxAttackCounter = Ammo;
    }
    void WeaponUpdate()
    {
        playersCurrentAmmo = playerInfo.maxBullets;
        equItemText.GetComponent<UnityEngine.UI.Text>().text = playersCurrentAmmo.ToString();

    }

    public void UpdateScore(int points)
    {
        currentScore += points;

        scoreHolder.GetComponent<UnityEngine.UI.Text>().text = currentScore.ToString();

    }

    public void PlayGame()
    {

        isStopped = false;
        inventoryMenu.SetActive(false);
        Time.timeScale = 1.0f;
        playerInfo.canAttack = true;
    }
    public void StopGame()
    {
        isStopped = true;
        inventoryMenu.SetActive(true);
        pausedScore.GetComponent<UnityEngine.UI.Text>().text = currentScore.ToString();
        Time.timeScale = 0.0f;
        playerInfo.canAttack = false;

    }

    public void GiveUp()
    {
        menuButtons.SetActive(false);
        giveUpMenu.SetActive(true);
    }

    public void DontGiveUp()
    {
        menuButtons.SetActive(true);
        giveUpMenu.SetActive(false);
    }

    public void UpdateHealth()
    {
        int playerHP = playerInfo.playerHealth;
        int maxHP = playerInfo.startingPlayerHealth;

        playerHealthBar.GetComponent<UnityEngine.UI.Image>().fillAmount = ((float)playerHP) / maxHP;
    }

    

    // Update is called once per frame
    void Update()
    {
        WeaponUpdate();

        if (Input.GetButtonDown("Submit"))
        {
            if (!isStopped)
            {
                StopGame();
            }
        }
    }
}
