using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
/* Amazing Team Name
 * Kevin Curtin
 * Idan Mintz
 * Jackson Millsaps
 * Jessica Chin
 * Matthew Johnston
 */

public class PlayerController : MonoBehaviour {
    public float HorizontalSpeed = 2f;
    public float KnockoutCollisionMagnitude = 1f;
    public float GroundedThreshold = 0.25f;

	public float decelerationRate = 5f;

    public float forwardSpeed = 0f;
    public bool bDead = false;

    private Vector3 speed;

    private Animator animator;
    private CharacterController ccontroller;
    private float ccBaseHeight;
    private float ccBaseY;
    public bool bRagdoll = false;

    private float currentTimeNotGrounded = 0f;
    private bool bPastGroundedThreshold = false;


	// referencing animator params
	static string FORWARD = "forward";
	static string LATERAL = "lateral";

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
	void Start () {
        animator = GetComponent<Animator>();
        ccontroller = GetComponent<CharacterController>();
        //ccBaseHeight = ccontroller.center.y;
        //ccontroller.detectCollisions = false;
        ccBaseHeight = ccontroller.height;
        ccBaseY = ccontroller.center.y;
        disableRagdoll();


        // Add Ragdoll part script to all subcomponents
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            RagdollPart rps = rb.gameObject.AddComponent<RagdollPart>();

            //Set the scripts mainScript reference so that it can access
            //the score and scoreTextTemplate member variables above
            rps.playerController = this;
        }

        speed = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {
        switch(State)
        {
            case PlayerState.disabled:
                break;
            case PlayerState.running:
                Running();
                break;
            case PlayerState.projectile:
                Projectile();
                break;
            case PlayerState.dead:
                break;
        }
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
                FixedProjectile();
                break;
            case PlayerState.dead:
                break;
        }
    }

    void Running ()
    {
        if (bDead)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene("IdanScene");
                // gameover
            }
            return;
        }

        if (ccontroller.isGrounded)
        {
            bPastGroundedThreshold = false;
            currentTimeNotGrounded = 0f;
        }
        else
        {
            currentTimeNotGrounded += Time.deltaTime;
            if (currentTimeNotGrounded > GroundedThreshold)
            {
                bPastGroundedThreshold = true;
            }
        }

        // only change run speed if in running-ish state -- ie, not getting up, jumping, etc
        //		if (animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion")) {
        if (!bRagdoll && !bPastGroundedThreshold)
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
                forwardSpeed = Mathf.Max(
                    forwardSpeed - (speedDec * Time.deltaTime),
                    0
                );
            }
        }


        // Update Animation
        animator.SetFloat(FORWARD, forwardSpeed);

        //Ragdoll toggle
        if (Input.GetKeyDown(KeyCode.Tab) && !bDead)
        {
            if (bRagdoll)
            {
                disableRagdoll();
                forwardSpeed = 0;
                bRagdoll = false;
            }
            else
            {
                enableRagdoll();
                bRagdoll = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetTrigger("Slide"); ;
        }
        if (Input.GetKeyDown(KeyCode.Space) && animator.GetFloat("JumpHeight") == 0f)
        {
            // No more Jumping
            //animator.SetTrigger("Jump");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("Vault"); ;
        }
    }

    void FixedRunning()
    {
        speed.x = Input.GetAxis("Horizontal") * HorizontalSpeed;//* forwardSpeed;
        speed.z = forwardSpeed*10f;
        ccontroller.SimpleMove(speed);

        // Adjust collider with slide height
        ccontroller.center = new Vector3(ccontroller.center.x, ccBaseY * animator.GetFloat("SlideHeight"), ccontroller.center.z);
        ccontroller.height = ccBaseHeight * animator.GetFloat("SlideHeight");

        if (!bPastGroundedThreshold)
        {
            //            animator.SetFloat(LATERAL, Input.GetAxis("Horizontal") * HorizontalSpeed);
            animator.SetFloat(LATERAL, Input.GetAxis("Horizontal"));

        }
        else
        {
            // Steer in the air with no turning
            animator.SetFloat(LATERAL, 0);
            speed.x = Input.GetAxis("Horizontal") * HorizontalSpeed;//* forwardSpeed;
            ccontroller.SimpleMove(speed);
        }



        // When Jumping move the ccontroller to stay at player's feet
        float jumpHeight = animator.GetFloat("JumpHeight");
        if (jumpHeight > 0)
        {
            print("JumpHeight = " + ccontroller.center.y);
            ccontroller.center = new Vector3(ccontroller.center.x, ccBaseHeight + jumpHeight, ccontroller.center.z);
            ccontroller.Move(new Vector3(ccontroller.center.x, ccBaseHeight + jumpHeight / 2f, ccontroller.center.z));
        }
    }

    void Projectile()
    {
        // On Hitting ground, transition back to running
        if (ccontroller.isGrounded)
        {
            State = PlayerState.running;
        }
    }

    void FixedProjectile()
    {

    }

    void OnCollisionEnter (Collision collision)
    {
        /*if (collision.gameObject.tag == "ObstacleMovable")
        {
            print("Hit Movable");
            // Reduce Speed based on the type of object
            forwardSpeed -= collision.gameObject.GetComponent<Rigidbody>().mass;
            
            // If speed becomes lower than 0, set to 0;
            if (forwardSpeed < 0)
            {
                forwardSpeed = 0;
            }
        }
        if (collision.gameObject.tag == "ObstacleImmovable")
        {
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            print("Immvable: " + collision.relativeVelocity.magnitude);
            //Check that we are colliding with sufficient velocity
            if (collision.relativeVelocity.magnitude > KnockoutCollisionMagnitude)
            {
                enableRagdoll();
                forwardSpeed = 0;
                Invoke("disableRagdoll", 2f);
            }
        }
        */
    }

    private void enableRagdoll()
    {
        animator.enabled = false;
        animator.Rebind();

        // disable the character controller and reset it to its original position
        //ccontroller.Move(new Vector3(0, ccBaseHeight, 0));
        ccontroller.center = new Vector3(0, ccBaseHeight, 0);


        //ccontroller.enabled = false;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
        }
		bRagdoll = true;
    }

    private void disableRagdoll()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
        animator.Rebind();
        animator.enabled = true;
        ccontroller.enabled = true;

        animator.SetTrigger("GetUp");
		bRagdoll = false;
    }

    public void RagdollCollisionHandler(Collision collision)
    {
        if (collision.gameObject.tag == "ObstacleImmovable")
        {
            //Check that we are colliding with sufficient velocity
            if (collision.relativeVelocity.magnitude > KnockoutCollisionMagnitude)
            {
                enableRagdoll();
                forwardSpeed = 0;
                Invoke("disableRagdoll", 2f);
            }
        }
        if (collision.gameObject.tag == "ObstacleMovable")
        {
            forwardSpeed -= collision.gameObject.GetComponent<Rigidbody>().mass;
            if (forwardSpeed < 0)
            {
                forwardSpeed = 0;
            }
        }
    }

    public void RagdollTriggerHandler(Collider other)
    {
        //if (other.gameObject.CompareTag("DeathPlane"))
        //{
            enableRagdoll();
        bDead = true;
       // }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        LaunchRamp ramp = hit.gameObject.GetComponent<LaunchRamp>();
        if (ramp != null && ramp.Active)
        {
            forwardSpeed += ramp.SpeedBoostMagnitude;
            ramp.Active = false;
            State = PlayerState.projectile;
        }
    }
}
