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
	public float ragdollPositionSmoothing = 3f;
    public float positionSmoothingX = 5f;
    public float positionSmoothingY = 5f;
    public float positionSmoothingZ = 5f;
    public float directionSmoothingFast = 5f;
    public float directionSmoothingSlow = 1f;
    public float directionSmoothingMedium = 3f;
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
			transform.position = Vector3.Lerp(transform.position, deadTarget.position, ragdollPositionSmoothing * Time.deltaTime);
			transform.forward = Vector3.Lerp(transform.forward, deadTarget.forward, directionSmoothingMedium * Time.deltaTime);
        } else {
			liveTarget = GameObject.Find("playerCameraTarget").transform;
			spot = liveTarget.position + offset;

			// you have to do it this way because althought you can set a transform.position, you cannot set it's individual components
			float resultX = Mathf.Lerp(transform.position.x, spot.x, positionSmoothingX * Time.deltaTime);
			float resultY = Mathf.Lerp(transform.position.y, spot.y, positionSmoothingY * Time.deltaTime);
			float resultZ = Mathf.Lerp(transform.position.z, spot.z, positionSmoothingZ * Time.deltaTime);
			transform.position = new Vector3(resultX, resultY, resultZ);
			if (pc.bGettinUp) {
				// rotate fast
				transform.forward = Vector3.Lerp(transform.forward, liveTarget.forward, directionSmoothingFast * Time.deltaTime);
			} else {
				// rotate slow
				transform.forward = Vector3.Lerp(transform.forward, liveTarget.forward, directionSmoothingSlow * Time.deltaTime);
			}
		}
	}
}
