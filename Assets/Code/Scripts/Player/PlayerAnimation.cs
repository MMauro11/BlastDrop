using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float maxZinclination;
    [SerializeField] private float minZinclination;
    [SerializeField] private float maxFov;
    [SerializeField] private float minFov;
    [SerializeField] private float dashDistance;
    [SerializeField] private float inclinationSpeed,fovSpeed;
    [SerializeField] private float initialFov;
    [SerializeField] private float dashSpeed;
    private Vector3 rotEulerAngles, newPos, initialPos, deltaSide, deltaPosition;
    private float horizDirection, vertDirection;
    private Quaternion deltaRotation, newRot;

    private float currentFov, desiredFov;

    //camera side shake
    public Transform transform;

    //shake direction
    private float dir;
    private bool activateShake;

    void Awake()
    {
        if (transform == null)
        {
            transform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        initialPos = this.transform.localPosition;
        currentFov = 50f;
        //Conductor.instance.Beat += MovementAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        horizDirection = Input.GetAxis("Horizontal");
        RotationAnimation();
        FovControl();

        //camera side shake code
        CameraSideShake();
    }


    public void Shake(float direction)
    {
        dir = direction;
        activateShake = true;
    }

    private void CameraSideShake()
    {/*
        //transform.localPosition = new Vector3(transform.localPosition.x + Mathf.PingPong(Conductor.instance.loopPositionInAnalog, 0.10f), Mathf.PingPong(Conductor.instance.loopPositionInAnalog, 0.5f),transform.localPosition.z);
        if (activateShake)
        {
            print(activateShake);
            print(dir);

            transform.localPosition = new Vector3(Mathf.Lerp(initialPos.x,dir * 0.5f, Time.fixedDeltaTime * dashSpeed), initialPos.y, initialPos.z);

            print(transform.localPosition.x);

            if (transform.localPosition.x <= 0)
            {
                activateShake = false;
            }
        }
        else
        {
            transform.localPosition = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        }
        */
        float newPosition, desiredPos;
        newPosition = transform.localPosition.x;
        desiredPos= newPosition + horizDirection * fovSpeed;
        desiredPos= Mathf.SmoothStep(newPosition , desiredPos, 0.3f);

        //Position Change
        if (desiredPos> -dashDistance && desiredPos< dashDistance)
        {
            transform.localPosition = new Vector3(desiredPos,transform.localPosition.y,transform.localPosition.z);
        }
        else
        {
            //print("desiredDistance exceeded");
        }
        //Stabilization
        if (horizDirection == 0 || (newPosition < initialPos.x && horizDirection > 0) || (newPosition > initialPos.x && horizDirection < 0))
        {
            transform.localPosition = new Vector3(Mathf.MoveTowards(newPosition , initialPos.x , 0.05f * Time.fixedDeltaTime), transform.localPosition.y, transform.localPosition.z);
        }
    }

    private void RotationAnimation()
    {
        deltaRotation = Quaternion.Euler(0, 0, horizDirection * inclinationSpeed * Time.fixedDeltaTime);
        newRot = Quaternion.Slerp(this.transform.localRotation, this.transform.localRotation * deltaRotation, 0.9f);
        rotEulerAngles = newRot.eulerAngles;

        //angle conversion
        rotEulerAngles.z = (rotEulerAngles.z > 180) ? rotEulerAngles.z - 360 : rotEulerAngles.z;
        //z rotation constraint
        rotEulerAngles.z = Mathf.Clamp(rotEulerAngles.z, minZinclination, maxZinclination);
        //rotation
        this.transform.localRotation = Quaternion.Euler(rotEulerAngles);

        //Stabilization
        if (horizDirection == 0 || (transform.localRotation.z < 0 && horizDirection > 0) || (transform.localRotation.z > 0 && horizDirection < 0))
        {
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.identity, 0.02f);
        }
    }

    private void FovControl()
    {
        currentFov = Camera.main.fieldOfView;
        vertDirection = -Input.GetAxis("Vertical");
        desiredFov = currentFov + vertDirection * fovSpeed;
        desiredFov = Mathf.SmoothStep(currentFov, desiredFov, 0.3f);

        //Fov Change
        if (desiredFov > minFov && desiredFov < maxFov)
        {
            Camera.main.fieldOfView = desiredFov;
        }
        else
        {
            print("desiredFov exceeded");
        }
        //Stabilization
        if (vertDirection == 0 || (currentFov < initialFov && vertDirection > 0) || (currentFov > initialFov && vertDirection < 0))
        {
           Camera.main.fieldOfView = Mathf.MoveTowards(currentFov, initialFov, 2 * Time.fixedDeltaTime);
        }
    }
}
