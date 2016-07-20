using UnityEngine;
using System.Collections;
/* Amazing Team Name
 * Kevin Curtin
 * Idan Mintz
 * Jackson Millsaps
 * Jessica Chin
 * Matthew Johnston
 */
public class FollowCameraController : MonoBehaviour {
	// where to point the camera when player is live
	Transform liveTarget;
	// where to point the camera when player is dead
    Transform deadTarget;
    public float positionSmoothing = 10f;
    public float directionSmoothing = 5f;
    public PlayerControllerAlpha pc;

    private Vector3 offset;

	// Use this for initialization
	void Start () {
		liveTarget = GameObject.Find("playerCameraTarget").transform;
//		deadTarget = GameObject.Find("headCameraTarget").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//        if (pc.bRagdoll) {
//			transform.position = Vector3.Slerp(transform.position, deadTarget.position, positionSmoothing * Time.deltaTime);
//			transform.forward = Vector3.Slerp(transform.forward, deadTarget.forward, directionSmoothing * Time.deltaTime);
//        } else {
			transform.position = Vector3.Slerp(transform.position, liveTarget.position, positionSmoothing * Time.deltaTime);
			transform.forward = Vector3.Slerp(transform.forward, liveTarget.forward, directionSmoothing * Time.deltaTime);
//		}
	}
}
