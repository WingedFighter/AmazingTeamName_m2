using UnityEngine;
using System.Collections;

public class Ring : MonoBehaviour {

    private AudioSource audioSource;
    private Rigidbody body;
    private bool bActivated = false;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        print("Collision");
        if (!bActivated)
        {
            audioSource.Play();
            body.useGravity = true;
            bActivated = true;
        }
    }
}
