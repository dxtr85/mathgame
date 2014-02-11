using UnityEngine;
using System.Collections;

public class ShootingController : MonoBehaviour {

	public GameObject bullet;
	public float resizeValue;
	public float shootSpeed;
	public float growSpeed;

	private Vector3 targetScale;
	private Rigidbody projectile = null;
	private Animator anim;
	private HashIDs hash;
	private Camera camPlayer, camBullet;

	void Start () {
		camPlayer = Camera.main;
		anim = GameObject.FindGameObjectWithTag ("Player").GetComponent<Animator> ();
		hash = GameObject.FindGameObjectWithTag ("Player").GetComponent<HashIDs> ();
		anim.SetBool (hash.firePressedBool, false);
	}
	
	// Update is called once per frame
	void Update () {
		ResizeBulletAndShoot ();
	}
	void ResizeBulletAndShoot(){
		if (Input.GetMouseButtonDown (0)) {
			projectile = Instantiate (bullet.rigidbody, bullet.transform.position + 5*transform.forward, bullet.transform.rotation) as Rigidbody;
			anim.SetBool (hash.firePressedBool, true);
			targetScale.Set (resizeValue, resizeValue, resizeValue);
			camBullet = projectile.GetComponentInChildren<Camera>();
		}
		if (projectile) {
			if (projectile.transform.localScale != targetScale)
				projectile.transform.localScale = Vector3.Lerp (projectile.transform.localScale, targetScale, Time.deltaTime * growSpeed);
			if (projectile.transform.localScale.x >= 0.8 * resizeValue){
				anim.SetBool (hash.firePressedBool, false);
				projectile.rigidbody.isKinematic = false;
				projectile.AddForce(projectile.transform.forward * shootSpeed);

				camBullet.enabled = true;
				camPlayer.enabled = false;
			}
			if (Input.GetMouseButtonDown (1)) {
				camBullet.enabled = false;
				projectile = null;
				camBullet = null;
				camPlayer.enabled = true;
			}
		}
	}
}
