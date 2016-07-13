using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Animations;
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
    public bool bRagdoll = false;

    private float currentTimeNotGrounded = 0f;
    private bool bPastGroundedThreshold = false;


	// referencing animator params
	static string FORWARD = "forward";
	static string LATERAL = "lateral";

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        ccontroller = GetComponent<CharacterController>();
        ccBaseHeight = ccontroller.center.y;
        ccontroller.detectCollisions = false;
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
        if (!bRagdoll && !bPastGroundedThreshold) { 
	        if (Input.GetAxis("Vertical") > 0)
	        {
				// Exponentially decay acceleration with respect to speed
				float speedInc = Mathf.Pow(1-forwardSpeed, 3f);
				forwardSpeed += speedInc * Time.deltaTime;
	        }
			if (Input.GetAxis("Vertical") < 0) {
				// deceleration rate is also exponential with respect to speed
				float speedDec = 1-forwardSpeed;
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
            } else
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
            animator.SetTrigger("Jump");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("Vault"); ;
        }
    }

    void FixedUpdate()
    {
        if(!bPastGroundedThreshold)
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
            ccontroller.Move(new Vector3(ccontroller.center.x, ccBaseHeight + jumpHeight/2f, ccontroller.center.z));
        }
    }

    void OnCollisionEnter (Collision collision)
    {
        print("Collided");
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
            print("Immvable: " + collision.relativeVelocity.magnitude);
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

}
