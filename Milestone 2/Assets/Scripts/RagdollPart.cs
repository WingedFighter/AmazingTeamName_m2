using UnityEngine;
using System.Collections;

public class RagdollPart : MonoBehaviour {

    public PlayerController playerController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        playerController.RagdollCollisionHandler(collision);
    }

    void OnTriggerEnter(Collider other)
    {
        playerController.RagdollTriggerHandler(GetComponent<Collider>());
    }
}
