using UnityEngine;
using System.Collections;

public class BuildingComponent : MonoBehaviour {
    public float MovementVelocityThreshold = 0.25f;
    public Vector3 OriginalLocation;

    private Rigidbody rb;
	// Use this for initialization
	void Start () {
        OriginalLocation = transform.position;
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

    }

    public bool IsMoving()
    {
        return (rb.velocity.magnitude > MovementVelocityThreshold);
    }
}
