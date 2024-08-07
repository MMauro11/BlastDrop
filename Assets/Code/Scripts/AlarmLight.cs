using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    [SerializeField] Light thisLight;
    private AudioSource audio;

    private bool isImmune;
    // Start is called before the first frame update
    void Start()
    {
        audio = thisLight.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isImmune)
        {
            ShotStats shot;
            if (collision.gameObject.tag == "Enemy")
            {
                StartCoroutine(Alarm());
                ScoreController.instance.PlayerHit(10);
            }
            if (collision.gameObject.tag == "EnemyShot")
            {
                collision.gameObject.TryGetComponent<ShotStats>(out shot);
                ScoreController.instance.PlayerHit(shot.Damage * Conductor.instance.ActualMultiplier);
                StartCoroutine(Alarm());
            }
            StartCoroutine(ImmunityTime(3)); 
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

    IEnumerator ImmunityTime(float immuneTime)
    {
        isImmune = true;
        float timer = 0;
        while (timer < immuneTime)
        {
            yield return null; // wait for the next Update frame
            timer += Time.deltaTime;
        }
        isImmune = false;
    }
}