using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
    public float ComponentPositionThreshold = 5f;
    public float PercentDestroyedThreshold = 0.8f;
    public bool bPlayerInZone = false;
    public bool isKinematic = true;

	// Use this for initialization
	void Start () {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            // Add a building component
            rb.gameObject.AddComponent<BuildingComponent>();

            // Set all components to be kinematic
            rb.isKinematic = isKinematic;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //print("Is Stable: " + IsStable());
	}

    void FixedUpdate()
    {
        if (!bPlayerInZone && !isKinematic && IsStable())
        {
            SetKinematic(true);
        }
    }

    public bool IsDestroyed()
    {
        float numberDestroyed = 0f;
        BuildingComponent[] components = GetComponentsInChildren<BuildingComponent>();
		/* use this "if" if you're destroying buildings as they fall

		if (components.Length == 0) {
			return true;
		}
		*/
        foreach (BuildingComponent bc in components)
        {
            if ((bc.transform.position - bc.OriginalLocation).magnitude > ComponentPositionThreshold)
            {
                numberDestroyed++;
            }
			/* Use this instead of above is you're destroying buildings when they hit the ground
			if (bc.isActiveAndEnabled) {
				return false;
			}
			*/
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

    void OnTriggerEnter(Collider other)
    {
        print("Tag: " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            SetKinematic(false);
            bPlayerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bPlayerInZone = false;
        }
    }

    void SetKinematic(bool bOnOff)
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        if (bOnOff)
        {
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = false;
            }
        }

        isKinematic = bOnOff;
    }
}