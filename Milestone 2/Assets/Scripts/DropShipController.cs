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

    // Flyby Waypoints
    public Transform[] Waypoints;

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

    }

    private void Move()
    {
        CharacterController cc = playerController.GetComponent<CharacterController>();
        print(cc.velocity);

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

        print(targetPosition);
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
        int index = Random.Range(0, 1);
        Instantiate(DropItems[index], transform.position, transform.rotation);

        // Transition back to move
        CurrentState = state.MOVE;
    }
}
