using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Raise Beat and Bars events based on the rhytm of the AudioSource "metronomeSource".
/// Specifing metronome BeatsPerLoop and Bpm is needed. It assumes 4/4 signature.
/// </summary>
public class Conductor : MonoBehaviour
{

    //Song beats per minute
    //This is determined by the song you're trying to sync up to
    public float songBpm;

    //The number of seconds for each song beat
    [SerializeField] private float secPerBeat;

    //Current song position, in seconds
    private float songPosition;

    //Current song position in beats. NOTE: it start from 0, so on 4/4 signature, the 4 beat is equal to 3 in this variable.
    private float songPositionInBeats;

    //How many seconds have passed since the song started
    private float dspSongTime;

    //an AudioSource attached to this GameObject that will play the music.
    [SerializeField] private AudioSource metronomeSource;
    [SerializeField] private AudioSource enemyDeathSource;
    [SerializeField] private AudioSource multDown;
    [SerializeField] private AudioSource multUp;

    //List of AudioSources of the multiplier audio clip
    [SerializeField] public AudioSource[] MultClips;

    //counter of actual multiplier
    private int actualMult = -1;
    public int ActualMultiplier { get { return actualMult; } }


    //The offset to the first beat of the song in seconds
    public float firstBeatOffset;

    //auxiliary value to calculate next beat time
    private float nextBeat;

    //the number of beats in each loop
    public float beatsPerLoop;

    //the total number of loops completed since the looping clip first started
    public int completedLoops = 0;

    //The current position of the song within the loop in beats. NOTE: beats are counted starting from 0 to 3
    private float loopPositionInBeats;
    private int lastLoopPositionInBeats;

    //The current relative position of the song within the loop measured between 0 and 1.
    public float loopPositionInAnalog;

    //Conductor static instance
    public static Conductor instance;

    //Pause information
    private static bool paused = false;
    private static float pauseTimeStamp = -1f;
    private static float pausedTime = 0;
    public GameObject PauseCanvas;

    //input tolerance (seconds) relative to precise beat
    public float tolerance;
    private float hitPrecision;

    //Event invoked every beat start
    public event Action Beat;
    public event Action Bar;

    //list of AudioSource clips to play on next Bar event
    List<AudioSource> NextBarClips = new List<AudioSource>();

    //save the initial timeScale for pause purposes
    float initialTimeScale;

    // Start is called before the first frame update
    void Start()
    {
        SilenceAllMultipliers();
        //paused = false;
        //pauseTimeStamp = -1f; //-1f mean "not managed"
        loopPositionInBeats = 0;
        //Load the AudioSource attached to the Conductor GameObject
        metronomeSource = GetComponent<AudioSource>();

        //calculate Bpm based on clip length and beat per loop, it can cause Sync problems with other sources
        //songBpm = beatsPerLoop / metronomeSource.clip.length * 60;

        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        //start metronome
        metronomeSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //determine how many seconds since the song started, considering Offset
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;

        //calculate the loop position
        if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
        {
            metronomeSource.Play();
            completedLoops++;
        }

        //position relative to the loop
        lastLoopPositionInBeats = BeatCounter();
        loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;

        //Beat Invoke
        if (lastLoopPositionInBeats == beatsPerLoop - 1 && BeatCounter() == 0)
        {
            //raising Beat event
            Beat?.Invoke();
            lastLoopPositionInBeats = 0;

            //invoke a Bar event
            if (BeatCounter() == 0)
            {
                Bar?.Invoke();
                if (NextBarClips.Count != 0)
                {
                    NextBarClipsPlay();
                }
            }
        }
        if (BeatCounter() > lastLoopPositionInBeats)
        {   //raising Beat event
            Beat?.Invoke();
        }


        //position relative to the beat 
        loopPositionInAnalog = loopPositionInBeats - lastLoopPositionInBeats;
    }

    void Awake()
    { instance = this; }

    public int BeatCounter()
    {
        return (int)loopPositionInBeats;
    }

    public void Pause()
    {
        paused = true;
        AudioListener.pause = true;
        pauseTimeStamp = (float)AudioSettings.dspTime;
        //Activate pause UI
        PauseCanvas.SetActive(true);
        initialTimeScale = Time.timeScale;
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        paused = false;
        AudioListener.pause = false;
        //Deactivate pause UI
        PauseCanvas.SetActive(false);
        pauseTimeStamp = -1f;

        //restore time
        Time.timeScale = initialTimeScale;

        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsPaused() { return paused; }

    public bool IsOnBeat()
    {
        if ((loopPositionInAnalog < 0 + tolerance) || loopPositionInAnalog > 1 - tolerance)
        {
            return true;
        }
        else return false;
    }

    public float SecPerBeat
    {
        get { return secPerBeat; }
    }

    public void SilenceAllMultipliers()
    {
        AudioSource clip;
        for(int i=0; i<MultClips.Length; i++)
        {
            clip = MultClips[i];
            SilenceMultiplier(clip);
        }
    }

    public void PlayEnemyDeath()
    { enemyDeathSource.Play(); }

    public void PlayNextMultip()
    {
        if (actualMult >=-1 && actualMult <MultClips.Length)
        {
            actualMult++;
            PlayMultUp();

            //play multiplier clip from next bar
            //PlayOnNextBar(MultClips[actualMult]);

            //Fade in audio clip
            StartCoroutine(FadeAudioSource.StartFade(MultClips[actualMult], SecPerBeat * 2, MultClips[actualMult].GetComponent<ClipController>().maxVolume));
        }
    }

    /// <summary>
    /// Play the audioSource clip starting from the next Bar event
    /// </summary>
    /// <param name="audioSource"></param>
    public void PlayOnNextBar(AudioSource audioSource)
    {
        NextBarClips.Add(audioSource);
    }

    private void NextBarClipsPlay()
    {
        foreach (var clip in NextBarClips)
        {
            if (!clip.isPlaying)
            { clip.Play(); }

            //fade in audio clip
            StartCoroutine(FadeAudioSource.StartFade(clip, SecPerBeat*2, clip.GetComponent<ClipController>().maxVolume));
        }
        NextBarClips.Clear();
    }

    /// <summary>
    /// Trigger Multiplier Down, silencing last multiplier audio clip
    /// </summary>
    public void MultDown() 
    {
        SilenceMultiplier(MultClips[actualMult]);
        PlayMultDown();
        actualMult--;
    }

    private void PlayMultUp() 
    { 
        multUp.Play();
    }
    private void PlayMultDown()
    { 
        multDown.Play();
    }

    //Get the clip volume to the initial volume
    private void SilenceMultiplier(AudioSource clip)
    {
        clip.volume = clip.GetComponent<ClipController>().initialVolume;
        StartCoroutine(FadeAudioSource.StartFade(clip, SecPerBeat * 2, clip.GetComponent<ClipController>().initialVolume));
    }
}
