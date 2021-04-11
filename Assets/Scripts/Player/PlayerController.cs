using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Player Stats
    [Header("player Stats")]
    public int playerHealth;                    //The max amount of times the player can be hit
    public int startingPlayerHealth = 10;       //the starting HP of the player whenever the game resets
    public bool isDead = false;                        //the flag for when the plaers hp drops to 0 or lower
    bool beenDamaged = false;                   //Have we been attacked
    Renderer render;                            //a reference to the renderer
    public bool canAttack = true;               //stops player from attacking

   
     public Transform cam;

    public float rotationSpeed;
    private float y;

    //Player movement VArs
    [Header("Player Movement")]
    public float playerSpeed = 6.0f;            //The max speed the player can move
    Vector3 movementDir;                        //Stores the players inputs and drives the Rigidbody

    //Jumping Vars
    [Header("Jumping")]
    public Transform feetPosition;              //gets an empty's positon for ground checking
    public float jumpForce = 5.0f;               //the amount of force needed to make our player jump
    public float groundRadiusSize = 5.0f;       //the radius size to check if the player is on any object on the floor layer
    bool isGrounded;                            //is used to see if the player can jump or is falling

    //Plyaer turning Vars
    public float camRayLength = 100.0f;         //the Length of the raycast to see if the mouse is over anything on the floor layer

    //Damage
    [Header("Damage Effects")]
    public float flashSpeed = 5.0f;             //how quickly we switch between colours
    public Color flashColour = new Color(0.0f, 0.0f, 0.0f, 1.0f);            //the colour when we are hit
    public Color unDamagedColour = new Color(0.0f, 0.0f, 0.0f, 1.0f);       //our normal colour


    //References
    [Header("Misc")]
    Rigidbody playerRB;                         //a reference tot the players rigidbody
    public LayerMask floorLayer;                //the layer that is used for the ground objects and mouse information
    GameObject UI;

    [Header("Melee Attacks")]
    public GameObject meleeAttackPoint;         //the attack point for a normal attack
    float meleeAtkTimer;                        //melee attack Timer
    bool canMeleeAttack = true;

    [Header("Attack")]
    public float bulletSpeed = 100.0f;          //th speed of which bullets move
    public GameObject bullet;                   // the bullet to fire
    public Transform bulletSpawnPoint;          //Where our bullets spawn from
    public int attackCounter = 0;                      //the current attack
    public int maxAttackCounter = 10;           //the max amount of shot the player can shoot
    float atkTimer;                             //cool down timer for when the player has shot the max amount of bullets
    public float atkCoolDownTimer = 2.0f;       //the cool down timer for attacking
    public int maxBullets;                      //if this reaches zero we cant shoot

    //Animator
   
    Vector3 camForward;
    Vector3 move;
    Vector3 moveInput;
    float forwardAmount;
    float turnAmount;
    Animator anim;




    // Start is called before the first frame update
    void Awake()
    {
       

        UI = GameObject.FindGameObjectWithTag("Manager");

        //Gets the player's Rigidbody and stores it in the playerRB
        playerRB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        //gets the children with mesh renderer so we can change the colour
        render = gameObject.GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        cam = Camera.main.transform;

        //Checks if we are on the floor layer
        isGrounded = Physics.OverlapSphere(feetPosition.position, groundRadiusSize, floorLayer).Length > 0;

        if(isGrounded)
        {
            anim.SetLayerWeight(1, 0.65f);
        }
        else
        {
            anim.SetLayerWeight(1, 0.0f);
        }

        anim.SetBool("isGrounded", isGrounded);

        /*
        //Gets the user Raw input and stores them in a float every frame
        float inputH = Input.GetAxisRaw("Horizontal");
        float inputV = Input.GetAxisRaw("Vertical");

        */


        var floatX = Input.GetAxis("Horizontal");
        var floatY = Input.GetAxis("Vertical");

        Moving(floatX, floatY);



        if (attackCounter >= maxAttackCounter)
        {
            AttackCoolDown();
        }

        //Animation
        if(cam != null)
        {
            camForward = Vector3.Scale(cam.up, new Vector3(1, 0, 1)).normalized;
            move = (floatX * Vector3.forward) + (floatY * Vector3.right);
        }

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        Move(move);

        if (!isDead)
        {
            //Calls the movement event and transfers the input information tot the event
             //  Movement(inputH, inputV);

            

            //Check to see if we can jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }

            //calls our turning Event
            Turning();

            //calls the been attacked effect
            DamageColourChange();

            //Check to see if we can jump
            if (Input.GetButtonDown("Fire1") && isGrounded && attackCounter <= maxAttackCounter && maxBullets !=0)
            {
                Attack();
            }
            else
            {
                AttackReset();
            }

            // Melee Attack Input
            if(Input.GetButtonDown("Fire2") && isGrounded && canMeleeAttack)
            {
                MeleeAttack();
            }
            if (!canMeleeAttack)
            {
                MeleeAttackReset();
            }

        }

    }
    void Move(Vector3 move)
    {
        if(move.magnitude>1)
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
        anim.SetFloat("Forward", forwardAmount, 0.1f , Time.deltaTime);

        anim.SetFloat("Turn", turnAmount, 0.1f , Time.deltaTime);
    }

    void MeleeAttack()
    {
        anim.SetTrigger("MeleeAttack");

        meleeAttackPoint.SetActive(true);
        canMeleeAttack = false;

    }

   void MeleeAttackReset()
    {
        meleeAtkTimer = meleeAtkTimer + Time.deltaTime;

        if(meleeAtkTimer >= 0.5f)
        {
            meleeAttackPoint.SetActive(false);
        }
        if(meleeAtkTimer >= 1)
        {
            canMeleeAttack = true;
            meleeAtkTimer = 0;
        }
    }
    void Attack()
    {
        attackCounter += 1;
        maxBullets -= 1;

        anim.SetTrigger("ShootAttack");

        //spawns the bullet game object
        GameObject bulletClone = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bulletClone.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
    }
    void AttackCoolDown()
    {
        atkTimer += Time.deltaTime;
    }
    void AttackReset()
    {
        if (atkTimer >= atkCoolDownTimer)
        {
            //resets values
            attackCounter = 0;
            atkTimer = 0.0f;
        }
    }

    public void Movement(float h, float v)
    {
        //Sets the players values to MovementDir Vector3
        movementDir.Set(h, 0, v);

        //Normalizes the movementDir Vector and scales the value with the frame rate
        movementDir = movementDir.normalized * playerSpeed * Time.deltaTime;

        //Adds the player input to the rigidbody position
        playerRB.MovePosition(transform.position + movementDir);
    }
    /*
     void Movement1(float h, float v)
     {
         Vector3 direction = new Vector3(h, 0f, v).normalized;

         if (movementDir.magnitude >= 0.1f)
         {
             float targetAngle = Mathf.Atan2(movementDir.x, movementDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
             float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turSmoothVelocity, turnSmoothTime);
             transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

             Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
             playerRB.MovePosition(moveDir.normalized * playerSpeed * Time.deltaTime);

         }
     }
    */
    void Moving(float x, float z)
    {
        var v = cam.transform.right * x;
        var h = cam.transform.forward * z;

        var inputDir = (v + h).normalized;
        inputDir.y = 0.0f;

        y = playerRB.velocity.y;

        var moveAmount = Mathf.Clamp01(Mathf.Abs(x) + Mathf.Abs(z));
        var speedAmount = moveAmount * playerSpeed;

        //Rotation
        var targetDir = inputDir;
        if (targetDir == Vector3.zero) targetDir = transform.forward;

        var rotAmount = rotationSpeed * Time.fixedDeltaTime;
        var lookDir = Quaternion.LookRotation(targetDir);
        var targetRot = Quaternion.Slerp(transform.rotation, lookDir, rotAmount);

        transform.rotation = targetRot;

        var moveDir = new Vector3(inputDir.x * speedAmount, y, inputDir.z * speedAmount);
        playerRB.velocity = moveDir;


    }
    void Jump()
    {
        //Makes the player jump up
        playerRB.velocity = Vector3.up * jumpForce;
    }

    void Turning()
    {
        //Creates a raycast based on the mouses's position on the screen
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Stores what ever the raycast hit
        RaycastHit floorHit;

        //Creates the raycast and sees if it hit anything on our layer
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorLayer))
        {
            //finds the point that the raycast hit and removes our current position from it
            Vector3 posToMove = floorHit.point - transform.position;

            //Hardsets the y value to 0 to stop any extra y rotation
            posToMove.y = 0.0f;

            //Creats a look at rotation between the two positions
            Quaternion newRotation = Quaternion.LookRotation(posToMove);

            //sets the rotation
            playerRB.MoveRotation(newRotation);
        }
    }

    //changes the player's colour when hit
    void DamageColourChange()
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

    //everytime take damage is called we will also need to send an amount
    public void TakeDamage(int damageAmount)
    {
        //has been attacked or hit by hazard
        beenDamaged = true;

        anim.SetTrigger("gotHit");

        //lowers the player's health by the amount of damage
        playerHealth -= damageAmount;

        GameUI uiHealthbar = UI.GetComponent<GameUI>();

        uiHealthbar.UpdateHealth();


        //check if we are dead
        if (playerHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        //sets the flag for death to trigger all death related events
        isDead = true;

        anim.SetBool("isDead",isDead);
    }

}
