using UnityEngine;
using System.Collections;

public class PlayerControllerAlpha : MonoBehaviour {


	public float decelerationRate = 5f;
    public float HorizontalSpeed = 2f; 
	public float forwardSpeed = 0f;
	private Vector3 speed;
	public float time;
	public float velocitySum;
	public float animVelocitySum;
	public int ticks;

	private Animator animator;
	private CapsuleCollider myCapsuleCollider;
	private Rigidbody myRigidBody;

    public bool bRagdoll = false;

	// referencing animator params
	static string FORWARD = "forward";
	static string LATERAL = "lateral";

	// public so we can see what's happening
	public Vector3 addForce;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
		myRigidBody = GetComponent<Rigidbody>();
		myCapsuleCollider = GetComponent<CapsuleCollider>();

        disableRagdoll();

		addForce = new Vector3();

		time = 0;
		ticks = 0;
		velocitySum = 0;
		animVelocitySum = 0;
	}
	
	void FixedUpdate () {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			if (bRagdoll) {
			 	disableRagdoll();
			} else {
				enableRagdoll();
			}
		}
	
        if (!bRagdoll)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                // Exponentially decay acceleration with respect to speed
//                float speedInc = Mathf.Pow(1 - forwardSpeed, 3f);
//                forwardSpeed += speedInc * Time.deltaTime;
//				addForce.z = 1f;
//				myRigidBody.AddRelativeForce(addForce);
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                // deceleration rate is also exponential with respect to speed
                float speedDec = 1 - forwardSpeed;
                forwardSpeed = Mathf.Max(
                    forwardSpeed - (speedDec * Time.deltaTime),
                    0
                );
            }
        }

        // Update Animation
//        animator.SetFloat(FORWARD, forwardSpeed);

		time += Time.deltaTime;
		if (time > 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")) {
			ticks ++;
			velocitySum += myRigidBody.velocity.magnitude;
			animVelocitySum += animator.velocity.magnitude;
			float avgVel = velocitySum/ticks;
			float avgAnimVel = animVelocitySum/ticks;
//			Debug.Log(
//				"forwardSpeed: " + forwardSpeed + " // " +
//				"animator.velocity: " + animator.velocity + " // " +
//				"rigidbody.velocity" + myRigidBody.velocity
//			);
			Debug.Log(
				"forwardSpeed: " + forwardSpeed + " // " +
				"velocityAvg: " + avgVel + " // " +
				"animVelAvg:" + avgAnimVel
			);
			time = 0f;
		}
	}


    private void enableRagdoll()
    {
        animator.enabled = false;
        animator.Rebind();

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
			rb.isKinematic = false;
			rb.detectCollisions = true;
        }

		myRigidBody.isKinematic = true;
		myRigidBody.detectCollisions = false;

		bRagdoll = true;
    }

    private void disableRagdoll()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
			rb.detectCollisions = false;
        }

		myRigidBody.isKinematic = false;
		myRigidBody.detectCollisions = true;

        animator.Rebind();
        animator.enabled = true;

        animator.SetTrigger("GetUp");
		bRagdoll = false;
    }
}
