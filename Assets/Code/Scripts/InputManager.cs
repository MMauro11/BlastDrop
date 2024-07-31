using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Unity.IO.LowLevel.Unsafe;
using System;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerControl player;
    public KeyCode up;
    public KeyCode right;
    public KeyCode down;
    public KeyCode left;
    public KeyCode pause;
    public KeyCode fire1;
    public KeyCode fire2;
    public KeyCode fire3;
    public KeyCode fire4;
    public KeyCode moveUp;
    public KeyCode moveDown;


    public event Action Fire1Pressed;
    public event Action UpPressed;
    public event Action DownPressed;
    public event Action LeftPressed;
    public event Action RightPressed;

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerControl>();
    }
    private void Update()
    {
        if (!Conductor.instance.IsPaused())
        {
            player.RotateCharacter();

            if (Input.GetKeyDown(fire1))
            {
                player.Shoot(player.cannon1);
            }

            if (Input.GetKeyDown(up))
            {
                //Debug.Log("Up Pushed on loop beat:" + Conductor.instance.loopPosInBeats.ToString());

                UpPressed?.Invoke();
            }

            if (Input.GetKeyDown(right))
            {
                //Debug.Log("Right Pushed on loop beat:" + Conductor.instance.loopPosInBeats.ToString());
                RightPressed?.Invoke();
            }

            if (Input.GetKeyDown(down))
            {
                //Debug.Log("Down Pushed on loop beat:" + Conductor.instance.loopPosInBeats.ToString());
                DownPressed?.Invoke();
            }

            if (Input.GetKeyDown(left))
            {
                //Debug.Log("Left Pushed on loop beat:" + Conductor.instance.loopPosInBeats.ToString());
                LeftPressed?.Invoke();
            }

            if (Input.GetKeyDown(fire2))
            {
                player.Shoot(player.cannon2);
            }

            if (Input.GetKeyDown(fire3))
            {
                player.Shoot(player.cannon3);
            }

            if (Input.GetKeyDown(fire4))
            {
                player.Shoot(player.cannon4);
            }

            
            if (Input.GetKey(moveUp))
            {
                player.MoveUp();
            }
            if (Input.GetKey(moveDown))
            {
                player.MoveDown();
            }


            if (Input.GetKeyDown(pause))
            {
                {
                    if (!Conductor.instance.IsPaused())
                    {
                        Conductor.instance.Pause();
                    }
                    else
                    {
                        Conductor.instance.Resume();
                    }
                }
            }
        }
    }
}