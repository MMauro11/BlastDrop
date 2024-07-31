using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipController : MonoBehaviour
{
    //number of beats in each loop
    public float beatsPerLoop;
    public float clipBpm;
    private float secPerBeat;
    private float dspSongTime;
    public float loopPositionInBeats;
    AudioSource audioSource;

    public float initialVolume;
    public float maxVolume;

    // Start is called before the first frame update
    void Start()
    {
        loopPositionInBeats = 0;
        //Load the AudioSource attached to the Conductor GameObject
        audioSource = GetComponent<AudioSource>();

        //calculate Bpm based on clip length and beat per loop, it can cause Sync problems with other sources
        //clipBpm = beatsPerLoop * 4 / audioSource.clip.length * 60;

        //Calculate the number of seconds in each beat
        secPerBeat = 60f / clipBpm;

        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        Conductor.instance.Beat += BeatCounter;

    }

    // Update is called once per frame
    private void BeatCounter()
    {
        if(loopPositionInBeats == 0)
        { audioSource.Play();}
        loopPositionInBeats++;
        if (loopPositionInBeats > beatsPerLoop - 1)
        { loopPositionInBeats = 0; }    
    }
}
