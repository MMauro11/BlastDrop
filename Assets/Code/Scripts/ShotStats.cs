using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotStats : MonoBehaviour
{
    [Tooltip("Damage dealt from the projectile")]
    [SerializeField] private int damage;

    public int Damage
    {
        get { return this.damage; }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
