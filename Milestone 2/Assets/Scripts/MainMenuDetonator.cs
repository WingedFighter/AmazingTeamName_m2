using UnityEngine;
using System.Collections;

public class MainMenuDetonator : MonoBehaviour {
    public MainMenuBuilding[] Buildings;
    public int BuildingIndex = 0;
    public bool bDetonate = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (bDetonate)
        {
            Detonate();
        }
	}

    public void Detonate()
    {
        if (!Buildings[BuildingIndex].bCollapsing)
        {
            Buildings[BuildingIndex].bCollapsing = true;
        }
        if (Buildings[BuildingIndex].bDoneCollapsing)
        {
            BuildingIndex++;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        print("Collided");
        bDetonate = true;
    }
}
