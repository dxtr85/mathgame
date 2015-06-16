using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public float smooth = 1.5f;         // The relative speed at which the camera will catch up.
	
	private Camera cam;
	private Transform target;           // Reference to the player's transform.

	void Awake ()
	{
		// Setting up the reference.
		target = GameObject.FindGameObjectWithTag(Tags.player).transform;
		cam = Camera.main;
		// Setting the relative position as the initial relative position of the camera in the scene.
	}
	
	
	void FixedUpdate ()
	{
		cam.transform.rotation = Quaternion.Euler (12, target.eulerAngles.y+180, 0);
		// Lerp the camera's position between it's current position and it's new position.

		cam.transform.position = Vector3.Lerp(cam.transform.position,target.position + 30 *(transform.rotation * Vector3.back)+ 10*Vector3.up,2.5f*Time.deltaTime); 

	}

	public void SetNewTargetTransform(Transform newTarget){
		target = newTarget;
	}

}
