using UnityEngine;
using System.Collections;
using System.IO;

public class AreYouSureMenuButtonsController : MonoBehaviour {
	public bool opt1enabled;
	public bool opt2enabled;
	public bool opt3enabled;
	public bool opt4enabled;

	public bool prevEnabled;
	public bool cancelEnabled;
	public bool nextEnabled;

	public string opt1text;
	public string opt2text;
	public string opt3text;
	public string opt4text;

	public string prevText;
	public string cancelText;
	public string nextText;

	public int opt1level;
	public int opt2level;
	public int opt3level;
	public int opt4level;

	public int prevLevel;
	public int cancelLevel;
	public int nextLevel;

	public Texture option1, option2, option3, option4, cancel, prev, next;

	private int minLvlNeedingUnlock = 2;
	private int maxLvlNeedingUnlock = 23;

	private AudioSource buttonClick;
	private GUIStyle buttonStyle;

	// Use this for initialization
	void Start () {
		buttonClick = GameObject.FindGameObjectWithTag (Tags.mainCamera).transform.Find ("audioButton").audio;	
		
		buttonStyle = GUIStyle.none;
		buttonStyle.stretchWidth = true;
		buttonStyle.stretchHeight = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

// Option 1
			if ((ButtonVisible(opt1enabled, opt1level)) && 
		    	GUI.Button(new Rect ((float)(Screen.width * 3/8), (float)(Screen.height *2 / 8), 
		        		             (float)(Screen.width * 1/4), (float)(Screen.height / 8)),opt1text))
			{
				if (AudioOn())buttonClick.Play();
				Application.LoadLevel(opt1level);
			}
// Option 2
			if ((ButtonVisible(opt2enabled, opt2level)) && 
		    	GUI.Button(new Rect ((float)(Screen.width * 3/8), (float)(Screen.height *3 / 8), 
			                         (float)(Screen.width * 1/4), (float)(Screen.height / 8)),opt2text))
			{
				if (AudioOn())buttonClick.Play();
				Application.LoadLevel(opt2level);
			}
// Option 3
 			if ((ButtonVisible(opt3enabled, opt3level)) && 
			    GUI.Button(new Rect ((float)(Screen.width * 3/8), (float)(Screen.height *4 / 8), 
				                     (float)(Screen.width * 1/4), (float)(Screen.height / 8)),opt3text))
			{
				if (AudioOn())buttonClick.Play();
				Application.LoadLevel(opt3level);
			}
// Option 4
			if ((ButtonVisible(opt4enabled, opt4level)) && 
			    GUI.Button(new Rect ((float)(Screen.width * 3/8), (float)(Screen.height *5 / 8), 
		    		                 (float)(Screen.width * 1/4), (float)(Screen.height / 8)), opt4text))
			{
				if (AudioOn())buttonClick.Play();
				Application.LoadLevel(opt4level);
			}
//TODO
// Prev
			if ((ButtonVisible(prevEnabled, prevLevel)) && 
		    GUI.Button(new Rect ((float)(float)(Screen.width - prev.width) * 1/8, (float)(Screen.height *6.5 / 8), 
				                     (float)(prev.width), (float)(Screen.height / 8)), prev,buttonStyle))
			{
			if (File.Exists (Application.persistentDataPath + "/" + PlayerPrefs.GetString ("Player") + "mathGame.dat"))
				File.Delete (Application.persistentDataPath + "/" + PlayerPrefs.GetString ("Player") + "mathGame.dat");
			PlayerPrefs.SetInt("Continue",1);
			PlayerPrefs.SetInt ("LastAvailableLevel", 1);
			if (AudioOn())buttonClick.Play();
			Application.LoadLevel (prevLevel);
			}
		// Cancel
		if ((ButtonVisible(cancelEnabled, cancelLevel)) && 
		    GUI.Button(new Rect ((float)(Screen.width - cancel.width) * 4/8, (float)(Screen.height *6.5 / 8), 
		                     (float)(cancel.width), (float)(Screen.height / 8)), cancel,buttonStyle))
		{
			if (AudioOn())buttonClick.Play();
			Application.LoadLevel(cancelLevel);
		}

		// Next
		if ((ButtonVisible(nextEnabled, nextLevel)) && 
		    GUI.Button(new Rect ((float)(Screen.width - next.width) * 7/8, (float)(Screen.height *6.5 / 8), 
		                     (float)(next.width), (float)(Screen.height / 8)), next,buttonStyle))
		{
			if (AudioOn())buttonClick.Play();
			Application.LoadLevel(nextLevel);
		}


	}

	private bool ButtonVisible(bool buttonEnabled, int levelToLoad){
		if (buttonEnabled && ((levelToLoad < minLvlNeedingUnlock) || (levelToLoad > maxLvlNeedingUnlock)))
			return true;
		else {
			if (buttonEnabled && PlayerPrefs.GetInt ("LastAvailableLevel") >= levelToLoad)
				return true;
			else
				return false;
		}
		
	}
	private bool AudioOn(){
		if(PlayerPrefs.GetInt("AudioEnabled")==1)
			return true;
		else 
			return false;
	}
}
