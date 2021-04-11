using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //Movement
    GameObject playerObj;                      //Reference to the player
    UnityEngine.AI.NavMeshAgent nav;           //reference to the nav mesh agent
    Vector3 targetPos;                         //the target position to go to


    [Header("AI Logic")]
    public float attackDistance = 15.0f;        //the distamce the enemy will attack the player
    bool playerDead;                               //the amount of health the player has
    public LayerMask randomPoint;               //the layers theAi can get a random position from
    float disFromPlayer;                        //the Current distance from player
    public float walkRadius = 120.0f;           //the area to find a random position
    public float maxTimeToPos = 6.0f;           //the max amount of time to get the random area
    float moveTimer;

    [Header("Enemy Stats")]
    //Enemy set up
    public int enemyHealth = 2;                 //the max healt of this current enemy
    public float enemySpeed = 2.0f;             //The enemy speed
    GameObject UI;
    public int awardedPoints = 1;               // amounts of points for the player
    GameObject objEnemySpawn;
    public GameObject meleeAttackpoint;         //the attack point for melee attack

    [Header("Attack Mode")]
    //sets up the attack logic for our AI
    public float maxAttackTime = 1.5f;          //the max amount of time the enemy can attack
    bool inRange;                               // sets the flag for attack
    float attackTimer = 0.0f;                   //the timer for attacking
    public float bulletSpeed = 100.0f;          //th speed of which bullets move
    public GameObject bullet;                   // the bullet to fire
    public Transform bulletSpawnPoint;          //Where our bullets spawn from


    //Damage
    [Header("Damage Effects")]
    public float flashSpeed = 5.0f;             //how quickly we switch between colours
    public Color flashColour = new Color(0.0f, 0.0f, 0.0f, 1.0f);            //the colour when we are hit
   // public Color unDamagedColour = new Color(0.0f, 0.0f, 0.0f, 1.0f);       //our normal colour
    bool isDead = false;                                                    //the flag to se if they are dead
    bool beenDamaged = false;                                               //has the enemy been hit
    Renderer render;                            //a reference to the renderer

    //Animator
    Transform cam;
    Vector3 camForward;
    Vector3 move;
    Vector3 moveInput;
    float forwardAmount;
    float turnAmount;
    Animator anim;




    // Start is called before the first frame update
    void Awake()
    {
        //get player info
        playerObj = GameObject.FindGameObjectWithTag("Player");

        //to the UI
        UI = GameObject.FindGameObjectWithTag("Manager");

        //Enemy spawn point
        objEnemySpawn = GameObject.FindGameObjectWithTag("EnemySpawner");

        //sets up nav mesh
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

      
        //set the nav mesh speed to our speed
        nav.speed = enemySpeed;

        //gets a reference to the enemy renderer
        render = gameObject.GetComponentInChildren<Renderer>();

        anim = GetComponent<Animator>();

        anim.SetBool("isGrounded", true);
    }

    // Update is called once per frame
    void Update()
    {
        cam = Camera.main.transform;

        float aiForward = nav.velocity.x;
        float aiRight = nav.velocity.z;

        camForward = Vector3.Scale(cam.up, new Vector3(1, 0, 1)).normalized;
        move = (aiForward * Vector3.forward) + (aiRight * Vector3.right);

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        Move(move);

        // call playerCheck
        PlayerCheck();

        //Makes the timer increase everyframe based on frame rate
        moveTimer += Time.deltaTime;
        //attack timer
        attackTimer += Time.deltaTime;

        //Check the distance from player
        disFromPlayer = Vector3.Distance(transform.position, playerObj.transform.position);


        //Calls playerInRange
        PlayerInRange();

        if (playerDead)
        {
            Win();
        }
        else
        {
            //if the player isn't dead check this information
            PlayerInRange();
           // DamageColourChange();
        }
        
    }

    void Move(Vector3 move)
    {
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        this.moveInput = move;

        ConvertMoveInput();
        Animating();
    }
    void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);

        turnAmount = localMove.x;
        forwardAmount = localMove.z;
    }
    void Animating()
    {
        anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.0f, Time.deltaTime);
    }
    void OnTriggerEnter(Collider other)
    {
        //the player is in range for attacking
        //sets the attack flag to active
        if (other.gameObject == playerObj)
        {
            inRange = true;
        }
        
    }
    void OnTriggerExit(Collider other)
    {
        //sets the attack flag to inactive so the AI attacks player
        if (other.gameObject == playerObj)
        {
            inRange = false;
        }
    }

    void PlayerInRange()
    {
        if(disFromPlayer < attackDistance)
        {
            AttackMode();
        }
        //if the player is not close enough to the enemy find random points to go to
        else
        {
            Wondering();
        }
    }
    void Wondering()
    {
        if(moveTimer > maxTimeToPos)
        {
            //make a random vector within a sphere
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

            //find a random position within the sphere
            randomDirection += transform.position;
            UnityEngine.AI.NavMeshHit hit;
            UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, walkRadius, randomPoint);

            //sets the random positition to a point for the enemy to find
            targetPos = hit.position;

            //Reset Timer
            moveTimer = 0.0f;

            Movement(targetPos);
        }
    }
    //changes the player's colour when hit
  /*  void DamageColourChange()
    {
        if (beenDamaged)
        {
            render.material.color = flashColour;
        }
        //otherwise
        else
        {
            //goes back to the non hit colour
            render.material.color = Color.Lerp(render.material.color, unDamagedColour, flashSpeed * Time.deltaTime);
        }

        //resets the damage flag
        beenDamaged = false;
    }
  */

    //everytime take damage is called we will also need to send an amount
    public void TakeDamage(int damageAmount)
    {
        //has been attacked or hit by hazard
        beenDamaged = true;

        anim.SetTrigger("gotHit");

        //lowers the player's health by the amount of damage
        enemyHealth -= damageAmount;

        //check if we are dead
        if (enemyHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void AttackMode()
    {
        //get the player's position
        targetPos = playerObj.transform.position;
        Movement(targetPos);

        //makes the AI look at the player
        Vector3 LookAt = playerObj.transform.position - transform.position;

        float step = enemySpeed * Time.deltaTime;

        //makes the two points for the enemy to look at
        Vector3 newDir = Vector3.RotateTowards(transform.forward, LookAt, step, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDir);

        //Can we attack?
        if(inRange && attackTimer >= maxAttackTime&& disFromPlayer <= attackDistance)
        {
            //if the player is still close enough to attack
            Attack();
        }
        else
        {
            //if the player has moved too far or the enemy has attacked then cool down
            AttackCoolDown();
        }
    }
    void Attack()
    {
        nav.speed = 0.0f;
        attackTimer = 0.0f;

       

        int attackType = Random.Range(0, 3);

        if (attackType == 0)
        {
            meleeAttackpoint.SetActive(true);
            anim.SetTrigger("MeleeAttack");
        }
        else
        {

            //spawns the bullet game object
            GameObject bulletClone = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bulletClone.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

            anim.SetTrigger("ShootAttack");
        }


    }
    void AttackCoolDown()
    {
        if(attackTimer > 0.5f)
        {
            meleeAttackpoint.SetActive(false);
        }

        if (attackTimer > 1.0f)
        {
            nav.speed = enemySpeed;
        }
    }

    void Movement(Vector3 pos)
    {
        if(enemyHealth> 0 && !playerDead)
        {
            //go to target
            nav.SetDestination(pos);
        }
        else
        {

            Win();
            
        }
    }
    void PlayerCheck()
    {
        //gets the players current script and stores it
        PlayerController playerData = playerObj.GetComponent<PlayerController>();

        //get player's health
        playerDead = playerData.isDead;

    }
    void Death()
    {
        //updates the score in the UI
        GameUI uiPointSystem = UI.GetComponent<GameUI>();

        uiPointSystem.UpdateScore(awardedPoints);

        EnemySpawner enemyCounter = objEnemySpawn.GetComponent<EnemySpawner>();

        enemyCounter.UpdateEnemyCounter();

        anim.SetBool("isDead", true);

        Destroy(gameObject, 1.0f);
    }
    void Win()
    {
        nav.enabled = false;
    }
}
