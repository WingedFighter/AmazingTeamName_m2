using UnityEngine;
using System.Collections;

public class PlayerControllerAlpha : MonoBehaviour {
    private Vector3 StartingLocation;

	// making this a public var so we can see it in gui
	public bool useRootMotion;
	public float knockoutForce = 2000;

	public float matchSpeedDivisor = 22;
	public float myZVelocity;
	public float animatorSpeed;

	// jumping experiment
	private float jumpForce = 40000f;
	private float jumpBoostForce = 40000f;
	private float slideForce = 30000f;
    static float idleToSlideForce = 200000f;
	private float currentIdleToSlideForce = idleToSlideForce;
	private float decelerationRate = 5f;
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
	// we need this to know where to get up from after ragdolling
	private Rigidbody myHipsRigidBody;
	private Material myMaterial;

	public bool bRagdoll = false;
	public bool bGettinUp = false;
	// keep track of how long we've been a ragdoll.
	public float ragdollDuration = 0f;
	public bool bDead = false;
	public bool hitBuilding = false;

	// referencing animator params
	static string FORWARD = "forward";
	static string LATERAL = "lateral";
	static string COLLIDER_HEIGHT = "colliderHeight";
	public float myColliderHeight;
	private float myOriginalColliderHeight;
	private float myOriginalColliderCenterY = .89f;
    private Vector3 myOriginalColliderCenter;

	public float previousYRotation = 0;

	// animator state and transition name hashes
	// states
	static int LOCOMOTION_STATE = Animator.StringToHash ("base.locomotion");
	static int JUMP_STATE = Animator.StringToHash ("base.jump");
	static int SLIDE_START_STATE = Animator.StringToHash ("base.slideStart");
	static int SLIDE_MIDDLE_STATE = Animator.StringToHash ("base.slideMiddle");
	static int SLIDE_END_STATE = Animator.StringToHash ("base.slideEnd");
	static int IDLE_STATE = Animator.StringToHash ("base.idle");
	static int FALLING_STATE = Animator.StringToHash ("base.falling");
	static int STRAFE_LEFT_STATE = Animator.StringToHash ("base.strafeLeft");
	static int STRAFE_RIGHT_STATE = Animator.StringToHash ("base.strafeRight");
	static int GETTING_UP_STATE = Animator.StringToHash ("base.getting_up");

	// transitions
	static int IDLE_TO_LOCOMOTION_TRANS = Animator.StringToHash ("idleToLocomotion");
	static int IDLE_TO_JUMP_TRANS = Animator.StringToHash ("idleToJump");
	static int LOCOMOTION_TO_IDLE_TRANS = Animator.StringToHash ("locomotionToIdle");
	static int LOCOMOTION_TO_JUMP_TRANS = Animator.StringToHash ("locomotionToJump");
	static int JUMP_TO_FALLING_TRANS = Animator.StringToHash ("jumpToFall");
	static int SLIDE_TO_LOCOMOTION_TRANS = Animator.StringToHash ("slideToLocomotion");
	static int LOCOMOTION_TO_SLIDE_TRANS = Animator.StringToHash ("locomotionToSlide");
	static int FALL_TO_LOCOMOTION_TRANS = Animator.StringToHash ("fallToLocomotion");
	static int IDLE_TO_STRAFE_LEFT_TRANS = Animator.StringToHash ("idleToStrafeLeft");
	static int IDLE_TO_STRAFE_RIGHT_TRANS = Animator.StringToHash ("idleToStrafeRight");
	static int STRAFE_LEFT_TO_IDLE_TRANS = Animator.StringToHash ("strafeLeftToIdle");
	static int STRAFE_RIGHT_TO_IDLE_TRANS = Animator.StringToHash ("strafeRightToIdle");
	static int GETTING_UP_TO_IDLE_TRANS = Animator.StringToHash ("gettingUpToIdle");
	static int SLIDE_START_TO_MIDDLE_TRANS = Animator.StringToHash ("slideStartToMiddle");
	static int SLIDE_MIDDLE_TO_END_TRANS = Animator.StringToHash ("slideMiddleToEnd");
	static int IDLE_TO_SLIDE_TRANS = Animator.StringToHash ("idleToSlide");

	// Layers
	private static int OBSTACLES_LAYER = 8;
	private static int BUILDING_LAYER = 11;
	private static int GROUND_LAYER = 10;
	private static int PLAYER_LAYER = 9;

	// are we on the ground or near it at least
	public bool grounded;
	public bool almostGrounded;
	public bool didJump = false;
	public bool goingUp = false;
	private bool slideBoostLateUpdate = false;
	private bool jumpBoostLateUpdate = false;
	public bool idleToSlideBoostLateUpdate = false;
	public bool prepareToIdleToSlideBoostLateUpdate = false;

	// the spherecast will ignore the player layer (don't want to hit colliders on feet)
	private int sphereColliderLayerMask = ~(1 << PLAYER_LAYER);

	// Ground slope tags
	private static string UPHILL_TAG = "uphill";
	private static string DOWNHILL_TAG = "downhill";
	private static string FLAT_TAG = "flat";

	// Speed increase expoonents corresponding to Ground slope Tags
	// Note that becuase we're applying these to a value less than one, smaller numbers result in greater acceleration
	private float SLOPE_FLAT_EXPONENT = 3.5f;
	private float SLOPE_DOWNHILL_EXPONENT = 1f;
	private float SLOPE_UPHILL_EXPONENT = 10f;
	private float accelerationExponent;

	public int currentAnimationStateInt;
	public string currentAnimationStateString;

    private bool footstepAudioPlaying = false;
    private AudioSource footstepsAudioSource;

    private AudioSource laserAudioSource;

    // Player Controller StateMachine
    public enum PlayerState
    {
        disabled,
        running,
        projectile, 
        dead
    }

    public PlayerState State = PlayerState.running;

    // Use this for initialization
    void Start ()
	{
		StartingLocation = transform.position;
		animator = GetComponent<Animator> ();
		myRigidBody = GetComponent<Rigidbody> ();
		myCapsuleCollider = GetComponent<CapsuleCollider> ();
		myHipsRigidBody = GameObject.FindGameObjectWithTag ("hips").GetComponent<Rigidbody> ();
		myMaterial = GameObject.FindGameObjectWithTag("skin").GetComponent<SkinnedMeshRenderer>().material;
        footstepsAudioSource = GetComponents<AudioSource>()[0];
        laserAudioSource = GetComponents<AudioSource>()[1];
		myOriginalColliderHeight = myCapsuleCollider.height;
        myOriginalColliderCenter = new Vector3(0, myOriginalColliderCenterY, 0);
	    accelerationExponent = SLOPE_FLAT_EXPONENT;


		time = 0;
		ticks = 0;
		velocitySum = 0;
		animVelocitySum = 0;

		disableRagdoll (false); // false means don't position the player at the ragdoll hips
	}

	void Update () {

		Color myColor;
		if (laserAudioSource.isPlaying) {
			myColor = new Color(
				Mathf.Lerp(myMaterial.color.r, 1f, 10 * Time.deltaTime),
				Mathf.Lerp(myMaterial.color.g, .1f, 10 *Time.deltaTime),
				Mathf.Lerp(myMaterial.color.b, 0f, 10 *Time.deltaTime)
			);
		} else {
			myColor = new Color(
				Mathf.Lerp(myMaterial.color.r, Mathf.Max(0f, 0.05f + Mathf.Pow(myZVelocity/2, 2)/400), Time.deltaTime),
				Mathf.Lerp(myMaterial.color.g, Mathf.Max(0f, 0.1f + Mathf.Pow(myZVelocity/2, 2)/100), Time.deltaTime),
				Mathf.Lerp(myMaterial.color.b, Mathf.Max(0f, 0.15f - Mathf.Pow(myZVelocity/2, 2)/400), Time.deltaTime)
			);
		}
		myMaterial.SetColor("_Color", myColor);
		myMaterial.SetColor("_EmissionColor", myColor);

	}

    void FixedUpdate()
    {
        switch (State)
        {
            case PlayerState.disabled:
                break;
            case PlayerState.running:
                FixedRunning();
                break;
            case PlayerState.projectile:
                break;
            case PlayerState.dead:
                break;
        }
    }

    void LateUpdate()
    {
        switch (State)
        {
            case PlayerState.disabled:
                break;
            case PlayerState.running:
                LateRunning();
                break;
            case PlayerState.projectile:
                break;
            case PlayerState.dead:
                break;
        }
    }

    public void Reset()
    {
        laserAudioSource.Play();
        forwardSpeed = 0;
        animator.SetTrigger("GetUp");
        transform.position = StartingLocation;
        bDead = false;
    }

	void FixedRunning ()
	{

		myZVelocity = myRigidBody.velocity.z;
		// here just updating this in the gui for debugging
		// if successful, this will stay in sync with grounded bool
		useRootMotion = animator.applyRootMotion;

		if (bRagdoll) {
			// move the rigidbody to the hips position so we can tell if grounded
			myRigidBody.position = myHipsRigidBody.position;
			// logic to transition out of ragdoll
			if (
                ragdollLongDurationElapsed() 
                || getGroundedAndSetSurface () && ragdollDurationElapsed () && !bDead
            ){
                ragdollDuration = 0f;
                ragdollLongDuration = 0f;
				disableRagdoll (true); // true means position the player at the ragdoll hips
			}
			myCapsuleCollider.height = myOriginalColliderHeight;
		} else { // we are in an animation state

			if (Input.GetKeyDown (KeyCode.Tab)) {
				enableRagdoll ();
			}

			// figure out which animator state the player is in
			else if (animator.IsInTransition (0)) {
				currentAnimationStateInt = animator.GetAnimatorTransitionInfo (0).userNameHash;
			} else {
				currentAnimationStateInt = animator.GetCurrentAnimatorStateInfo (0).fullPathHash;
			}
			currentAnimationStateString = getCurrentAnimationStateStringAndSetColliderHeight (currentAnimationStateInt);
			myCapsuleCollider.height = myOriginalColliderHeight * myColliderHeight;

			// much depends on if we are gounded or near it
			almostGrounded = getAlmostGrounded ();
			grounded = getGroundedAndSetSurface ();
			goingUp = myRigidBody.velocity.y > .5f;

			// set this to let the camera know how fast to rotate
			bGettinUp = (currentAnimationStateInt == GETTING_UP_STATE);

			// now what to do for each state
			if (currentAnimationStateInt == IDLE_STATE) {
				myRigidBody.rotation = Quaternion.identity;
				animator.speed = 1f;
				if (grounded) {
					setRootMotion (true);
					proccessInput ();
				} else {
					animator.SetTrigger ("airborne");
					setRootMotion (false);
				}
			} else if (
				currentAnimationStateInt == LOCOMOTION_STATE
				||
				currentAnimationStateInt == LOCOMOTION_TO_IDLE_TRANS
				||
				currentAnimationStateInt == IDLE_TO_LOCOMOTION_TRANS
			) {
				if (grounded) {
					setRootMotion (true);
					proccessInput ();
					animator.speed = Mathf.Max (1, forwardSpeed * 5);
				} else if (almostGrounded) {
					setRootMotion (false);
				} else {
					setRootMotion (false);
					animator.SetTrigger ("airborne");
				}
			} else if (currentAnimationStateInt == LOCOMOTION_TO_JUMP_TRANS
			           ||
			           currentAnimationStateInt == IDLE_TO_JUMP_TRANS) {
				setRootMotion (false);

				if (Input.GetKey (KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0)) {
					jumpBoostLateUpdate = true;
				}
			} else if (currentAnimationStateInt == JUMP_STATE) {
				setRootMotion (false);

				if (Input.GetKey (KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0)) {
					jumpBoostLateUpdate = true;
				}
			} else if (currentAnimationStateInt == JUMP_TO_FALLING_TRANS) {
				setRootMotion (false);

				if (almostGrounded && !goingUp) {
					animator.SetTrigger ("land");
					didJump = false;
				} else if (  // if still going upwards, apply jump boost if space is pressed
					(Input.GetKey (KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
                    && didJump
					&& goingUp) {
					jumpBoostLateUpdate = true;
				}
			} else if (currentAnimationStateInt == FALLING_STATE) {
				setRootMotion (false);

				if (almostGrounded && !goingUp) {
					animator.SetTrigger ("land");
					didJump = false;
				} else if (  // if still going upwards, apply jump boost if space is pressed
					(Input.GetKey (KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
                    && didJump
					&& goingUp) {
					jumpBoostLateUpdate = true;
				}
			} else if (currentAnimationStateInt == SLIDE_START_STATE) {
                    if (animator.speed > 1) {
                        float reduction = (animator.speed - 1);
                        animator.speed -= Mathf.Min((reduction * 2 * Time.deltaTime), 1f);
                    }
                    myRigidBody.velocity = myLastVelocity;
                    setRootMotion (false);
                    slideBoostLateUpdate = true;
                    if (!almostGrounded) {
                        animator.SetTrigger ("airborne");
                    }
			} else if (currentAnimationStateInt == SLIDE_MIDDLE_STATE) {
			    idleToSlideBoostLateUpdate = false;
				currentIdleToSlideForce = idleToSlideForce;
				if (forwardSpeed < 1) {
					slideBoostLateUpdate = true;
				}
				setRootMotion (false);
				if (!grounded) {
				}
				if (!almostGrounded) {
					animator.SetTrigger ("airborne");
				} else if (!(Input.GetKey (KeyCode.X) || Input.GetKey(KeyCode.JoystickButton2))) {
					animator.SetTrigger ("endSlide");
					matchAnimatorSpeedToVelocity ();
				}
			} else if (currentAnimationStateInt == SLIDE_END_STATE) {
				setRootMotion (false);
				if (!almostGrounded) {
					animator.SetTrigger ("airborne");
				}
			} else if (currentAnimationStateInt == STRAFE_LEFT_STATE
			           || currentAnimationStateInt == STRAFE_RIGHT_STATE) {
				animator.speed = .8f;
				processAxisInput ();
				if (!grounded) {
					animator.SetTrigger ("airborne");
				}
			} else {  // we are in some other transition
			}

		}

		animatorSpeed = animator.speed;
	}

	void LateRunning ()
	{
        //handleFootstepsSound();

		if (jumpLateUpdate) {
			myRigidBody.AddForce (0, jumpForce, 0);
			jumpLateUpdate = false;
		} else if (jumpBoostLateUpdate) {
			myRigidBody.AddForce (0, jumpBoostForce * Time.deltaTime, 0);
			jumpBoostLateUpdate = false;
		} else if (idleToSlideBoostLateUpdate) {
			myRigidBody.AddForce (0, 0, (Mathf.Max(slideForce, currentIdleToSlideForce)) * Time.deltaTime);
			currentIdleToSlideForce *= (1 - Time.deltaTime);
		} else if (slideBoostLateUpdate) {
            float mySlideForce = slideForce;
            if (accelerationExponent == SLOPE_UPHILL_EXPONENT) {
                mySlideForce *= 3;
            }
			myRigidBody.AddForce (0, 0, mySlideForce * Time.deltaTime);
			slideBoostLateUpdate = false;
        }
	}

	private void proccessInput ()
	{

		// check if should jump or slide
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
			doJump ();
		} else if (Input.GetKeyDown (KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2)) {
			doSlide ();
            // see if we need to boost this dude because he's sliding from a standstill
            if (currentAnimationStateInt == IDLE_STATE) {
                idleToSlideBoostLateUpdate = true;
            }
			// else we are checking if we should locomote
		} else {
			processAxisInput ();
		}
	}

	public float currentRotation = 0;
	public float lateral = 0;
	public bool turnLimitReached = false;

	private void processAxisInput ()
	{

		// Horizontal first because at first I thought it was easy
		float tempLateral = Input.GetAxis ("Horizontal");
		if (Mathf.Abs(tempLateral) >= .15f) {
            tempLateral *= Mathf.Max(Mathf.Pow(1f - forwardSpeed, .8f), .15f);
        }
		currentRotation = myRigidBody.rotation.y;
		if (currentAnimationStateInt == LOCOMOTION_STATE) {
			// if we aren't pressing left/right, make him run straight ahead
			/*
			if (Mathf.Abs (tempLateral) < .15f) {
				// if we aren't pressing turn, manually turn the dude to face foward
				myRigidBody.rotation = Quaternion.Lerp (myRigidBody.rotation, Quaternion.identity, .1f * Mathf.Max (1f - forwardSpeed, .2f));
				lateral = tempLateral;
				// if he has turned too far, keep him at about 30 degrees
				*/
		 if (turnLimitReached) {
				// check to make sure we aren't already trying to turn back, ie with the direction keys
				if (
					(tempLateral > 0 && currentRotation > 0)
					||
					(tempLateral < 0 && currentRotation < 0)) {
					// change the animator param to make him run strait, but at an angle
					lateral = Mathf.Lerp (lateral, 0, .1f);
					turnLimitReached = false;
				} else {
					// we are already trying to turn him back
					lateral = tempLateral;
				}
				if (Mathf.Abs (lateral) < 0.0001) {
					// if we are back to straight, reset the flag
					turnLimitReached = false;
					lateral = 0;
				}
			} else {
				// see if we have turned too far
				// this is in radians, dammit
				if (Mathf.Abs (currentRotation) > (Mathf.PI / 12)) {
					// change the animator param to make him run strait, but at an angle
					lateral = Mathf.Lerp (lateral, 0, .1f);
					turnLimitReached = true;
				} else {
					// use the axis imput
					lateral = tempLateral;
				}
			}
		} else {
			// if not in locomotive state, we still need to get input to strafe
			lateral = tempLateral;
		}
		animator.SetFloat (LATERAL, lateral);


		// Now Vertical, ie, speed
		if (Input.GetAxis ("Vertical") > 0) {
			// Exponentially decay acceleration with respect to speed
			float speedInc = Mathf.Pow (1 - forwardSpeed, accelerationExponent);
			forwardSpeed += speedInc * Time.deltaTime;
		}
		if (Input.GetAxis ("Vertical") < 0) {
			// deceleration rate is also exponential with respect to speed
			float speedDec = 1 - forwardSpeed;
			float tempForwardSpeed = forwardSpeed - (speedDec * Time.deltaTime);
			// no walking backwards
			forwardSpeed = Mathf.Max (
				tempForwardSpeed,
				0f
			);
		}
		animator.SetFloat (FORWARD, forwardSpeed);
	}

	private bool jumpLateUpdate = false;

	private void doJump ()
	{
		setRootMotion (false);
		animator.SetTrigger ("Jump");
		jumpLateUpdate = true;
		didJump = true;
	}

	private Vector3 myLastVelocity;

	private void doSlide ()
	{
		myLastVelocity = myRigidBody.velocity;
		animator.SetTrigger ("Slide");
	}

	private void doTheLocomotion ()
	{
		// like the song.  Sadly, I didn't get to use this.
	}

	// This just for debugging
	// It's so we can check in the scrip param in the gui that the STATEs are being detected correctly
	private string getCurrentAnimationStateStringAndSetColliderHeight (int stateInt)
	{
		if (stateInt == LOCOMOTION_STATE) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "LOCOMOTION";
		} else if (stateInt == JUMP_STATE) {
			myColliderHeight = animator.GetFloat(COLLIDER_HEIGHT);
			myCapsuleCollider.center = myOriginalColliderCenter + new Vector3(0, .3f, 0);
			return	"JUMP";
		} else if (stateInt == SLIDE_START_STATE) {
			myColliderHeight = animator.GetFloat(COLLIDER_HEIGHT);
			myCapsuleCollider.center = new Vector3(0, animator.GetFloat(COLLIDER_HEIGHT), 0);
			return "SLIDE START";
		} else if (stateInt == SLIDE_MIDDLE_STATE) {
			myColliderHeight = animator.GetFloat(COLLIDER_HEIGHT);
			myCapsuleCollider.center = new Vector3(0, animator.GetFloat(COLLIDER_HEIGHT), 0);
			return "SLIDE MIDDLE";
		} else if (stateInt == SLIDE_END_STATE) {
			myColliderHeight = animator.GetFloat(COLLIDER_HEIGHT);
			myCapsuleCollider.center = new Vector3(0, animator.GetFloat(COLLIDER_HEIGHT), 0);
			return "SLIDE END";
		} else if (stateInt == IDLE_STATE) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "IDLE";
		} else if (stateInt == FALLING_STATE) {
			myColliderHeight = .8f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "FALLING";
		} else if (stateInt == STRAFE_LEFT_STATE) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "STRAFE LEFT";
		} else if (stateInt == STRAFE_RIGHT_STATE) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "STRAFE RIGHT";
		} else if (stateInt == GETTING_UP_STATE) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "GETTING UP";

		} else if (stateInt == IDLE_TO_LOCOMOTION_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "IDLE_TO_LOC";
		} else if (stateInt == IDLE_TO_JUMP_TRANS) {
			myColliderHeight = .95f;
			myCapsuleCollider.center = myCapsuleCollider.center + new Vector3(0, 1.3f * Time.deltaTime, 0);
			return "IDLE_TO_JUMP";
		} else if (stateInt == LOCOMOTION_TO_IDLE_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "LOC_TO_IDLE";
		} else if (stateInt == LOCOMOTION_TO_JUMP_TRANS) {
			myColliderHeight = .9f;
			myCapsuleCollider.center = myCapsuleCollider.center + new Vector3(0, 1f * Time.deltaTime, 0);
			return "LOC_TO_JUMP";
		} else if (stateInt == JUMP_TO_FALLING_TRANS) {
			myColliderHeight = .7f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "JUMP_TO_FALL";
		} else if (stateInt == SLIDE_TO_LOCOMOTION_TRANS) {
			myColliderHeight = .9f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "SLIDE_TO_LOC";
		} else if (stateInt == LOCOMOTION_TO_SLIDE_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "LOC_TO_SLIDE";
		} else if (stateInt == FALL_TO_LOCOMOTION_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "FALL_TO_LOC";
		} else if (stateInt == IDLE_TO_STRAFE_LEFT_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "IDLE_TO_STRAFE_LEFT";
		} else if (stateInt == IDLE_TO_STRAFE_RIGHT_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "IDLE_TO_STRAFE_RIGHT";
		} else if (stateInt == STRAFE_LEFT_TO_IDLE_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "STRAFE_LEFT_TO_IDLE";
		} else if (stateInt == STRAFE_RIGHT_TO_IDLE_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "STRAFE_RIGHT_TO_IDLE";
		} else if (stateInt == SLIDE_START_TO_MIDDLE_TRANS) {
			myColliderHeight = .3f;
			myCapsuleCollider.center = new Vector3(0,.3f, 0);
			return "slideStartToMiddle";
		} else if (stateInt == SLIDE_MIDDLE_TO_END_TRANS) {
			myColliderHeight = .3f;
			myCapsuleCollider.center = new Vector3(0,.3f, 0);
			return "SLIDE MID TO END";
		} else if (stateInt == IDLE_TO_SLIDE_TRANS) {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "IDLE_TO_SLIDE";
		} else {
			myColliderHeight = 1f;
			myCapsuleCollider.center = myOriginalColliderCenter;
			return "UNKNOWN";
		}
	}

	private void doVelocityDebugging ()
	{
		time += Time.deltaTime;
		if (time > 1f && animator.GetCurrentAnimatorStateInfo (0).IsName ("locomotion")) {
			ticks++;
			if (ticks > 10) {
				velocitySum += myRigidBody.velocity.magnitude;
				animVelocitySum += animator.velocity.magnitude;
				float avgVel = velocitySum / (ticks - 10);
				float avgAnimVel = animVelocitySum / (ticks - 10);
				Debug.Log (
					"forwardSpeed: " + forwardSpeed + " // " +
					"velocityAvg: " + avgVel + " // " +
					"animVelAvg:" + avgAnimVel
				);
			}
			time = 0f;
		}
	}

	private void enableRagdoll ()
	{
		// get the velocity of the dude
		Vector3 velocity = myRigidBody.velocity;
		bRagdoll = true;
		forwardSpeed = 0f;
		animator.enabled = false;
		animator.Rebind ();

		foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
			rb.isKinematic = false;
			rb.detectCollisions = true;
			rb.velocity = velocity;
		}

		myRigidBody.isKinematic = true;
		myRigidBody.detectCollisions = false;

	}

	// passTrue useHipPosition if not calling in start
	private void disableRagdoll (bool useHipPosition)
	{
		foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
			rb.isKinematic = true;
			rb.detectCollisions = false;
		}

		myRigidBody.isKinematic = false;
		myRigidBody.detectCollisions = true;

		animator.Rebind ();
		animator.enabled = true;

		animator.SetTrigger ("GetUp");
		bRagdoll = false;

		if (useHipPosition) {
			gameObject.transform.position = myHipsRigidBody.position;
		}
	}

	void OnCollisionEnter (Collision myCollision)
	{
		if (myCollision.collider.gameObject.layer == OBSTACLES_LAYER) {
			// calculate the total force of the impact
			float totalForce = 0f;
			Vector3 contactVeloity = myCollision.relativeVelocity;
			Debug.Log ("relativeVel  " + contactVeloity.magnitude);
			foreach (ContactPoint contact in myCollision.contacts) {
				Debug.Log("contact.normal : " + contact.normal);
				if (Vector3.SqrMagnitude(contact.normal - Vector3.up) > 0.1) {
			    	totalForce += Vector3.Dot (contact.normal, contactVeloity);
                }
			}
			totalForce *= myCollision.rigidbody.mass;
			Debug.Log ("totalForce  " + totalForce);
			if (totalForce > knockoutForce) {
                laserAudioSource.Play();
				enableRagdoll ();
			} else if (totalForce > 1f) {
				Debug.Log("reducing speed for total force : " + totalForce);
				matchAnimatorSpeedToVelocity ();
				doSlow = true;
			}
		}
		if (myCollision.collider.gameObject.layer == BUILDING_LAYER) {
			hitBuilding = true;
			enableRagdoll();
			bDead = true;
		}
	}

	public bool doSlow = false;

	void OnCollisionStay (Collision myCollision)
	{
		if (
			myCollision.collider.gameObject.layer == OBSTACLES_LAYER 
			&& doSlow
		) {
			matchAnimatorSpeedToVelocity ();
		}
	}

	void OnCollisionExit (Collision myCollision)
	{
		if (
			myCollision.collider.gameObject.layer == OBSTACLES_LAYER 
			&& doSlow
		) {
			matchAnimatorSpeedToVelocity ();
			doSlow = false;
		}
	}

	private void matchAnimatorSpeedToVelocity ()
	{
		// if you don't do the min here, you can get going really fast by sliding
		forwardSpeed = Mathf.Min (1, (myZVelocity / matchSpeedDivisor));
		if (currentAnimationStateInt == IDLE_STATE 
            && Input.GetAxis("Vertical") > .1f
        ) {
			forwardSpeed = .11f;
		}
		animator.SetFloat (FORWARD, forwardSpeed);
	}


	// sees if the player is grounded, if so sets his acceleration potential according to the surface
	private bool getGroundedAndSetSurface ()
	{
		return detectGroundAndSetSurface (0f, true);
	}

	// like above, but detects more distant ground
	private bool getAlmostGrounded ()
	{
		return detectGroundAndSetSurface (1f, false);
	}

	// returns true if there is ground within 0.3 + extraCastDistance of the player's feet
	// sets the player's acceleration potential according to the surface slope tag if setSurface is true
	private bool detectGroundAndSetSurface (float extraCastDistance, bool doSetSurface)
	{
		// Define a Ray from the center of the capsule collider straight down.
		Vector3 origin = myCapsuleCollider.transform.position
		                 + new Vector3 (0, myCapsuleCollider.height, 0);
		Vector3 direction = Vector3.down;
		Ray ray = new Ray (origin, direction);


		float sphereRadius = myCapsuleCollider.radius;
		float castDistance = myCapsuleCollider.height + extraCastDistance;

		// Cast a sphere the radius of the capsule collider
		// from the center of the capusule collider straight down,
		// to the point at which the sphere's center is at the player's feet
		// and detect if it hits something not on the PLAYER_LAYER 
		RaycastHit hit;
		if (Physics.SphereCast (ray, sphereRadius, out hit, castDistance, sphereColliderLayerMask)) {
			if (doSetSurface) {
				// setting the surface controlls the player's accelleration potential
				setSurface (hit.collider.gameObject.tag);
			}
			// only consider the ground layer and obstacle layer
			return (
				hit.collider.gameObject.layer == GROUND_LAYER
				|| hit.collider.gameObject.layer == BUILDING_LAYER
				|| hit.collider.gameObject.layer == OBSTACLES_LAYER
			);
		} else {
			return false;
		}
	}

	// Sets the surface slope acceleration exponent depending according to the surfaceTag param
	private void setSurface (string surfaceTag)
	{
		if (surfaceTag.Equals (UPHILL_TAG)) {
			accelerationExponent = SLOPE_UPHILL_EXPONENT;
		} else if (surfaceTag.Equals (DOWNHILL_TAG)) {
			accelerationExponent = SLOPE_DOWNHILL_EXPONENT;
		} else { // it's tagged flat or not at all
			accelerationExponent = SLOPE_FLAT_EXPONENT;
		}
	}

	// Wrapper method for setting root motion
	private void setRootMotion (bool b)
	{
		animator.applyRootMotion = b;
	}

	// Returns true if the ragdoll has stopped moving.
	private bool ragdollStoppedMoving ()
	{
		bool stillMoving = false;
		foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
			if (!rb.IsSleeping ()) {
				stillMoving = true;
			}
		}
		return !stillMoving;
	}

	private bool ragdollDurationElapsed ()
	{
		ragdollDuration += Time.deltaTime;
		if (ragdollDuration > 3f) {
			ragdollDuration = 0f;
			return true;
		}
		return false;
	}

    private float ragdollLongDuration = 0f;
	private bool ragdollLongDurationElapsed ()
	{
		ragdollLongDuration += Time.deltaTime;
		if (ragdollLongDuration > 5f) {
			ragdollLongDuration = 0f;
			return true;
		}
		return false;
	}

    /*private void handleFootstepsSound()
    {
        if (forwardSpeed > 0 && currentAnimationStateInt == LOCOMOTION_STATE)
        {
            if (!footstepAudioPlaying)
            {
                footstepsAudioSource.Play();
                footstepAudioPlaying = true;
            }
            footstepsAudioSource.pitch = 1 + forwardSpeed;
        }
        else
        {
            footstepsAudioSource.Stop();
            footstepAudioPlaying = false;
        }
    }*/

    public void PlayFootstepSound()
    {
        if (grounded)
        {
            footstepsAudioSource.Play();
        }
    }
}
