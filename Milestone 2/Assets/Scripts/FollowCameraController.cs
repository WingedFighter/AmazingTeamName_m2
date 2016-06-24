using UnityEngine;
using System.Collections;

public class FollowCameraController : MonoBehaviour {
    public Transform target;
    public float smoothing = 5f;
    public PlayerController pc;

    private Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = transform.position - target.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        if (pc.bDead)
        {
            transform.LookAt(target);
        }
	}
}
