using UnityEngine;
using System.Collections;

public class MS3GameController : MonoBehaviour {
    public GameObject DropShip;
    public PlayerController pc;

    public GameObject[] DropShipTypes;

    public Vector3 DropShipSpawnPosition;
    public Quaternion DropShipSpawnRotation;

	// Use this for initialization
	void Start () {
        DropShipSpawnPosition = DropShip.transform.position;
        DropShipSpawnRotation = DropShip.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DropShip.gameObject.SetActive(false);
            DropShip = (GameObject)Instantiate(DropShipTypes[0], DropShipSpawnPosition, DropShipSpawnRotation);
            DropShip.GetComponent<DropShipController>().playerController = pc;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DropShip.gameObject.SetActive(false);
            DropShip = (GameObject)Instantiate(DropShipTypes[1], DropShipSpawnPosition, DropShipSpawnRotation);
            DropShip.GetComponent<DropShipController>().playerController = pc;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DropShip.gameObject.SetActive(false);
            DropShip = (GameObject)Instantiate(DropShipTypes[2], DropShipSpawnPosition, DropShipSpawnRotation);
            DropShip.GetComponent<DropShipController>().playerController = pc;
        }
    }
}
