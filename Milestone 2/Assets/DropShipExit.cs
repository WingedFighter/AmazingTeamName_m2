using UnityEngine;
using System.Collections;

public class DropShipExit : MonoBehaviour {
	public DropShipController dropShipController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.tag == "Player") {
			print ("ok");
			dropShipController.Explode ();
			dropShipController.gameObject.SetActive (false);
		}
	}
}
