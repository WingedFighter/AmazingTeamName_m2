using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
    public Building TargetBuilding;
    public DeathPlane dPlane;
    public GameObject Player;
    public GameObject SuccessText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // Look at buliding as its collapsing
        if (!TargetBuilding.IsStable())
        {

        }

        // Reset the player if the death plane is triggered
        if (dPlane.Triggered)
        {
            if (TargetBuilding.IsStable())
            {
                Player.GetComponent<PlayerControllerAlpha>().Reset();
                dPlane.Triggered = false;
            }

        }

        if (TargetBuilding.IsDestroyed())
        {
            Invoke("LevelComplete", 4);
        }
	}

    void LevelComplete()
    {
        SuccessText.SetActive(true);
    }
}
