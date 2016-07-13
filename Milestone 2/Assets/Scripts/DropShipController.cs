using UnityEngine;
using System.Collections;

public class DropShipController : MonoBehaviour {
    // External refs
    public PlayerController playerController;

    // Modifiable AI Parameters
    public float LeadDistance = 20f;
    public float MaxObjectSize = 10f;
    public float MinObjectSize = 10f;
    public float LateralSpeed = 5f;
    public float DropFrequency = 1f;

    // Flyby Waypoints
    public Transform[] Waypoints;

    // State Machine States
    public enum state {
        FLYBY,
        MOVE,
        DROP
    }

    // State Machine Controls
    public state CurrentState = state.FLYBY;

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

    }

    private void Drop()
    {

    }
}
