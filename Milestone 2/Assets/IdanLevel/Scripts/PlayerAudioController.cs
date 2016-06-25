using UnityEngine;
using System.Collections;

public class PlayerAudioController : MonoBehaviour {
    public PlayerController pc;

    private AudioSource ghz;
    private AudioSource grass;

    private bool bPlaying1 = false;
    private bool bPlaying2 = false;


    // Use this for initialization
    void Start () {
        ghz = GetComponents<AudioSource>()[0];
        grass = GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update () {
	    if (pc.forwardSpeed > 3 && !bPlaying1)
        {
            print("Playing");
            ghz.Play();
            bPlaying1 = true;
        } 
        if (pc.forwardSpeed == 0){
            ghz.Stop();
            bPlaying1 = false;
        }
        if (pc.forwardSpeed > 6 && !bPlaying2)
        {
            print("Playing");
            grass.Play();
            bPlaying2 = true;
        }
        if (pc.forwardSpeed == 0)
        {
            grass.Play();
            bPlaying2 = false;
        }
    }
}
