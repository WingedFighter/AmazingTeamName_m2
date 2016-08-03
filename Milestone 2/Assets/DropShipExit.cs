using UnityEngine;
using System.Collections;

public class DropShipExit : MonoBehaviour {
	public GameObject dropShip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.tag == "Player") {
			Destroy (dropShip);
		}
	}
}
