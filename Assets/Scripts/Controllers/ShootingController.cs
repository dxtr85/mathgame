using UnityEngine;
using System.Collections;

public class ShootingController : MonoBehaviour {

	public GameObject bullet;
	public float resizeValue;
	public float shootSpeed;
	public float growSpeed;

	private bool timeCounterEnabled;
	private float timeAfterShotFired = 0.0f;
	private bool shootingEnabled = true;
	private Vector3 targetScale;
	private Rigidbody projectile = null;
	private Animator anim;
	private HashIDs hash;
	private CameraController camCtrl;
	private PlayerMovement playerMovement;

	private AudioSource ballShotAudio;

	void Start () {
		timeCounterEnabled = true;
		camCtrl = Camera.main.GetComponent<CameraController>();
		anim = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<Animator> ();
		hash = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<HashIDs> ();
		playerMovement = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<PlayerMovement> ();
		anim.SetBool (hash.firePressedBool, false);

		ballShotAudio = GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("audioBallShot").audio;
	}
	
	// Update is called once per frame
	void Update () {
		ResizeBulletAndShoot ();

		// When nothing has been hit yet, increase time since bullet was shoot
		if (timeCounterEnabled)
			timeAfterShotFired += Time.deltaTime;

		// This is used to make sure proper animation is played
		if (timeAfterShotFired >= 0.1f){
			anim.SetBool (hash.firePressedBool, false);
		}
	}

	void ResizeBulletAndShoot(){
		if (Input.GetMouseButtonDown (0) && ShootingEnabled &&
		    ! ((Input.mousePosition.x >= Screen.width * 7 / 8)
		   && (Input.mousePosition.y >= Screen.height * 9 / 10))){
			//Disable both moving and shooting
			shootingEnabled = false;

			//Disable Player walking
			playerMovement.WalkingEnabled=false;

			// Create a copy of "in head" golden bullet and set its tag
			projectile = Instantiate (bullet.rigidbody, bullet.transform.position - 5*transform.forward, bullet.transform.rotation) as Rigidbody;
			projectile.tag = Tags.bullet;

			//Make a sound
			if (AudioOn ()) ballShotAudio.Play();

			// Make camera follow bullet
			camCtrl.SetNewTargetTransform(projectile.transform);

			// Modify Animation controller's value to true
			anim.SetBool (hash.firePressedBool, true);
			targetScale.Set (resizeValue, resizeValue, resizeValue);

			//Enable time counting and reset counter
			timeCounterEnabled = true;
			timeAfterShotFired = 0.0f;
		}
	/*	if (Input.GetMouseButtonDown (0) && ShootingEnabled &&
		         ((Input.mousePosition.x >= Screen.width * 7 / 8)
		         && (Input.mousePosition.y >= Screen.height * 9 / 10))){
			Debug.Log("Menu pressed.");
			Application.LoadLevel(0);
		}
		*/

		if (projectile) {
			if (projectile.transform.localScale != targetScale)
				projectile.transform.localScale = Vector3.Lerp (projectile.transform.localScale, targetScale, Time.deltaTime * growSpeed);
			if (projectile.transform.localScale.x >= 0.8 * resizeValue){
				//anim.SetBool (hash.firePressedBool, false);
				projectile.rigidbody.isKinematic = false;
				projectile.AddForce(-1*projectile.transform.forward * shootSpeed);

			}
		}
	}

	public bool ShootingEnabled{
		get{ return shootingEnabled;}
		set{shootingEnabled = value;}
	}
	public float TimeAfterShotFired{
		get{return timeAfterShotFired;}
		set{timeAfterShotFired=value;}
	}
	public bool TimeCounterEnabled{
		get{ return shootingEnabled;}
		set{timeCounterEnabled = value;}
	}
	private bool AudioOn(){
		if(PlayerPrefs.GetInt("AudioEnabled")==1)
			return true;
		else 
			return false;
	}
	
}
