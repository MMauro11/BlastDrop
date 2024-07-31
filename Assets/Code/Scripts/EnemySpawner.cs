using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;

    [Tooltip("Number of bars to wait for spawn")]
    [SerializeField] private float spawnDelay;

    [SerializeField] private int enemiesNumber, barsCounter=0;
    [SerializeField] private float distanceMultiplier;
    PlayerControl playerControl;

    int enemyIndex = 0;

    public void CreateEnemiesInCircle(int num, Vector3 point, float radius)
    {
        //randomize starting position
        float rand = UnityEngine.Random.Range(0, 360);
        for (int i = 0; i < num; i++)
        {
            /* Distance around the circle */
            var radians = 2 * Mathf.PI / num * i + rand;

            /* Get the vector direction */
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            var spawnDir = new Vector3(horizontal, UnityEngine.Random.Range(-0.2f, +0.2f), vertical);

            /* Get the spawn position */
            var spawnPos = point + spawnDir * radius; // Radius is just the distance away from the point

            /* Now spawn */
            var enemy = Instantiate(enemies[enemyIndex], spawnPos, Quaternion.identity) as GameObject;
            /* Rotate the enemy to face towards player */
            enemy.transform.LookAt(point);

            /* Adjust height */
            enemy.transform.Translate(new Vector3(0, enemy.transform.localScale.y / 2, 0));

            //Cycle enemies to spawn from the array
            enemyIndex++;
            enemyIndex = (int)Mathf.Repeat(enemyIndex, enemies.Length);
            print(enemyIndex);
        }
    }

    private void BarsCounter()
    {
        barsCounter++;

        if (barsCounter%spawnDelay==0)
        {
            CreateEnemiesInCircle(enemiesNumber, playerControl.PlayerPosition(), playerControl.GetComponent<SphereCollider>().radius * distanceMultiplier);
            //CreateEnemiesAroundPoint(enemiesNumber, playerControl.PlayerPosition(), playerControl.GetComponent<SphereCollider>().radius * distanceMultiplier);
        }
    }

    void Start()
    {
        Conductor.instance.Bar += BarsCounter;
        playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
    }

}
