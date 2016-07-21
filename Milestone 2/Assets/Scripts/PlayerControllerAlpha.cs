using UnityEngine;
using System.Collections;

public class PlayerControllerAlpha : MonoBehaviour {

	// jumping experiment
	private float jumpForceY = 10000f;
	private Vector3 jumpForce;

	public float decelerationRate = 5f;
    public float HorizontalSpeed = 2f; 
	public float forwardSpeed = 0f;

	private Vector3 speed;
	public float time;

	// these three just for debugging
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

	// Layers
	private static int OBSTACLES_LAYER = 8;
	private static int GROUND_LAYER = 10;
	private static int PLAYER_LAYER = 9;

	// are we on the ground
	public bool grounded;

	// the spherecast will ignore the player layer (don't want to hit colliders on feet)
	private int sphereColliderLayerMask = ~(1 << PLAYER_LAYER);

	// Ground slope tags
	private static string UPHILL_TAG = "uphill";
	private static string DOWNHILL_TAG = "downhill";
	private static string FLAT_TAG = "flat";

	// Speed increase expoonents corresponding to Ground slope Tags
	// Note that becuase we're applying these to fraction, smaller numbers result in greater acceleration
	private static float SLOPE_FLAT_EXPONENT = 3f;
	private static float SLOPE_DOWNHILL_EXPONENT = 1f;
	private static float SLOPE_UPHILL_EXPONENT = 10f;
	public float accelerationExponent = SLOPE_FLAT_EXPONENT;

	// Use this for initialization
	void Start () {


		Debug.Log(sphereColliderLayerMask);
        animator = GetComponent<Animator>();
		myRigidBody = GetComponent<Rigidbody>();
		myCapsuleCollider = GetComponent<CapsuleCollider>();

        disableRagdoll();

		time = 0;
		ticks = 0;
		velocitySum = 0;
		animVelocitySum = 0;
	}

	void FixedUpdate () {
		grounded = getGrounded();
		// this if is just a debug block
		if (Input.GetKeyDown(KeyCode.G)) {
			if (grounded) {
				Debug.Log("grounded");
			} else {
				Debug.Log("notGrounded");
			}
		}


		if (Input.GetKeyDown(KeyCode.Tab) && !animator.IsInTransition(0)) {
			if (bRagdoll) {
			 	disableRagdoll();
			} else {
				enableRagdoll();
			}
		}
	
		if (!bRagdoll 
			&& grounded
			&&  (  animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")
				|| animator.GetCurrentAnimatorStateInfo(0).IsName("idle")))
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                // Exponentially decay acceleration with respect to speed
				float speedInc = Mathf.Pow(1 - forwardSpeed, accelerationExponent);
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

			if (Input.GetKeyDown(KeyCode.Space)) {
				jumpForce = new Vector3(0, jumpForceY, 0);
				animator.applyRootMotion = false;
				myRigidBody.AddForce(jumpForce);
				animator.applyRootMotion = true;
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
		if (myCollision.collider.gameObject.layer == OBSTACLES_LAYER) {
//			Debug.Log("col ENTER  vel = " + myRigidBody.velocity.magnitude);
			slowDownOnCollision(myCollision);
		}
	}

	void OnCollisionExit(Collision myCollision) {
		if (myCollision.collider.gameObject.layer == OBSTACLES_LAYER) {
//			Debug.Log("col EXIT   vel = " + myRigidBody.velocity.magnitude);
			slowDownOnCollision(myCollision);
		}
	}

	private void slowDownOnCollision(Collision myCollision) {
		float myVel = myRigidBody.velocity.magnitude;
		forwardSpeed = myVel/12.6f;
	}

	private bool getGrounded() {
		// Define a Ray from the center of the capsule collider straight down.
		Vector3 origin = myCapsuleCollider.transform.position 
			+ new Vector3(0, 0.5f * myCapsuleCollider.height, 0);
		Vector3 direction = Vector3.down;
		Ray ray = new Ray(origin, direction);


		float sphereRadius = myCapsuleCollider.radius;
		float castDistance = 0.5f * myCapsuleCollider.height;

		// Cast a sphere the radius of the capsule collider
		// from the center of the capusule collider straight down,
		// to the point at which the sphere's center is at the player's feet
		// and detect if it hits something not on the PLAYER_LAYER 
		RaycastHit hit;
		if (Physics.SphereCast(ray, sphereRadius, out hit, castDistance, sphereColliderLayerMask)) {
//			Debug.Log(hit.collider.gameObject.name);
			Debug.Log(hit.collider.gameObject.tag);
			setSurface(hit.collider.gameObject.tag);
			// only consider the ground layer 
			return (hit.collider.gameObject.layer == GROUND_LAYER);
		} else {
			return false;
		}
	}

	/*
	 * Sets the surface slope acceleration exponent depending according to the surfaceTag param
	 */
	private void setSurface(string surfaceTag) {
		if (surfaceTag.Equals(UPHILL_TAG)) {
			accelerationExponent = SLOPE_UPHILL_EXPONENT;
		} else if (surfaceTag.Equals(DOWNHILL_TAG)) {
			accelerationExponent = SLOPE_DOWNHILL_EXPONENT;
		} else {
			accelerationExponent = SLOPE_FLAT_EXPONENT;
		}
	}
}
