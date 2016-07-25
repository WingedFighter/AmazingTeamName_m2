using UnityEngine;
using System.Collections;

public class CreditsCube : MonoBehaviour {

    public bool bLanded = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        // Delete the object when hit from the top with another credits cube
        if ( collision.gameObject.GetComponent<CreditsCube>() != null && collision.gameObject.transform.position.y > transform.position.y)
        {
            Destroy(gameObject, 3f);
        } else
        {
            bLanded = true;
        }
    }
}
