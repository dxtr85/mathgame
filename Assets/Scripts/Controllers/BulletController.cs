using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float bulletLifetime;

	private GameObject bullet;
	private GameObject tutAction;
	private MouseController mouseCtrl;
	private ShootingController shootingCtrl;
	private DigitsController digitsCtrl;
	private GameObject player;
	private CameraController camCtrl;
	private GameObject symbolToBeAdded;
	private GameObject digitToBeAdded,d2;
	private PlayerMovement playerMovement;
	private GUIController guiCtrl;
	private TutorialController tutCtrl;
	private AudioSource ballHitOtherAudio, ballHitTargetAudio;
	private Animator anim;
	private HashIDs hash;

	// Use this for initialization
	void Start () {
		anim = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<Animator> ();
		bullet = GameObject.FindGameObjectWithTag (Tags.bullet);
		hash = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<HashIDs> ();
		tutAction = GameObject.FindGameObjectWithTag (Tags.tutorialAction);
		mouseCtrl = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<MouseController> ();
		shootingCtrl = GameObject.FindGameObjectWithTag(Tags.spawnPoint).GetComponent<ShootingController> ();
		digitsCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DigitsController> ();
		guiCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GUIController> ();
		player = GameObject.FindGameObjectWithTag (Tags.player);
		camCtrl = Camera.main.GetComponent<CameraController>();
		tutCtrl = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<TutorialController> ();
		playerMovement = player.GetComponent<PlayerMovement> ();
		ballHitOtherAudio = GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("audioBallHitOther").audio;
		ballHitTargetAudio = GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("audioBallHitTarget").audio;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (shootingCtrl.TimeAfterShotFired >= bulletLifetime && bullet != null) {
			camCtrl.SetNewTargetTransform(player.transform);
			GameObject.Destroy(bullet);
			shootingCtrl.ShootingEnabled = true;
			playerMovement.WalkingEnabled = true;
		}
	
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.gameObject.tag == Tags.digit && shootingCtrl.ShootingEnabled == false)
		{
			anim.SetBool (hash.firePressedBool, false);
			//Play a sound
			if (AudioOn()) ballHitTargetAudio.Play();

			//Disable time counter in shooting controller
			shootingCtrl.TimeCounterEnabled = false;
			//Evaluating what type of operation should be performed
			symbolToBeAdded =  (GameObject)Instantiate(digitsCtrl.returnOperationBestSuitableForDigit(other.GetComponent<DigitContext>().value));
			
			//Setting symbol's position
			symbolToBeAdded.transform.position 
				= other.transform.position - 6* Vector3.Cross(other.transform.position - player.transform.position,Vector3.up).normalized +5*Vector3.up;
			symbolToBeAdded.transform.rotation = Quaternion.Euler (symbolToBeAdded.transform.eulerAngles.x, player.transform.eulerAngles.y, 0);

			//Evaluating which digit should be selected for given digit and operation type
			int ind = digitsCtrl.returnIndexOfDigitBestSuitableForValueOf(other.gameObject.GetComponent<DigitContext>().value,
			                                                              symbolToBeAdded.GetComponent<SymbolContext>().value);
			//Debug.Log("Best suitable index is: "+ind.ToString());
			if (ind !=-1){
				Debug.Log("Index = "+ind.ToString());
				digitToBeAdded =  (GameObject)Instantiate(digitsCtrl.digitsAvailableToPlayer[ind]);}
			else
				digitToBeAdded =  (GameObject)Instantiate(digitsCtrl.digitsAvailableToPlayer[1]);

			//Setting digit's position
			digitToBeAdded.transform.position 
				= other.transform.position - 13* Vector3.Cross(other.transform.position - player.transform.position,Vector3.up).normalized;

			//To prevent from division by zero exception
			if (ind == 0){
				ind = Mathf.FloorToInt(Random.Range(0,3));
			}else{
				ind = Mathf.FloorToInt(Random.Range(0,4));
			}

			//Do the gesture recognition (incomplete)
			mouseCtrl.gestureCapturingEnabled = true;

			//Cleaning the bullet and creating equation
			GameObject.Destroy(bullet);
			digitsCtrl.AddInGameDigit(digitToBeAdded);
			digitsCtrl.CreateEquation(other.gameObject,symbolToBeAdded,digitToBeAdded);

			//Changing camera to point to symbol of equation
			camCtrl.SetNewTargetTransform(symbolToBeAdded.transform);

			//Initialize counter
			guiCtrl.InitializeCounter();
		}
		if(other.gameObject.tag == Tags.tutorialItem)
		{
			anim.SetBool (hash.firePressedBool, false);
			//Play a sound
			if (AudioOn()) ballHitTargetAudio.Play();

			tutCtrl.ItemUnderEvaluation = other.gameObject;
			mouseCtrl.gestureCapturingEnabled = true;
			//Create non-mathematical tutorial equation
			digitsCtrl.CreateEquation(other.gameObject, tutAction ,null);
			GameObject.Destroy(bullet);

			camCtrl.SetNewTargetTransform(other.transform);

			//playerMovement.WalkingEnabled = true;
			//shootingCtrl.ShootingEnabled = true;
			//camCtrl.SetNewTargetTransform(player.transform);
		}

		else if(other.gameObject.tag != Tags.digit && other.gameObject.tag != Tags.tutorialItem
		        && other.gameObject.tag != Tags.player)
		{
			//Play a sound
			if (AudioOn()) ballHitOtherAudio.Play();

			GameObject.Destroy(bullet);
			playerMovement.WalkingEnabled = true;
			shootingCtrl.ShootingEnabled = true;
			camCtrl.SetNewTargetTransform(player.transform);
		}
	}

	private bool AudioOn(){
		if(PlayerPrefs.GetInt("AudioEnabled")==1)
			return true;
		else 
			return false;
	}
}
