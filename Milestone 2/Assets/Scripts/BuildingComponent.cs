using UnityEngine;
using System.Collections;

public class BuildingComponent : MonoBehaviour {
    public float MovementVelocityThreshold = 0.5f;
    public Vector3 OriginalLocation;
	private static int GROUND_LAYER = 10;
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
		/* use this as an alternate for the above -- works better for single-object towers
		return !rb.IsSleeping();
		*/
	}

    public void OnCollisionEnter(Collision other)
    {
        AudioSource a = GetComponent<AudioSource>();
        if (!a.isPlaying)
        {
            a.Play();
        }
    }

	/*  use this if you want building to vanish when it falls over

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == GROUND_LAYER) {
			gameObject.SetActive(false);
		}
	}
	 */
}
