using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float SpeedIncrement = 1f;
    public float HorizontalSpeed = 1f;
    public float KnockoutCollisionMagnitude = 1f;

    private float forwardSpeed = 0f;
    private Vector3 speed;

    private Animator animator;
    private CharacterController ccontroller;
    private float ccBaseHeight;
    private bool bRagdoll = false;

	// Use this for initialization
	void Start () {
        //animator = GetComponentInChildren<Animator>();
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
        if (Input.GetAxis("Vertical") > 0)
        {
            forwardSpeed += SpeedIncrement * Time.deltaTime;
        }
       
        // Update Animation
        animator.SetFloat("WalkRun", forwardSpeed/3f);

        //Ragdoll toggle
        if (Input.GetKeyDown(KeyCode.Tab))
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
        if (Input.GetKeyDown(KeyCode.Space) && ccontroller.isGrounded)
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
        if(ccontroller.isGrounded)
        {
            animator.SetFloat("Strafe", Input.GetAxis("Horizontal") * HorizontalSpeed);

        } else
        {
            // Update Location
            speed.x = Input.GetAxis("Horizontal") * HorizontalSpeed;
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

}
