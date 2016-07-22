using UnityEngine;
using System.Collections;

public class PlayerControllerAlpha : MonoBehaviour {

	// making this a public var so we can see it in gui
	public bool useRootMotion;

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

	// animator state and transition name hashes
	static int LOCOMOTION_STATE = Animator.StringToHash("base.locomotion");
	static int JUMP_STATE = Animator.StringToHash("base.jump");
	static int SLIDE_STATE = Animator.StringToHash("base.running_slide");
	static int IDLE_STATE = Animator.StringToHash("base.idle");
	static int FALLING_STATE = Animator.StringToHash("base.falling");

	static int IDLE_TO_LOCOMOTION_TRANS = Animator.StringToHash("base.idleToLocomotion");
	static int LOCOMOTION_TO_IDLE_TRANS = Animator.StringToHash("base.locomotionToIdle");
	static int LOCOMOTION_TO_JUMP_TRANS = Animator.StringToHash("base.locomotionToJump");
	static int JUMP_TO_FALLING_TRANS = Animator.StringToHash("base.jumpToFall");
	static int SLIDE_TO_LOCOMOTION_TRANS = Animator.StringToHash("base.slideToLocomotion");
	static int LOCOMOTION_TO_SLIDE_TRANS = Animator.StringToHash("base.locomotionToSlide");
	static int FALL_TO_LOCOMOTION_TRANS = Animator.StringToHash("base.fallToLocomotion");

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
	// Note that becuase we're applying these to a value less than one, smaller numbers result in greater acceleration
	private static float SLOPE_FLAT_EXPONENT = 3f;
	private static float SLOPE_DOWNHILL_EXPONENT = 1f;
	private static float SLOPE_UPHILL_EXPONENT = 10f;
	public float accelerationExponent = SLOPE_FLAT_EXPONENT;

	public int currentAnimationStateInt;
	public string currentAnimationStateString;
	public bool liftingOff = false;

	// Use this for initialization
	void Start () {

        animator = GetComponent<Animator>();
		myRigidBody = GetComponent<Rigidbody>();
		myCapsuleCollider = GetComponent<CapsuleCollider>();

        disableRagdoll();

		time = 0;
		ticks = 0;
		velocitySum = 0;
		animVelocitySum = 0;
	}


	void FixedUpdate() {

		// much depends on if we are gounded
		grounded = getGrounded();

		// here just updating this in the gui for debugging
		// if successful, this will stay in sync with grounded bool
		useRootMotion = animator.applyRootMotion;

		if (bRagdoll) {
			// logic to transition out of ragdoll
		} else { // we are in an animation state

			// figure out which animator state the player is in
			if (animator.IsInTransition(0)) {
				currentAnimationStateInt = animator.GetAnimatorTransitionInfo(0).nameHash;
			} else {
				currentAnimationStateInt = animator.GetCurrentAnimatorStateInfo(0).nameHash;
			}
			currentAnimationStateString = getAnimationStateString(currentAnimationStateInt);

			// now what to do for each state
			if (currentAnimationStateInt == IDLE_STATE) {
				if (grounded) {
					setRootMotion(true);
					proccessInput();
				} else {
					setRootMotion(false);
				}
			} else if (currentAnimationStateInt == LOCOMOTION_STATE) {
				if (grounded) {
					setRootMotion(true);
					proccessInput();
					animator.speed = Mathf.Max(1, forwardSpeed * 3);
				} else {
					setRootMotion(false);
				}
			} else if (currentAnimationStateInt == JUMP_STATE) {
			} else if (currentAnimationStateInt == FALLING_STATE) {
			} else if (currentAnimationStateInt == SLIDE_STATE) {
			}
//			} else if (currentAnimationState == ) {
		}
	}

	private void proccessInput() {
		// check if should go ragdoll
		if (Input.GetKeyDown(KeyCode.Tab)) {
			enableRagdoll();

		// check if should jump or slide
		} else if (Input.GetKeyDown(KeyCode.Space)) {
			doJump();
		} else if (Input.GetKeyDown(KeyCode.X)) {
			doSlide ();

		// else we are checking if we should locomote
		} else {
			processAxisInput();
		}
	}

	private void processAxisInput() {

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
		animator.SetFloat(FORWARD, forwardSpeed);
	}

		/*
	void FixedUpdate () {
		grounded = getGrounded();
		animator.applyRootMotion = (grounded & animator.GetCurrentAnimatorStateInfo(0).IsName(LOCOMOTION_STATE));
		if (animator.GetCurrentAnimatorStateInfo(0).IsName(JUMP_STATE)) {
			Debug.Log("in jump state");
			if (!liftingOff && grounded) {
				animator.SetTrigger("land");

			} else if (liftingOff && !grounded) {
				liftingOff = false;
			}
		}
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

			// jump
			if (Input.GetKeyDown(KeyCode.Space)) {
				jumpForce = new Vector3(0, jumpForceY, 0);
				animator.applyRootMotion = false;
				animator.SetTrigger("Jump");
				liftingOff = true;
				myRigidBody.AddForce(jumpForce);
//				animator.applyRootMotion = true;
			}

			if (Input.GetKeyDown(KeyCode.X)) {
				animator.applyRootMotion = false;
				animator.SetTrigger("Slide");
			}
        }

		// update the parameter that controls transitions between idle and locomotion
        animator.SetFloat(FORWARD, forwardSpeed);
		// update the param that controls how fast the animation plays (makes the guy run faster)
		// but only for the locomotion state
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")) {
			animator.speed = Mathf.Max(1, forwardSpeed * 3);
		} else {
			animator.speed = 1f;
		}

	}
	*/


	private void doJump() {
	}

	private void doSlide() {
	}

	private void doTheLocomotion() {
	}

	private string getAnimationStateString(int stateInt) {

		if (stateInt == LOCOMOTION_STATE) return "LOCOMOTION";
		if (stateInt == JUMP_STATE) return	"JUMP";
		if (stateInt == SLIDE_STATE) return "SLIDE";
		if (stateInt == IDLE_STATE) return "IDLE";
		if (stateInt == FALLING_STATE) return "FALLING";

		if (stateInt == IDLE_TO_LOCOMOTION_TRANS) return "IDLE_TO_LOC";
		if (stateInt == LOCOMOTION_TO_IDLE_TRANS) return "LOC_TO_IDLE";
		if (stateInt == LOCOMOTION_TO_JUMP_TRANS) return "LOC_TO_JUMP";
		if (stateInt == JUMP_TO_FALLING_TRANS) return "JUMP_TO_FALL";
		if (stateInt == SLIDE_TO_LOCOMOTION_TRANS) return "SLIDE_TO_LOC";
		if (stateInt == LOCOMOTION_TO_SLIDE_TRANS) return "LOC_TO_SLIDE";
		if (stateInt == FALL_TO_LOCOMOTION_TRANS) return "FALL_TO_LOC";

		return "UNKNOWN";
	}

	private void doVelocityDebugging() {
		time += Time.deltaTime;
		if (time > 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")) {
			ticks ++;
			if (ticks > 10) {
				velocitySum += myRigidBody.velocity.magnitude;
				animVelocitySum += animator.velocity.magnitude;
				float avgVel = velocitySum/(ticks -10);
				float avgAnimVel = animVelocitySum/(ticks - 10);
				Debug.Log(
					"forwardSpeed: " + forwardSpeed + " // " +
					"velocityAvg: " + avgVel + " // " +
					"animVelAvg:" + avgAnimVel
				);
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
//			Debug.Log(hit.collider.gameObject.tag);
			setSurface(hit.collider.gameObject.tag);
			// only consider the ground layer 
			return (hit.collider.gameObject.layer == GROUND_LAYER);
		} else {
			return false;
		}
	}

	// Sets the surface slope acceleration exponent depending according to the surfaceTag param
	private void setSurface(string surfaceTag) {
		if (surfaceTag.Equals(UPHILL_TAG)) {
			accelerationExponent = SLOPE_UPHILL_EXPONENT;
		} else if (surfaceTag.Equals(DOWNHILL_TAG)) {
			accelerationExponent = SLOPE_DOWNHILL_EXPONENT;
		} else {
			accelerationExponent = SLOPE_FLAT_EXPONENT;
		}
	}

	// Wrapper method for setting root motion 
	private void setRootMotion(bool b) {
		animator.applyRootMotion = b;
	}

}
