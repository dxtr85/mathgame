using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour {

	public int idleState;
	public int locomotionState;
	public int speedFloat;
	public int idleSelectorFloat;
	public int firePressedBool;
	public int isHappyBool;
	public int isSadBool;

	void Awake () {
	
		idleState = Animator.StringToHash ("Base Layer.PlayerIdle");
		locomotionState = Animator.StringToHash ("Base Layer.PlayerWalk");
		speedFloat = Animator.StringToHash ("Speed");
		idleSelectorFloat = Animator.StringToHash ("IdleSelector");
		firePressedBool = Animator.StringToHash ("FirePressed");
		isHappyBool =  Animator.StringToHash ("isHappy");
		isSadBool =  Animator.StringToHash ("isSad");
	}

}
