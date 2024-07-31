using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    private Collider collider;

    [Tooltip("Object containing explosion effect")]
    [SerializeField] GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = GameObject.Instantiate(explosion, this.transform);
    }
}
