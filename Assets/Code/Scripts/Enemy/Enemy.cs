using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int life;
    [SerializeField] private int points;
    //GameObject contaning particle effects
    public GameObject explosionEffect;
    private AudioSource audioSource;
    // Start is called before the first frame update

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    { 
        ShotStats shot;
        if (collision.gameObject.tag == "Projectile")
        {
            collision.gameObject.TryGetComponent<ShotStats>(out shot);
            HealthDown(shot.Damage);
            if (!IsStillAlive())
            {
                Death();
            }
        }
    }

    private void HealthDown(int damage)
    {
        if (life > 0)
        {
            life = life - damage;
            SightDrawer.instance.RemoveDot();
        }
    }

    private bool IsStillAlive()
    {
        if (life > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Death()
    {
        Conductor.instance.PlayEnemyDeath();
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        //instantiate Gameobject containing death particle effect
        GameObject go = Instantiate(explosionEffect,this.transform.position, this.transform.rotation) as GameObject;
        GameObject.Destroy(go, 2f);

        ScoreController.instance.EnemyDefeated(points);
    }

    public int Life { get => life;}
}
