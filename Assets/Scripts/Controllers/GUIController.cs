using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour {
	
	public GUIStyle  myGUIStyleBack;
	public GUIStyle  myGUIStyleFront;
	public GUIStyle  myGUIStyleCounter;
	public GUIStyle  myGUIStyleEquation;
	//public Texture2D txGreat,txGood,txDanger;
	//public Color colorGreat, colorGood, colorDanger;
	public float counterMax;

	public Texture menu, resume, recalibrate, audioOn, audioOff, quit;
	
	private int expPerc;
	private float counter;
	private PlayerExperience playerExp;
	private PlayerMovement playerMvt;
	private DigitsController digitCtrl;
	private ShootingController shootCtrl;
	private bool displayCounter = false;
	private bool displayEquation = false;
	private Texture2D texture;
	private bool inGameMenuOn;

	private string guiText;
	private string guiTextInfo;
	private float guiTextDisplayCounter,guiTextDisplayDuration;
	private GUIStyle buttonStyle;

	private AudioSource buttonClick;

	void Start () {
		playerExp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<PlayerExperience> ();
		playerMvt = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<PlayerMovement> ();
		digitCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DigitsController> ();
		shootCtrl = GameObject.FindGameObjectWithTag(Tags.spawnPoint).GetComponent<ShootingController> ();
		guiText = "";//GameObject.FindGameObjectWithTag(Tags.text).GetComponent<GUIText>();
		//guiText.transform.position = new Vector3 (Screen.width / 2, Screen.height / 2, 1);
		//guiTextInfo = GameObject.FindGameObjectWithTag(Tags.textInfo).GetComponent<GUIText>();

		texture = new Texture2D (1, 1);
		texture.SetPixel (0, 0, Color.clear);
		texture.Apply ();

		myGUIStyleCounter.normal.background = texture;
		counter = 0.0f;

		buttonClick = GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("audioButton").audio;	

		buttonStyle = GUIStyle.none;
		buttonStyle.stretchWidth = true;
		buttonStyle.stretchHeight = true;

		SetGuiText ("Go, go, go!!!", 5.0f, Color.blue);
		inGameMenuOn = false;
	}

	void OnGUI() {
		expPerc = playerExp.ExperiencePerc;
		GUI.Box (new Rect (0, 0, (float)(Screen.width / 2), (float)(0.1 * Screen.height)),  "Level: " + playerExp.Level + "\t\tExp: " + expPerc.ToString () + "%",myGUIStyleBack);
		GUI.Box (new Rect (0, 0, (float)(0.005 * Screen.width * expPerc), (float)(0.1 * Screen.height)),"",myGUIStyleFront);
		if (displayCounter) {
			GUI.Box (new Rect ((float)(Screen.width * (0.5 + (0.25) * counter / counterMax)), 0, 
	        (float)(Screen.width * (0.5 - (0.5) * counter / counterMax)), (float)(0.1 * Screen.height)), 
		         "Hurry!", myGUIStyleCounter);	
		} else {
			if (GUI.Button(new Rect ((float)(Screen.width - Screen.height*0.2), 0, 
			                     (float)(Screen.width)*1/8, (float)(0.1 * Screen.height)),menu,buttonStyle))
			{
				if (AudioOn()) buttonClick.Play();
				openInGameMenu();
			}
			//GUI.Box (new Rect ((float)(Screen.width * 7/8), 0, 
			  //       (float)(Screen.width * 1/8), (float)(0.1 * Screen.height)),"Menu",myGUIStyleBack); 
		}

		//Info about how many items left
		if (guiTextInfo != null)
			GUI.Button (new Rect (0.0f, (float)(Screen.height * 0.1), (float)(Screen.width / 8), (float)(Screen.height * 0.06)), guiTextInfo);
		//General info with equation to solve
		if (displayEquation)
			GUI.Box (new Rect ((float)(Screen.width / 8 * 3), (float)(0.1 * Screen.height), (float)(Screen.width / 4), (float)(0.1 * Screen.height)), guiText,myGUIStyleEquation);

		//If InGameMenu should be visible
		if (inGameMenuOn) {
			//GUI.Box (new Rect ((float)(Screen.width / 4), (float)(Screen.height / 4), (float)(Screen.width *3 / 4), (float)(Screen.height / 2)),  "Level: " + playerExp.Level + "Menu",myGUIStyleMenu);	
			//Button for resume
			if (GUI.Button(new Rect ((float)(Screen.width - resume.width) * 1/2, (float)(Screen.height *3 / 8), 
			                         (float)(resume.width), (float)(Screen.height / 8)),resume,buttonStyle))
			{
				if (AudioOn()) buttonClick.Play();
				closeInGameMenu();
			}
			//Button for recalibrate
			if (GUI.Button(new Rect ((float)(Screen.width - recalibrate.width) * 1/2, (float)(Screen.height * 4 / 8),
			                         (float)(recalibrate.width), (float)(Screen.height / 8)),recalibrate,buttonStyle))
			{
				if (AudioOn()) buttonClick.Play();
				recalibrateAccelerometer();
			}
			//Button for Quit
			if (GUI.Button(new Rect ((float)(Screen.width - audioOn.width) * 1/2, (float)(Screen.height * 5 / 8),
			                         (float)(audioOn.width), (float)(Screen.height / 8)),AudioOn ()?audioOn:audioOff,buttonStyle))
			{
				if (AudioOn()) PlayerPrefs.SetInt ("AudioEnabled",0);
				else{  
					PlayerPrefs.SetInt ("AudioEnabled",1);
					buttonClick.Play ();
				}
			}
			//Button for Quit
			if (GUI.Button(new Rect ((float)(Screen.width - quit.width) * 1/2, (float)(Screen.height * 6 / 8),
			                         (float)(quit.width), (float)(Screen.height / 8)),quit,buttonStyle))
			{
				if (AudioOn()) buttonClick.Play();
				quitToMainMenu();
			}
		}
	}

	void Update () {
		if (displayCounter) {
						counter += Time.deltaTime;
						if (counter >= 0 && counter < 0.2 * counterMax) {
								texture.SetPixel (0, 0, Color.green);
								texture.Apply ();
								myGUIStyleCounter.normal.background = texture;
						}
						if (counter >= 0.2 * counterMax && counter < 0.8 * counterMax) {
								texture.SetPixel (0, 0, Color.yellow);
								texture.Apply ();
						}
						if (counter >= 0.8 * counterMax && counter < counterMax) {
								texture.SetPixel (0, 0, Color.red);
								texture.Apply ();
						}
						if (counter >= counterMax) {
								digitCtrl.ForceVerification ();
								DestroyCounter ();
						}
				} 
		UpdateGuiText ();
	}

	public void InitializeCounter(){
		displayCounter = true;
		counter = 0.0f;
	}
	public void DestroyCounter(){
		displayCounter = false;
		counter = 0.0f;
	}

	public float Counter{
		get{return counter;}
	}

	public float CounterMax{
		get { return counterMax;}
	}

	public void SetGuiText(string text, float duration, Color color){
		guiText = text;
		myGUIStyleEquation.normal.textColor = color;
		guiTextDisplayDuration = duration;
		guiTextDisplayCounter = 0.0f;
		displayEquation = true;
	}

	public void SetGuiTextInfo(string text){
		guiTextInfo = text;
	}

	private void UpdateGuiText(){
		guiTextDisplayCounter += Time.deltaTime;
		if (guiTextDisplayCounter >= guiTextDisplayDuration) {
			SetGuiText ("", 3600.0f, Color.white);
			displayEquation = false;
		}
	}
	private void ClearGuiText(){
		guiText = "";
		displayEquation = false;
	}

	private void openInGameMenu(){
		inGameMenuOn = true;
		digitCtrl.AdsHide ();
		playerMvt.WalkingEnabled = false;
		shootCtrl.ShootingEnabled = false;
	}
	private void closeInGameMenu(){
		inGameMenuOn = false;
		digitCtrl.AdsShow ();
		playerMvt.WalkingEnabled = true;
		shootCtrl.ShootingEnabled = true;
		//AdBuddizBinding.ShowAd();
	}
	private void quitToMainMenu(){
		digitCtrl.AdsKill ();
		//AdBuddizBinding.ShowAd();
		Application.LoadLevel (39); //Full version 31);
	}
	private void recalibrateAccelerometer(){
		//inGameMenuOn = false;
		//playerMvt.WalkingEnabled = true;
		//shootCtrl.ShootingEnabled = true;
		playerMvt.RecalibrateAccelerometer();
	}

	private bool AudioOn(){
		if(PlayerPrefs.GetInt("AudioEnabled")==1)
			return true;
		else 
			return false;
	}
}