using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPosition : MonoBehaviour
{
    [SerializeField] private Camera displayCamera;
    [SerializeField] private Transform followThis;

    //Position of the target relative to the screen
    private Vector2 screenTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        screenTarget = displayCamera.WorldToScreenPoint(followThis.position);
        this.transform.position = screenTarget;
        this.transform.rotation = followThis.localRotation;
    }
}
