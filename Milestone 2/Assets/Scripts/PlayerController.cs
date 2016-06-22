using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float SpeedIncrement = 1f;
    public float HorizontalSpeed = 1f;

    private float forwardSpeed = 0f;
    private Vector3 speed;

    private Animator animator;
    private CharacterController ccontroller;
    private bool bRagdoll = false;

	// Use this for initialization
	void Start () {
        //animator = GetComponentInChildren<Animator>();
        animator = GetComponent<Animator>();
        ccontroller = GetComponent<CharacterController>();
        disableRagdoll();
       
        speed = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Vertical") > 0)
        {
            forwardSpeed += SpeedIncrement * Time.deltaTime;
        }
       
        

        // Update Animation
        animator.SetFloat("WalkRun", forwardSpeed/5f);

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump"); ;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("Vault"); ;
        }
    }

    void FixedUpdate()
    {
        print("Speedx: " + speed.x);
        if(ccontroller.isGrounded)
        {
            print("Grounded");
            animator.SetFloat("Strafe", Input.GetAxis("Horizontal") * HorizontalSpeed);

        } else
        {
            // Update Location
            speed.x = Input.GetAxis("Horizontal") * HorizontalSpeed;
            ccontroller.SimpleMove(speed);
        }


    }

    void OnCollisionEnter (Collision collision)
    {
        print("Collided");
        if (collision.gameObject.tag == "ObstacleMovable")
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
            print("Collided with speed: " + forwardSpeed);
            if (forwardSpeed > 1)
            {
                enableRagdoll();
            }
            forwardSpeed = 0;
        }
    }

    private void enableRagdoll()
    {
        animator.enabled = false;
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
        animator.SetTrigger("GetUp");
    }

}
