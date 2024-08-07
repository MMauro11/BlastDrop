using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{

    [Tooltip("Object containing explosion effect")]
    [SerializeField] protected GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = GameObject.Instantiate(explosion, this.transform.position, this.transform.rotation);
    }
}
