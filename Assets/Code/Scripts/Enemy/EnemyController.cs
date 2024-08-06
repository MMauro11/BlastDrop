using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    private GameObject player;
    private Rigidbody rb;
    private float playerRadius;
    private Vector3 playerDirection, direction, forceDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerRadius = player.GetComponent<SphereCollider>().radius;
        Vector3 forceDirection = playerDirection.normalized;
        Conductor.instance.Bar += Move;
    }

    private void FixedUpdate()
    {
        playerDirection=player.transform.position - this.transform.position;
        FaceDirection(playerDirection);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FaceDirection(Vector3 direction)
    {
        Quaternion newRotation = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, 0.05f);
    }

    private void Move()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) < playerRadius)
        {
            return;
        }
        CalculateDirection();

        //sync-FREE movement
        //rb.MovePosition(transform.position + direction * speed);

        //Synced IMPULSE MOVEMENT
        rb.AddForce(forceDirection * speed, ForceMode.Impulse);

    }

    private void OnDestroy()
    {
        Conductor.instance.Bar -= Move;
    }

    private Vector3 CalculateDirection()
    {
        //Calculating random angles
        Quaternion horizontalRotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-75, 75), Vector3.down);
        Quaternion verticalRotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-25, 25), Vector3.left);
        return forceDirection = horizontalRotation * verticalRotation * playerDirection;
    }
}
