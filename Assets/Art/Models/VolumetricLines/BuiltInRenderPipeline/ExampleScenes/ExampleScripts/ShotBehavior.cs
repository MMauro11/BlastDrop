using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour {
	[SerializeField] private float speed = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * 1000f * speed;
	
	}
}
