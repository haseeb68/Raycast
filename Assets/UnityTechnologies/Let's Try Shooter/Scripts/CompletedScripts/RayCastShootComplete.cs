using UnityEngine;
using System.Collections;

public class RayCastShootComplete : MonoBehaviour {

	public int gunDamage = 1;											// Set the number of hitpoints that this gun will take away from shot objects with a health script
	public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    public float DetectSoundRate = 0.5f;                                // (Xain) Number in seconds which controls Detection Sound to play again.
    public float weaponRange = 50f;										// Distance in Unity units over which the player can fire
	public float hitForce = 100f;										// Amount of force which will be added to objects with a rigidbody shot by the player
	public Transform gunEnd;											// Holds a reference to the gun end object, marking the muzzle location of the gun

	private Camera fpsCam;												// Holds a reference to the first person camera
	private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);	// WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
	private AudioSource gunAudio;										// Reference to the audio source which will play our shooting sound effect
	private LineRenderer laserLine;										// Reference to the LineRenderer component which will display our laserline
	private float nextFire;                                             // Float to store the time the player will be allowed to fire again, after firing
	public AudioClip DetectionSound;                                    // (Xain) Audio Clip used for Enemy Or Shooting Item Detection.
    private float nextSoundtime;                                        // (Xain) Delay for Detection Sound to play again.
    void Start () 
	{
		// Get and store a reference to our LineRenderer component
		laserLine = GetComponent<LineRenderer>();

		// Get and store a reference to our AudioSource component
		gunAudio = GetComponent<AudioSource>();

		// Get and store a reference to our Camera by searching this GameObject and its parents
		fpsCam = GetComponentInParent<Camera>();
	}
	

	void Update () 
	{
        RaycastHit hit;
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) 
		{
			// Update the time when our player can fire next
			nextFire = Time.time + fireRate;

			// Start our ShotEffect coroutine to turn our laser line on and off
            StartCoroutine (ShotEffect());

            // Create a vector at the center of our camera's viewport
            

            // Declare a raycast hit to store information about what our raycast has hit
            

			// Set the start position for our visual effect for our laser to the position of gunEnd
			laserLine.SetPosition (0, gunEnd.position);

			// Check if our raycast has hit anything
			if (Physics.Raycast (rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
			{
				// Set the end position for our laser line 
				laserLine.SetPosition (1, hit.point);

				// Get a reference to a health script attached to the collider we hit
				ShootableBox health = hit.collider.GetComponent<ShootableBox>();

				// If there was a health script attached
				if (health != null)
				{
					// Call the damage function of that script, passing in our gunDamage variable
					health.Damage (gunDamage);
				}

				// Check if the object we hit has a rigidbody attached
				if (hit.rigidbody != null)
				{
					// Add force to the rigidbody we hit, in the direction from which it was hit
					hit.rigidbody.AddForce (-hit.normal * hitForce);
				}
			}
			else
			{
				// If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                laserLine.SetPosition (1, rayOrigin + (fpsCam.transform.forward * weaponRange));
			}
		}


		// (Xain) Check if the RayCast is aiming at a Shootable Object e.g Shootable Box.
		else
		{
            // (Xain) Raycasting from same origin as for shooting
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                // (Xain) Checking Ray is aiming for box and calculating time for sound to play.
                if (hit.collider.GetComponent<ShootableBox>() && Time.time > nextSoundtime && !gunEnd.gameObject.GetComponent<AudioSource>().isPlaying)
				{
                    // (Xain) adding next sound time to play.
                    nextSoundtime = Time.time + DetectSoundRate;
                    // (Xain) Added an audiosource for detection sound at GunEnd GameObject.
                    gunEnd.gameObject.GetComponent<AudioSource>().PlayOneShot(DetectionSound);
				}
                
            }
        }

    }


	private IEnumerator ShotEffect()
	{
		// Play the shooting sound effect
		gunAudio.Play ();

		// Turn on our line renderer
		laserLine.enabled = true;

		//Wait for .07 seconds
		yield return shotDuration;

		// Deactivate our line renderer after waiting
		laserLine.enabled = false;
	}
}