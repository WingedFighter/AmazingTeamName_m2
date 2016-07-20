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
	public bool bDead = false;

	// referencing animator params
	static string FORWARD = "forward";
	static string LATERAL = "lateral";

	// public so we can see what's happening
	public Vector3 addForce;

	private static int OBSTACLES = 8;

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
		if (Input.GetKeyDown(KeyCode.Tab) && !animator.IsInTransition(0)) {
			if (bRagdoll) {
			 	disableRagdoll();
			} else {
				enableRagdoll();
			}
		}
	
		if (!bRagdoll 
			&&  (  animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")
				|| animator.GetCurrentAnimatorStateInfo(0).IsName("idle")))
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                // Exponentially decay acceleration with respect to speed
                float speedInc = Mathf.Pow(1 - forwardSpeed, 3f);
                forwardSpeed += speedInc * Time.deltaTime;
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                // deceleration rate is also exponential with respect to speed
                float speedDec = 1 - forwardSpeed;
				float tempForwardSpeed = forwardSpeed - (speedDec * Time.deltaTime);
				// no walking backwards
                forwardSpeed = Mathf.Max(
					tempForwardSpeed,
                   	0f
                );
            }
        }

//		forwardSpeed = 0.66f;
		// update the parameter that controls transitions between idle and locomotion
        animator.SetFloat(FORWARD, forwardSpeed);
		// update the param that controls how fast the animation plays (makes the guy run faster)
		// but only for the locomotion state
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")) {
			animator.speed = Mathf.Max(1, forwardSpeed * 3);
		} else {
			animator.speed = 1f;
		}

		time += Time.deltaTime;
		if (time > 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")) {
			ticks ++;
			if (ticks > 10) {
				velocitySum += myRigidBody.velocity.magnitude;
				animVelocitySum += animator.velocity.magnitude;
				float avgVel = velocitySum/(ticks -10);
				float avgAnimVel = animVelocitySum/(ticks - 10);
//				Debug.Log(
//					"forwardSpeed: " + forwardSpeed + " // " +
//					"velocityAvg: " + avgVel + " // " +
//					"animVelAvg:" + avgAnimVel
//				);
			}
			time = 0f;
		}
	}


    private void enableRagdoll()
    {
		forwardSpeed = 0f;
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

	void OnCollisionEnter(Collision myCollision) {
		if (myCollision.collider.gameObject.layer == OBSTACLES) {
			Debug.Log("col ENTER  vel = " + myRigidBody.velocity.magnitude);
			slowDownOnCollision(myCollision);
		}
//		Debug.Log("COL" + myCollisoin.impulse);
//		float myVel = myRigidBody.velocity.magnitude;
//		Debug.Log(myVel);
//		forwardSpeed = myVel/12.6f;
	}

	void OnCollisionExit(Collision myCollision) {
		Debug.Log("col EXIT   vel = " + myRigidBody.velocity.magnitude);
		slowDownOnCollision(myCollision);
//		Debug.Log("COL" + myCollisoin.impulse);
//		float myVel = myRigidBody.velocity.magnitude;
//		Debug.Log(myVel);
//		forwardSpeed = myVel/12.6f;
	}

	private void slowDownOnCollision(Collision myCollision) {
//		Debug.Log("COL" + myCollision.impulse);
		float myVel = myRigidBody.velocity.magnitude;
//		Debug.Log(myVel);
		forwardSpeed = myVel/12.6f;
	}
}
