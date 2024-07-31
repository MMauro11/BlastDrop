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
    private Vector3 playerDirection;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerRadius = player.GetComponent<SphereCollider>().radius;
        Conductor.instance.Beat += Move;
    }

    private void FixedUpdate()
    {
        playerDirection=player.transform.position - this.transform.position;
        FacePlayer();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FacePlayer()
    {
        Quaternion newRotation = Quaternion.LookRotation(playerDirection);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, 0.05f);
    }

    private void Move()
    {
        if(Vector3.Distance(this.transform.position, player.transform.position) < playerRadius)
        {
            return;
        }
        //sync-FREE movement
        //rb.MovePosition(transform.position + direction * speed);

        //Synced IMPULSE MOVEMENT
        rb.AddForce(playerDirection * speed, ForceMode.Impulse);

    }

    private void OnDestroy()
    {
        Conductor.instance.Beat -= Move;
    }
}
