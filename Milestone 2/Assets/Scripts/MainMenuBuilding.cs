using UnityEngine;
using System.Collections;

public class MainMenuBuilding : MonoBehaviour {
    public bool bCollapsing = false;
    public bool bDoneCollapsing = false;

    private bool bAudioPlaying = false;

    private float baseX;
    private float baseZ;

    private AudioSource crumbleAudio;
    private AudioSource explosionAudio;

    private ParticleSystem[] particleSystems;

	// Use this for initialization
	void Start () {
        baseX = transform.position.x;
        baseZ = transform.position.z;

        crumbleAudio = GetComponents<AudioSource>()[0];
        explosionAudio = GetComponents<AudioSource>()[1];

        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (bCollapsing &! bDoneCollapsing)
        {
            if (!bAudioPlaying)
            {
                crumbleAudio.Play();
                explosionAudio.Play();
                bAudioPlaying = true;
                EnableSmokeAndFire();
            }
            Collapse();
        }
	}

    public void Collapse()
    {
        gameObject.transform.position = new Vector3(baseX + Random.Range(-0.25f, 0.25f), transform.position.y - 4 * Time.deltaTime, baseZ + Random.Range(-0.25f, 0.25f));
        if(transform.position.y + transform.localScale.y/2f < 0f)
        {
            bDoneCollapsing = true;
            crumbleAudio.loop = false;
            explosionAudio.loop = false;
            DisableSmokeAndFire();
        }
    }

    private void EnableSmokeAndFire()
    {
        print("Enabling Smoke and Fire: " + particleSystems.Length);
        foreach (ParticleSystem pe in particleSystems)
        {
            pe.Play();
        }
    }

    private void DisableSmokeAndFire()
    {
        foreach (ParticleSystem pe in particleSystems)
        {
            pe.Stop();
        }
    }
}
