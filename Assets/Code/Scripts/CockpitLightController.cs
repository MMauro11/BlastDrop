using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitLightController : MonoBehaviour
{
    private float intensity;
    private Light thisLight;
    // Start is called before the first frame update
    void Start()
    {
        thisLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        intensity = 1/Conductor.instance.loopPositionInAnalog+0.1f;
        thisLight.intensity = Mathf.Clamp(intensity,0f,10f);
    }

    public void LightSwitch()
    {
        thisLight.intensity=1.5f;
    }
}
