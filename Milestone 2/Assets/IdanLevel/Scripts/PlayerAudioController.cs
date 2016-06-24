using UnityEngine;
using System.Collections;

public class PlayerAudioController : MonoBehaviour {
    public PlayerController pc;

    private AudioSource audioSrc;
    private bool bPlaying = false;

	// Use this for initialization
	void Start () {
        audioSrc = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (pc.forwardSpeed > 3 && !bPlaying)
        {
            print("Playing");
            audioSrc.Play();
            bPlaying = true;
        } 
        if (pc.forwardSpeed == 0){
            audioSrc.Stop();
            bPlaying = false;
        }
	}
}
