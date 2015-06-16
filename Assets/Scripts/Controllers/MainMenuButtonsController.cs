using UnityEngine;
using System.Collections;
//using GoogleMobileAds.Api;

public class MainMenuButtonsController : MonoBehaviour {
	public bool opt1enabled = true;
	public bool opt2enabled = true;
	public bool opt3enabled = true;
	public bool opt4enabled = true;

	public bool prevEnabled = false;
	public bool cancelEnabled = false;
	public bool nextEnabled = false;

	public string opt1text = "New Game";
	public string opt2text = "Continue Game";
	public string opt3text = "Help";
	public string opt4text = "Exit";

	public string prevText;
	public string cancelText;
	public string nextText;

	public int opt1level = 18;
	public int opt2level = 18;
	public int opt3level = 25;
	public int opt4level;

	public int prevLevel;
	public int cancelLevel;
	public int nextLevel;

	public Texture option1,option2,option3,option4,prev,cancel,next;

	private int minLvlNeedingUnlock = 2;
	private int maxLvlNeedingUnlock = 23;

	private AudioSource buttonClick;
	private GUIStyle buttonStyle;
	private float timeThatPassed;
	private bool isQuitting;
	//private InterstitialAd interstitial;

	// Use this for initialization
	void Start () {
		buttonClick = GameObject.FindGameObjectWithTag (Tags.mainCamera).transform.Find ("audioButton").audio;	
		if (!PlayerPrefs.HasKey("AudioEnabled"))PlayerPrefs.SetInt ("AudioEnabled", 1);

		buttonStyle = GUIStyle.none;
		buttonStyle.stretchWidth = true;
		buttonStyle.stretchHeight = true;

		// Create an empty ad request.
		//InterstitialAd interstitial = new InterstitialAd("ca-app-pub-8079075054121466/4074432231");
		//AdRequest request = new AdRequest.Builder().Build();
		//interstitial.LoadAd(request);

	}
	
	// Update is called once per frame
	void Update () {	
	}

	void OnGUI() {

// Option 1
			if ((ButtonVisible(opt1enabled, opt1level)) && 
		    GUI.Button(new Rect ((float)(Screen.width - option1.width) * 1/2, (float)(Screen.height *2 / 8), 
		        		             (float)(option1.width), (float)(Screen.height / 8)),option1,buttonStyle))
			{
			PlayerPrefs.SetInt ("Continue", 0);
			PlayerPrefs.SetString ("Player", "");
			PlayerPrefs.SetInt ("LastAvailableLevel", 1);
			if (AudioOn()) buttonClick.Play();
			Application.LoadLevel(opt1level);
			}
// Option 2
			if ((ButtonVisible(opt2enabled, opt2level)) && 
		    GUI.Button(new Rect ((float)(Screen.width - option2.width) * 1/2, (float)(Screen.height *3 / 8), 
			                         (float)(option2.width), (float)(Screen.height / 8)),option2,buttonStyle))
			{
			PlayerPrefs.SetInt ("Continue", 1);
			PlayerPrefs.SetString ("Player", "");
			if (AudioOn()) buttonClick.Play();
			Application.LoadLevel(opt2level);
			}
// Option 3
 			if ((ButtonVisible(opt3enabled, opt3level)) && 
		    GUI.Button(new Rect ((float)(Screen.width - option3.width) * 1/2, (float)(Screen.height *4 / 8), 
				                     (float)(option3.width), (float)(Screen.height / 8)),option3,buttonStyle))
			{
			if (AudioOn()) buttonClick.Play();
				Application.LoadLevel(opt3level);
			}
// Option 4
			if ((ButtonVisible(opt4enabled, opt4level)) && 
		    GUI.Button(new Rect ((float)(Screen.width - option4.width) * 1/2, (float)(Screen.height *5 / 8), 
		    		                 (float)(option4.width), (float)(Screen.height / 8)), option4,buttonStyle))
			{
			if (AudioOn()) buttonClick.Play();
				//if (interstitial.IsLoaded()) {
				//	interstitial.Show();
				//}
				Application.Quit();
			}
// Prev
			if ((ButtonVisible(prevEnabled, prevLevel)) && 
			    GUI.Button(new Rect ((float)(Screen.width * 1/8), (float)(Screen.height *6.5 / 8), 
				                     (float)(Screen.width * 1/4), (float)(Screen.height / 8)), prevText))
			{
			if (AudioOn()) buttonClick.Play();
				Application.LoadLevel(prevLevel);
			}
		// Cancel
		if ((ButtonVisible(cancelEnabled, cancelLevel)) && 
		    GUI.Button(new Rect ((float)(Screen.width * 3/8), (float)(Screen.height *6.5 / 8), 
		                     (float)(Screen.width * 1/4), (float)(Screen.height / 8)), cancelText))
		{
			if (AudioOn()) buttonClick.Play();
			Application.LoadLevel(cancelLevel);
		}

		// Audio ENABLED after clicking changes to disabled
		if ((PlayerPrefs.GetInt ("AudioEnabled")==1) && 
		    GUI.Button(new Rect ((float)(Screen.width * 5/8), (float)(Screen.height *6.5 / 8), 
		                     (float)(next.width), (float)(Screen.height / 8)), next,buttonStyle))
		{
			//buttonClick.Play();
			PlayerPrefs.SetInt ("AudioEnabled", 0);
		}
		// Audio DISABLED after clicking changes to enabled
		if ((PlayerPrefs.GetInt ("AudioEnabled")==0) && 
		    GUI.Button(new Rect ((float)(Screen.width * 5/8), (float)(Screen.height *6.5 / 8), 
		                     (float)(cancel.width), (float)(Screen.height / 8)), cancel,buttonStyle))
		{
			buttonClick.Play();
			PlayerPrefs.SetInt ("AudioEnabled", 1);
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
