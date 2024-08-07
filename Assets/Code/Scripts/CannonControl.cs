using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;

public class CannonControl : MonoBehaviour
{
    // Update is called once per frame

    [SerializeField] private Transform m_muzzle;
    [SerializeField] private GameObject m_shotPrefab;
    [SerializeField] private AimControl aimControl;
    [SerializeField] private SC_CircularLoading loadingCircle;
    [SerializeField] private GameObject particleSys;
    [SerializeField] private int projectiles;
    [SerializeField] private TextMeshProUGUI cooldownGo;
    
    [Tooltip("number of Beats to wait to shoot again")]
    [SerializeField] private int cooldown;
    
    private AudioSource shotAudio;

    public float cooldownAnalogPosition = 0;
    private Quaternion initialRotation;

    //public bool isLocked = true;
    private bool isOnCooldown = false;


    public bool IsOnCooldown { get => isOnCooldown;}
    public AudioSource ShotAudio { get => shotAudio;}

    protected virtual void Awake()
    {
        initialRotation = this.transform.localRotation;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        shotAudio = m_shotPrefab.GetComponent<AudioSource>();
        //if projectile prefab not set to play audio when istantiated
        if (shotAudio.playOnAwake == false)
        {
            shotAudio = this.GetComponent<AudioSource>();
            shotAudio.enabled = true;
            shotAudio.clip = m_shotPrefab.GetComponent<AudioSource>().clip;
            shotAudio.outputAudioMixerGroup = m_shotPrefab.GetComponent<AudioSource>().outputAudioMixerGroup;
        }

        cooldownGo.gameObject.SetActive(false);

        //check for a minimum of 1 projectile to shoot
        if(projectiles < 1)
        { 
            projectiles = 1;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        RotateCannon();
    }

    public virtual void Shoot()
    {
        //if (!isLocked)
        //{
        StartCoroutine(SpawnProjectiles());
        StartCoroutine(CooldownCounter(cooldown));

        //Set projectile AudioSource to playOnAwake if you want the sound to repeat for every projectile
        if (m_shotPrefab.GetComponent<AudioSource>().playOnAwake == false)
        {
            shotAudio.Play();
        }
        //}
    }

    //Controls the direction of the cannons
    public virtual void RotateCannon()
    {
        if (aimControl.nearestEnemy != null && !aimControl.IsOutOfBounds())
        {
            Quaternion newRotation = Quaternion.LookRotation(aimControl.nearestEnemy.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, newRotation, 0.3f);
        }
        else { this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, initialRotation, 0.1f); }
    }

    //Controls Cooldown UI
    private IEnumerator CooldownCounter(int cooldown)
    {
        isOnCooldown = true;
        cooldownAnalogPosition = 1;
        float currentTime = 0;
        float waitTime;
        if (Conductor.instance.loopPositionInAnalog > 0.5f)
        {
            waitTime = (1 - Conductor.instance.loopPositionInAnalog - Conductor.instance.tolerance) + (Conductor.instance.SecPerBeat * cooldown);
        }
        else
        {
            waitTime = Conductor.instance.SecPerBeat * cooldown - Conductor.instance.loopPositionInAnalog - Conductor.instance.tolerance;
        }
        while (currentTime < waitTime)
        {
            currentTime += Time.deltaTime;
            cooldownAnalogPosition = Mathf.Lerp(1, 0, currentTime / waitTime);
            UpdateScoreText();
            UpdateLoadingCircle();
            yield return null;
        }
        //deactivate cooldown text
        cooldownGo.gameObject.SetActive(false);

        isOnCooldown = false;
        yield break;
    }

    //Update Score UI
     private void UpdateScoreText()
    {
        if (!cooldownGo.IsActive())
        { cooldownGo.gameObject.SetActive(true); }
        float percent = cooldownAnalogPosition;
        percent =  Mathf.RoundToInt(percent * 100);
        cooldownGo.SetText(percent.ToString()); 

    }
    //Update UI cooldown circle
    private void UpdateLoadingCircle() { loadingCircle.loadingProgress = cooldownAnalogPosition; }

    //Spawn all projectiles in equally delta time
    private IEnumerator SpawnProjectiles()
    {
        float waitTime = Conductor.instance.SecPerBeat / projectiles;
        for (int i = 0; i < projectiles; i++)
        {
            //istantiate projectile prefab
            GameObject go = (GameObject)Instantiate(m_shotPrefab, m_muzzle.position, m_muzzle.rotation);
            //Istantiate particle Object
            if (particleSys != null)
            {
                GameObject particleIstance = (GameObject)Instantiate(particleSys, m_muzzle.position, m_muzzle.rotation);
                particleIstance.transform.position = m_muzzle.transform.position;
                particleIstance.transform.rotation = m_muzzle.transform.rotation;
                particleIstance.GetComponent<ParticleSystem>().Play();
                //Destroy particle object
                GameObject.Destroy(go, shotAudio.clip.length * 2);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
