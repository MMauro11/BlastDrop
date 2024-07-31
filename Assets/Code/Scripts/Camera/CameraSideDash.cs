using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSideDash : MonoBehaviour
{
    public Transform transform;

    //shake direction
    private float dir;
    private bool activateShake;

    private Vector3 initialPos;
    private float horizDirection;

    void Awake()
    {
        if (transform == null)
        {
            transform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        initialPos = transform.localPosition;
    }

    private void Start()
    {
    }

    void Update()
    {
        //transform.localPosition = new Vector3(transform.localPosition.x + Mathf.PingPong(Conductor.instance.loopPositionInAnalog, 0.10f), Mathf.PingPong(Conductor.instance.loopPositionInAnalog, 0.5f),transform.localPosition.z);

        if (activateShake)
        {
            print(activateShake);
            print(dir);

            transform.localPosition = new Vector3(Mathf.Lerp(initialPos.x, initialPos.x + dir * 0.5f, 0.05f),initialPos.y,initialPos.z);

            print(transform.localPosition.x);

            if (transform.localPosition.x <= 0)
            {
                activateShake = false;
            }
        }
        else
        {
            transform.localPosition = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        }
    }

    public void Shake(float direction)
    {
        dir = direction;
        activateShake = true;
    }
}
