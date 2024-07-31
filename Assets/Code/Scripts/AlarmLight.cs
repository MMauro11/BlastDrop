using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    private float intensity;
    [SerializeField] Light thisLight;
    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = thisLight.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ShotStats shot;
        if (collision.gameObject.tag == "Enemy")
        {
            //collision.gameObject.TryGetComponent<ShotStats>(out shot);
            StartCoroutine(Alarm());
        }
    }

    private IEnumerator Alarm() 
    { 
        thisLight.enabled = true;
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length/8);
        thisLight.enabled = false;
        yield return new WaitForSeconds(audio.clip.length / 8);
        thisLight.enabled = true;
        yield return new WaitForSeconds(audio.clip.length / 8);
        thisLight.enabled = false;
    }
}