using UnityEngine;
using System.Collections;

public class MainMenuBuilding : MonoBehaviour {
    public bool bCollapsing = false;
    public bool bDoneCollapsing = false;

    private float baseX;
    private float baseZ;

	// Use this for initialization
	void Start () {
        baseX = transform.position.x;
        baseZ = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
	    if (bCollapsing &! bDoneCollapsing)
        {
            Collapse();
        }
	}

    public void Collapse()
    {
        gameObject.transform.position = new Vector3(baseX + Random.Range(-0.25f, 0.25f), transform.position.y - 4 * Time.deltaTime, baseZ + Random.Range(-0.25f, 0.25f));
        if(transform.position.y + transform.localScale.y/2f < 0f)
        {
            bDoneCollapsing = true;
        }
    }
}
