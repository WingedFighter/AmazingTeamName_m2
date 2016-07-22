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

	public Vector3 spot;
    public float positionSmoothing = 10f;
    public float directionSmoothing = 5f;
    public PlayerControllerAlpha pc;

	 public Vector3 offset;
	public float liveTargetX;

	// Use this for initialization
	void Start () {
		liveTarget = GameObject.Find("playerCameraTarget").transform;
		liveTargetX = liveTarget.position.x;
		deadTarget = GameObject.Find("headCameraTarget").transform;
		offset = new Vector3(0,0,-2);
		spot = new Vector3();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		offset = liveTarget.position - gameObject.transform.position;
		liveTarget = GameObject.Find("playerCameraTarget").transform;
		spot.x = liveTargetX;
		spot.y = liveTarget.position.y;
		spot.z = liveTarget.position.z;
//        if (pc.bRagdoll) {
//			transform.position = Vector3.Slerp(transform.position, deadTarget.position, positionSmoothing * Time.deltaTime);
//			transform.forward = Vector3.Slerp(transform.forward, deadTarget.forward, directionSmoothing * Time.deltaTime);
//        } else {
		transform.position = Vector3.Lerp(transform.position, spot + offset, positionSmoothing * Time.deltaTime);
//		transform.position = 
//			transform.forward = Vector3.Lerp(transform.forward, liveTarget.forward, directionSmoothing * Time.deltaTime);
//			transform.position = liveTarget.position - new Vector3(0, offset.y, offset.z);
//			transform.forward = Vector3.zero;
//		}
	}
}
