using UnityEngine;
using System.Collections;

public class DeathPlane : MonoBehaviour {
    public bool Triggered = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Triggered = true;
        }
    }
}
