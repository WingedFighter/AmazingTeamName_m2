using UnityEngine;
using System.Collections;

public class DropShipController : MonoBehaviour {
    // External refs
    public PlayerController playerController;

    // Modifiable AI Parameters
    public float LeadTime = 2f;
    public float minLeadDistance = 5f;
    public float MaxObjectSize = 10f;
    public float MinObjectSize = 10f;
    public float LateralSpeed = 2f;
    public float DropFrequency = 1f;
	public float flybySpeed = 2f;

	// Flyby Waypoints
	private GameObject[] waypoints;
	private int currentWaypoint = 0;

    // List of droppable objects
    public GameObject[] DropItems;

    public enum MovementAxis
    {
        x,
        y,
        z
    }

    public MovementAxis Axis = MovementAxis.z;

    // State Machine States
    public enum state {
        FLYBY,
        MOVE,
        DROP
    }

    // State Machine Controls
    public state CurrentState = state.FLYBY;

    // Private Members
    private float dropTimer = 0f;

	// Use this for initialization
	void Start () {
		waypoints = GameObject.FindGameObjectsWithTag ("Waypoint");
	}
	
	// Update is called once per frame
	void Update () {
	    switch (CurrentState)
        {
            case state.FLYBY:
                FlyBy();
                break;
            case state.MOVE:
                Move();
                break;
            case state.DROP:
                Drop();
                break;
        }
	}
    
    private void FlyBy()
    {
		if (currentWaypoint <= waypoints.Length - 1) {
			//print (currentWaypoint + ", " + transform.position);
			Vector3 target = waypoints [currentWaypoint].transform.position;
			transform.position = Vector3.Lerp (transform.position, target, flybySpeed * Time.deltaTime);
			if (Vector3.Distance (transform.position, target) < 1.5f) {
				currentWaypoint++;
			}
		} else {
			CurrentState = state.MOVE;
		}
    }

    private void Move()
    {
        CharacterController cc = playerController.GetComponent<CharacterController>();

        Vector3 deltaPosition = cc.velocity * LeadTime;
        Vector3 targetPosition = playerController.transform.position + deltaPosition;
        // We want the target height to remain the same as our current height
        targetPosition.y = transform.position.y;

        // Ensure that the DropShip stays a minimum distance in front of the player
        switch(Axis)
        {
            case MovementAxis.x:
                if (targetPosition.x - playerController.transform.position.x < minLeadDistance)
                {
                    targetPosition.x = transform.position.x;
                }
                break;
            case MovementAxis.y:
                if (targetPosition.y - playerController.transform.position.y < minLeadDistance)
                {
                    targetPosition.y = transform.position.y;
                }
                break;
            case MovementAxis.z:
                if (targetPosition.z - playerController.transform.position.z < minLeadDistance)
                {
                    targetPosition.z = transform.position.z;
                }
                break;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, LateralSpeed * Time.deltaTime);

        // Update the state transition parameters
        dropTimer += Time.deltaTime;
        if (dropTimer > DropFrequency)
        {
            CurrentState = state.DROP;
            dropTimer = 0f;
        }
    }

    private void Drop()
    {
        int index = Random.Range(0, DropItems.Length);
        Vector3 dropPosition = transform.position;
        dropPosition.y -= 0.75f;
        Instantiate(DropItems[index], dropPosition, transform.rotation);

        // Transition back to move
        CurrentState = state.MOVE;
    }
}
