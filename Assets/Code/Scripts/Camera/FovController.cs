using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FOV_Controller : MonoBehaviour
{
    [SerializeField] private PlayerControl player;
    public float playerSpeed;
    public float lastSpeed;
    public float currentFov; //currentQuantity
    public float desiredFov; //desiredQuantity
    const float zoomStep = 0.5f;

    void Start()
    {
        currentFov = 50f;
        desiredFov = currentFov;
    }

    void CheckSpeed()
    {
        if (playerSpeed < 10)
        {
            lastSpeed = playerSpeed;
            desiredFov = 45f;
            //currentFOV to minFOV
        }
        else if (playerSpeed > 15)
        {
            lastSpeed = playerSpeed;
            desiredFov = 55f;
            //current FOV to maxFOV
        }
    }

    void ProcessFOV()
    {
        currentFov = Mathf.MoveTowards(currentFov, desiredFov, zoomStep * Time.deltaTime);
    }

    void SetFOV()
    {
        Camera.main.fieldOfView = currentFov;
    }

    void Update()
    {

        playerSpeed = player.GetComponent<Rigidbody>().velocity.magnitude;

        //DEBUG
        print(playerSpeed);

        CheckSpeed();
        ProcessFOV();
        SetFOV();

    }
}

