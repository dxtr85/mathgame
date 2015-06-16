using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

	public GameObject player;
	public float moveSpeed;
	public float rotationSpeed;
	public Vector3 moveDirection;
	public CharacterController character;
	public float timeToSelectNextIdleAnim = 1.0f;
	public float timeSinceFeelingAnimationStarted = 0.0f;

	private float timeThatHasPassed = 0f;
	private float maxSpeed;
	private HashIDs hash;
	private Animator anim;
	private bool walkingEnabled;

	private Vector3 zeroAcc, curentAcc;
	private float sensibility = 0.04f;
	private float mouseX, mouseY;
	private bool mouseButtonWalkingPressed = false;
	private int turnSensibility = 4;
	private AudioSource walkingAudio;

	// Use this for initialization
	void Start () {
		moveDirection = Vector3.zero;
		character = GetComponent<CharacterController>();
		hash = player.GetComponent<HashIDs> ();
		anim = player.GetComponent<Animator> ();
		walkingEnabled = true;
		maxSpeed = 30;
		zeroAcc = Input.acceleration;
		curentAcc = Vector3.zero;

		walkingAudio = GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("audioWalk").audio;

		UpdateMousePosition ();
	}
	
	// Update is called once per frame
	void Update () {

		//Update mouse position, if present
		UpdateMousePosition ();

		curentAcc = Vector3.Lerp (curentAcc, Input.acceleration - zeroAcc, Time.deltaTime / sensibility);
		if ((curentAcc.z > (-1) * sensibility) && (curentAcc.z < sensibility))
						curentAcc.z = 0;
		if (WalkingEnabled) {
						timeThatHasPassed += Time.deltaTime;
						transform.Rotate (new Vector3 (0, (turnSensibility * curentAcc.x + mouseX) * rotationSpeed * Time.deltaTime, 0));
						moveDirection = Vector3.forward * (mouseY - curentAcc.z);// + Vector3.right * Input.GetAxis("Horizontal");

						//Enable walking audio if curAcc.z !=0 || mouseButton(1) pressed
						if (curentAcc.z != 0 || Input.GetMouseButton(1))
							if(!walkingAudio.isPlaying && AudioOn() )
								walkingAudio.Play ();
						//Disable walking audio if curAcc.z ==0 && mouseButton(1) released
						if (curentAcc.z == 0 && (!Input.GetMouseButton(1)))
							if(walkingAudio.isPlaying)
								walkingAudio.Stop ();

						//If RMB pressed then move
						if (mouseButtonWalkingPressed) {
								moveDirection = transform.TransformDirection (moveDirection).normalized;
								moveDirection *= moveSpeed * (-1);
								anim.SetFloat (hash.speedFloat, moveSpeed);
								timeThatHasPassed = timeToSelectNextIdleAnim;
								
						} else {
								anim.SetFloat (hash.speedFloat, 0.0f);
								if (timeThatHasPassed >= timeToSelectNextIdleAnim) {
										anim.SetFloat (hash.idleSelectorFloat, (float)Random.value);
										timeThatHasPassed = 0.0f;
								}
						}
						character.Move (moveDirection * Time.deltaTime);
				} else {
			//If walking disabled, stop playing audio for walking
			walkingAudio.Stop();
			}

		if (anim.GetBool (hash.isHappyBool) || anim.GetBool (hash.isSadBool)) {
			timeSinceFeelingAnimationStarted+= Time.deltaTime;
			if (timeSinceFeelingAnimationStarted>0.1f)
				ResetFeelings();
		}
	}

	public void IncreaseMoveSpeed(){
		moveSpeed*=1.07f;
		if (moveSpeed > maxSpeed)
						moveSpeed = maxSpeed;
	}
	public bool WalkingEnabled{
		get{return walkingEnabled;}
		set{walkingEnabled=value;}
	}

	public void RecalibrateAccelerometer(){
		zeroAcc = Input.acceleration;
	}

	public void SetHappy(){
				anim.SetBool (hash.isHappyBool, true);
	}

	public void SetSad(){
				anim.SetBool (hash.isSadBool, true);
		}
	public void ResetFeelings(){
		anim.SetBool (hash.isHappyBool, false);
		anim.SetBool (hash.isSadBool, false);
		timeSinceFeelingAnimationStarted = 0.0f;
	}

	private void UpdateMousePosition(){

		mouseX = (Input.mousePosition.x -Screen.width/2 )*2/Screen.width;
		mouseY = (Input.mousePosition.y - Screen.height/2)*2/Screen.height;
		mouseButtonWalkingPressed = Input.GetMouseButton(1);
		//*
		#if UNITY_ANDROID
		mouseX = 0.0f;
		mouseY = 0.0f;
		mouseButtonWalkingPressed = (curentAcc.z==0.0f?false:true);
		#endif
		#if UNITY_IPHONE
		mouseX = 0.0f;
		mouseY = 0.0f;
		mouseButtonWalkingPressed = (curentAcc.z==0.0f?false:true);
		#endif
		//*/
	}
	private bool AudioOn(){
		if(PlayerPrefs.GetInt("AudioEnabled")==1)
			return true;
		else 
			return false;
	}
}
