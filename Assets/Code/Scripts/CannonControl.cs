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

    public Transform m_muzzle;
    public GameObject m_shotPrefab;
    private AudioSource m_shotAudio;
    [SerializeField] private AimControl aimControl;
    [SerializeField] private SC_CircularLoading loadingCircle;
    [SerializeField] private GameObject particleSys;

    [Tooltip("number of Beats to wait to shoot again")]
    [SerializeField] private int cooldown;
    public float cooldownAnalogPosition = 0;
    private Quaternion initialRotation;

    [SerializeField] private TextMeshProUGUI cooldownGo;

    //public bool isLocked = true;
    private bool isOnCooldown = false;

    public bool IsOnCooldown { get => isOnCooldown;}

    protected virtual void Awake()
    {
        initialRotation = this.transform.localRotation;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
       m_shotAudio = m_shotPrefab.GetComponent<AudioSource>();
        cooldownGo.gameObject.SetActive(false);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        RotateCannon();
    }

    public virtual void Shoot()
    {
        //if (!isLocked && !onCooldown)
        //{
        StartCoroutine(CooldownCounter(cooldown));
        GameObject go = (GameObject)Instantiate(m_shotPrefab, m_muzzle.position, m_muzzle.rotation);

        //Istantiate particle Object
        GameObject particleIstance = (GameObject)Instantiate(particleSys, m_muzzle.position, m_muzzle.rotation);
        particleIstance.transform.position = m_muzzle.transform.position;
        particleIstance.transform.rotation = m_muzzle.transform.rotation;
        particleIstance.GetComponent<ParticleSystem>().Play();

        m_shotAudio.Play();
        GameObject.Destroy(go, m_shotAudio.clip.length*2);
        //}
        //else { MultiplierDown(); }
    }

    public virtual void RotateCannon()
    {
        if (aimControl.nearestEnemy != null && !aimControl.IsOutOfBounds())
        {
            Quaternion newRotation = Quaternion.LookRotation(aimControl.nearestEnemy.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, newRotation, 0.3f);
        }
        else { this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, initialRotation, 0.1f); }
    }

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

     private void UpdateScoreText()
    {
        if (!cooldownGo.IsActive())
        { cooldownGo.gameObject.SetActive(true); }
        float percent = cooldownAnalogPosition;
        percent =  Mathf.RoundToInt(percent * 100);
        cooldownGo.SetText(percent.ToString()); 

    }
    private void UpdateLoadingCircle() { loadingCircle.loadingProgress = cooldownAnalogPosition; }

}
