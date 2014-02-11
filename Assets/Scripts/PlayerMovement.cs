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

	private float timeThatHasPassed = 0f;
	private HashIDs hash;
	private Animator anim;

	// Use this for initialization
	void Start () {
		moveDirection = Vector3.zero;
		character = GetComponent<CharacterController>();
		hash = player.GetComponent<HashIDs> ();
		anim = player.GetComponent<Animator> ();
		//anim.SetBool (hash.firePressedBool, false);
	}
	
	// Update is called once per frame
	void Update () {
		timeThatHasPassed += Time.deltaTime;
		transform.Rotate (new Vector3(0, Input.GetAxis ("Horizontal") * rotationSpeed * Time.deltaTime, 0));
		moveDirection = Vector3.forward * Input.GetAxis ("Vertical");// + Vector3.right * Input.GetAxis("Horizontal");
		moveDirection = transform.TransformDirection (moveDirection).normalized;
		moveDirection *= moveSpeed*(-1);
		if (Input.GetAxis ("Vertical") != 0.0f ){//|| Input.GetAxis ("Horizontal") != 0.0f) {
						anim.SetFloat (hash.speedFloat, moveSpeed);
			timeThatHasPassed = timeToSelectNextIdleAnim;
		}
		else {
			anim.SetFloat (hash.speedFloat, 0.0f);
			if (timeThatHasPassed>=timeToSelectNextIdleAnim){
				anim.SetFloat (hash.idleSelectorFloat, (float)Random.value);
				timeThatHasPassed = 0.0f;
			}
		}
		character.Move(moveDirection * Time.deltaTime);

	}

}
