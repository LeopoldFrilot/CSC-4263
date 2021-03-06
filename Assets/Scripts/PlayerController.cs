﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed; //controls movementspeed from inspector
    public float jumpForce; //used to jump into air
    public CharacterController controller;

    private Vector3 moveDirection; //vector object ref used to control move direction (x,y,z)
    public float gravityScale;  //float to control gravity from inspector

    //public Animator animate; //used for animation of charactor model
    public Animator animate2;


    public Transform pivot; //setting charachter to turn with camera rotation
    public float rotationSpeed; //rotation speed of character 

    public int numSearches = 0;

    // Importing GameManager for state management
    public GameObject gameManager;
    
    public GameObject playerModel;

    private GameLoop gameLoop;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>(); //initializes public controller variable in inspector
        gameManager = GameObject.Find("GameManager");
        gameLoop = gameManager.GetComponent<GameLoop>();
    }

    void findItem()
    {

    }

    //Search a location for an item
    //void Search()
    //{
    //    numSearches++;
    //    Random ran = new Random();
    //    int prob = gen.Next(8)+2;
    //    if(prob numSearches >= prob) findItem();
    //}

    // Update is called once per frame
    void Update() //Remark: using rigidbody for movement is better for racing games (eg. jumping a ramp)
    {
        if (!(gameManager.GetComponent<GameLoop>().IsPaused())) // checking if the game is paused
        {
            Move();
        }

        //exit game with button press "Escape" was moved to GameLoop
    }

    private void Move()
    {
        //new move direction with rotation of mouse
        float yStore = moveDirection.y; //save y direction in a float to correct jump
        moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore; //applying previous up/down movement back to player

        if (controller.isGrounded)
        { //used to smooth out jumps and falling off ledges, "if player is grounded"
            moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            { //jump
                moveDirection.y = jumpForce; //jump is applied in y axis direction
            }
            else if (Input.GetButtonDown("Fire1")) //orgianlly Fire1
            {
               animate2.SetTrigger("Attack"); //changing player attack to trigger
            }
        }
        //to smooth out frames over different systems x Time.deltaTime 
        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);

        //move the player in different direction based on camera look direction
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            //used for gradual rotations for smoother camera rotations
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotationSpeed * Time.deltaTime); //slerp one of the 3 movement types in unity (for a smooth rotation w/ camera)    
        }

        //this code animates character to run motion when directional keys are pressed 
        //animate.SetFloat("Speed", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
        //animate.SetBool("isGrounded", controller.isGrounded);
        animate2.SetFloat("Speed2", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
        animate2.SetBool("isGrounded", controller.isGrounded);
        
  
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "shNoj.L." || col.gameObject.name == "Bip001 Prop2")
        {
            float dmg = -10f;
            gameLoop.UpdateHealth(dmg);
        }
    }
}
