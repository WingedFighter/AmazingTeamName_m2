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
        if (pc.bRagdoll) {
			transform.position = Vector3.Lerp(transform.position, deadTarget.position, positionSmoothing * Time.deltaTime);
			transform.forward = Vector3.Lerp(transform.forward, deadTarget.forward, directionSmoothing * Time.deltaTime);
        } else {
			liveTarget = GameObject.Find("playerCameraTarget").transform;
			spot.x = liveTargetX;
			spot.y = liveTarget.position.y;
			spot.z = liveTarget.position.z;
			transform.position = Vector3.Lerp(transform.position, spot + offset, positionSmoothing * Time.deltaTime);
			transform.forward = Vector3.Lerp(transform.forward, Vector3.forward, directionSmoothing * Time.deltaTime);
		}
	}
}
