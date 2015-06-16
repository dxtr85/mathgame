using UnityEngine;
using System.Collections;

public class IntroLoad : MonoBehaviour {
	public int levelToLoad;
	AudioSource introAudio;
	// Use this for initialization
	void Start () {
		introAudio = GameObject.Find("Audio").audio;
	}
	
	// Update is called once per frame
	void Update () {
		if (!introAudio.isPlaying)
			Application.LoadLevel (levelToLoad);
	
	}
}
