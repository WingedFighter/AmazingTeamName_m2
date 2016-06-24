using UnityEngine;
using System.Collections;

public class Hammer : MonoBehaviour {

    private AudioSource audioSrc;

	// Use this for initialization
	void Start () {
        audioSrc = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        audioSrc.Play();
    }
}
