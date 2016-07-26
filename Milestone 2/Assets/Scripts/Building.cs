using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
    public float ComponentPositionThreshold = 5f;
    public float PercentDestroyedThreshold = 0.8f;

	// Use this for initialization
	void Start () {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.gameObject.AddComponent<BuildingComponent>();
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    public bool IsDestroyed()
    {
        float numberDestroyed = 0f;
        BuildingComponent[] components = GetComponentsInChildren<BuildingComponent>();
        foreach (BuildingComponent bc in components)
        {
            if ((bc.transform.position - bc.OriginalLocation).magnitude > ComponentPositionThreshold)
            {
                numberDestroyed++;
            }
        }
        
        if (numberDestroyed/components.Length >= PercentDestroyedThreshold)
        {
            return true;
        }
        return false;
    }

    public bool IsStable()
    {
        BuildingComponent[] components = GetComponentsInChildren<BuildingComponent>();
        foreach (BuildingComponent bc in components)
        {
            if (bc.IsMoving())
            {
                return false;
            }
        }

        return true;
    }
}
