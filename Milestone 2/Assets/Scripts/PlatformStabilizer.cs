using UnityEngine;
using System.Collections;

public class PlatformStabilizer : MonoBehaviour {

	public Vector3 myRotion;
	// Use this for initialization
	void Start () {
		myRotion = new Vector3(0, 0, 270);
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = myRotion;
	}

	void LateUpdate() {

	}
}
