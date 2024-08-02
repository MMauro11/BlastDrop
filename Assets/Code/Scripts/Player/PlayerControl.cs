using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Conductor conductor;
    [SerializeField] private AudioSource primaryFireAudio;
    [SerializeField] private float turningSpeed = 10000.0f;
    [SerializeField] private float verticalSpeed = 1;
    [SerializeField] private float dashForce = 1f;
    private CameraSideDash cameraSideDash;
    public Rigidbody playerRb;
    private Vector3 movement;
    private Vector3 inclinationVector;
    private Vector3 playerPosition;
    private float horizDir, vertDir;
    private PlayerAnimation playerAnimation;
    [SerializeField] public CannonControl cannon1,cannon2,cannon3,cannon4;
    //public float playerSpeed => playerRb.velocity.magnitude;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimation = Camera.main.GetComponent<PlayerAnimation>();
        playerRb = this.GetComponent<Rigidbody>();
        Conductor.instance.Beat += MoveCharacter;
    }
    
    // Update is called once per frame
    void Update()
    {
        playerPosition = playerRb.position;

        MoveUp();
        MoveDown();
    }


    public void Shoot(CannonControl cannon)
    {
        if (Conductor.instance.IsOnBeat() && !cannon.IsOnCooldown)
        {
            cannon.Shoot();
        }
        else
        {
            //not on beat calling Streak breaker
            ScoreController.instance.StreakDown();
        }
    }

    public void MoveCharacter()
    {
        horizDir = Input.GetAxis("Horizontal");
        vertDir = Input.GetAxis("Vertical");
        // Convert direction into Rigidbody space.
        movement = new Vector3(horizDir, 0, vertDir);
        movement = playerRb.rotation * movement;

        //Sync-FREE MOVEMENT
        // playerRb.MovePosition(playerRb.position + movement * moveSpeed * Time.fixedDeltaTime);

        //Syncronized MOVEMENT
        playerRb.AddForce(movement * dashForce, ForceMode.Impulse);

        //camera shake
        playerAnimation.Shake(horizDir);
        
    }

    public void RotateCharacter()
    {
        //Rotate playerShip
        Quaternion deltaRotation = Quaternion.Euler(0, Input.GetAxis("Mouse X") * turningSpeed * Time.fixedDeltaTime, 0);
        playerRb.MoveRotation(playerRb.rotation * deltaRotation);
    }

    public Vector3 PlayerPosition() { return playerPosition; }

    public void MoveUp()
    {
            Vector3 verticalMovement;
            verticalMovement = new Vector3(0, 1, 0).normalized;
            verticalMovement = playerRb.rotation * verticalMovement;
            playerRb.MovePosition(playerRb.position + verticalMovement * verticalSpeed * Time.fixedDeltaTime); 
    }

    public void MoveDown()
    {
            Vector3 verticalMovement;
            verticalMovement = new Vector3(0, -1, 0).normalized;
            verticalMovement = playerRb.rotation * verticalMovement;
            playerRb.MovePosition(playerRb.position + verticalMovement * verticalSpeed * Time.fixedDeltaTime); 
    }
}

