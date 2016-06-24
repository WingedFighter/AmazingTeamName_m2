using UnityEngine;
using System.Collections;
/* Amazing Team Name
 * Kevin Curtin
 * Idan Mintz
 * Jackson Millsaps
 * Jessica Chin
 * Matthew Johnston
 */

public class KevinSoundController : MonoBehaviour {
    public AudioSource dashSound;
    public AudioSource jumpSound;
    public AudioSource runSound;
    private CharacterController ccontroller;

    // Use this for initialization
    void Start () {
        ccontroller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashSound.Play();
        }
        if (Input.GetKeyDown(KeyCode.Space) && ccontroller.isGrounded)
        {
            jumpSound.Play();
        }
        if (Input.GetKeyDown(KeyCode.W) && !runSound.isPlaying)
        {
            runSound.Play();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            runSound.Stop();
        }
    }
}
