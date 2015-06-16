using UnityEngine;
using System.Collections;
using System.IO;

public class SaveSlotMenuButtonsController : MonoBehaviour {
	public bool opt1enabled = true;
	public bool opt2enabled = true;
	public bool opt3enabled = true;
	public bool opt4enabled = true;

	public bool prevEnabled = false;
	public bool cancelEnabled  = true;
	public bool nextEnabled = false;

	public string opt1text = "Alfa";
	public string opt2text = "Beta";
	public string opt3text = "Gamma";
	public string opt4text = "Delta";

	private string opt1textPrv = "Alfa";
	private string opt2textPrv = "Beta";
	private string opt3textPrv = "Gamma";
	private string opt4textPrv = "Delta";

	public string prevText;
	public string cancelText = "Cancel";
	public string nextText;

	private int opt1level;
	private int opt2level;
	private int opt3level;
	private int opt4level;

	public int currentLevel = 18;
	public int areYouSureLevel = 19;
	public int selectLevel = 20;

	public int prevLevel;
	public int cancelLevel;
	public int nextLevel;

	public Texture option1, option2, option3, option4, cancel, prev, next;
	public Texture option1empty, option2empty, option3empty, option4empty;

	private Texture opt1prv, opt2prv,opt3prv,opt4prv;

	private int minLvlNeedingUnlock = 2;
	private int maxLvlNeedingUnlock = 23;

	private ProgressManager progMan;

	private AudioSource buttonClick;
	private GUIStyle buttonStyle;

	private void InitializeTextForOption(){
		//New Game
		if (PlayerPrefs.GetInt("Continue")==0){
			//Alfa
			if (File.Exists(Application.persistentDataPath +"/"+opt1textPrv+ "mathGame.dat")){
				opt1text = opt1textPrv + " USED!";
				opt1prv = option1;
				opt1level = areYouSureLevel;
			}
			else{
				opt1prv = option1empty;
				opt1text = opt1textPrv;
				opt1level = selectLevel;
			}
			//Beta
			if (File.Exists(Application.persistentDataPath +"/"+opt2textPrv+ "mathGame.dat"))
			{
				opt2prv = option2;
				opt2text = opt2textPrv + " USED!";
				opt2level = areYouSureLevel;
			}
			else{
				opt2prv = option2empty;
				opt2text = opt2textPrv;
				opt2level = selectLevel;
			}
			//Gamma
			if (File.Exists(Application.persistentDataPath +"/"+opt3textPrv+ "mathGame.dat")){
				opt3prv = option3;
				opt3text = opt3textPrv + " USED!";
				opt3level = areYouSureLevel;
			}
			else{
				opt3prv = option3empty;
				opt3text = opt3textPrv;
				opt3level = selectLevel;
			}
			//Delta
			if (File.Exists(Application.persistentDataPath +"/"+opt4textPrv+ "mathGame.dat")){
				opt4prv = option4;
				opt4text = opt4textPrv + " USED!";
				opt4level = areYouSureLevel;
			}
			else{
				opt4prv = option4empty;
				opt4text = opt4textPrv;
				opt4level = selectLevel;
			}

		}
		else if (PlayerPrefs.GetInt("Continue")==1){
			//Alfa
			if (File.Exists(Application.persistentDataPath +"/"+opt1textPrv+ "mathGame.dat")){
				opt1prv = option1;
				opt1text = opt1textPrv;
				opt1level = selectLevel;
			}
			else{
				opt1prv = option1empty;
				opt1text = opt1textPrv + " - unused";
				opt1level = currentLevel;
			}
			//Beta
			if (File.Exists(Application.persistentDataPath +"/"+opt2textPrv+ "mathGame.dat")){
				opt2prv = option2;
				opt2text = opt2textPrv;
				opt2level = selectLevel;
			}
			else{
				opt2prv = option2empty;
				opt2text = opt2textPrv + " - unused";
				opt2level = currentLevel;
			}
			//Gamma
			if (File.Exists(Application.persistentDataPath +"/"+opt3textPrv+ "mathGame.dat")){
				opt3prv = option3;
				opt3text = opt3textPrv;
				opt3level = selectLevel;
			}
			else{
				opt3prv = option3empty;
				opt3text = opt3textPrv + " - unused";
				opt3level = currentLevel;
			}
			if (File.Exists(Application.persistentDataPath +"/"+opt4textPrv+ "mathGame.dat"))
			{
				opt4prv = option4;
				opt4text = opt4textPrv;
				opt4level = selectLevel;
			}
			else{
				opt4prv = option4empty;
				opt4text = opt4textPrv + " - unused";
				opt4level = currentLevel;
			}
			
		}


	}
	// Use this for initialization
	void Start () {
		buttonClick = GameObject.FindGameObjectWithTag (Tags.mainCamera).transform.Find ("audioButton").audio;	
		//currentColor = Color.green;
		progMan = new ProgressManager ();
		InitializeTextForOption ();

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
		    GUI.Button(new Rect ((float)(Screen.width - option1.width) * 1/2, (float)(Screen.height *2 / 8), 
		                     (float)(option1.width), (float)(Screen.height / 8)),opt1prv,buttonStyle))
		{
			LevelLoader(opt1textPrv,opt1level);
			}
// Option 2
		if ((ButtonVisible(opt2enabled, opt2level)) && 
		    GUI.Button(new Rect ((float)(Screen.width - option1.width) * 1/2, (float)(Screen.height *3 / 8), 
		                     (float)(option2.width), (float)(Screen.height / 8)),opt2prv,buttonStyle))
		{
			LevelLoader(opt2textPrv,opt2level);
			}
// Option 3
		if ((ButtonVisible(opt3enabled, opt3level)) && 
		    GUI.Button(new Rect ((float)(Screen.width - option3.width) * 1/2, (float)(Screen.height *4 / 8), 
		                     (float)(option3.width), (float)(Screen.height / 8)),opt3prv,buttonStyle))
		{
			LevelLoader(opt3textPrv,opt3level);
			}
// Option 4
		if ((ButtonVisible(opt4enabled, opt4level)) && 
		    GUI.Button(new Rect ((float)(Screen.width - option4.width) * 1/2, (float)(Screen.height *5 / 8), 
		                     (float)(option1.width), (float)(Screen.height / 8)),opt4prv,buttonStyle))
		{
			LevelLoader(opt4textPrv,opt4level);
			}
//TODO
// Prev
		if ((ButtonVisible(prevEnabled, prevLevel)) && 
		    GUI.Button(new Rect ((float)(Screen.width - prev.width) * 1/8, (float)(Screen.height *6.5 / 8), 
		                     (float)(prev.width), (float)(Screen.height / 8)), prev,buttonStyle))
		{
			if (AudioOn()) buttonClick.Play();
				Application.LoadLevel(prevLevel);
			}
		// Cancel
		if ((ButtonVisible(cancelEnabled, cancelLevel)) && 
		    GUI.Button(new Rect ((float)(Screen.width -cancel.width)* 4/8, (float)(Screen.height *6.5 / 8), 
		                     (float)(cancel.width), (float)(Screen.height / 8)), cancel,buttonStyle))
		{
			if (AudioOn()) buttonClick.Play();
			Application.LoadLevel(cancelLevel);
		}

		// Next
		if ((ButtonVisible(nextEnabled, nextLevel)) && 
		    GUI.Button(new Rect ((float)(Screen.width - next.width) * 7/8, (float)(Screen.height *6.5 / 8), 
		                     (float)(next.width), (float)(Screen.height / 8)), next,buttonStyle))
		{
			if (AudioOn()) buttonClick.Play();
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

	private void LevelLoader(string slotName, int levelNumber){
		//Debug.Log ("Dupa ");
		PlayerPrefs.SetString ("Player", slotName);

		if (AudioOn()) buttonClick.Play();
		
		if (PlayerPrefs.GetString ("Player").Length > 0) {
			if (File.Exists(Application.persistentDataPath +"/"+PlayerPrefs.GetString("Player")+ "mathGame.dat")
			    & PlayerPrefs.HasKey("Continue"))
			{
				PlayerPrefs.SetInt ("LastAvailableLevel", progMan.GetLastAvailableLevelFromFile());
				Application.LoadLevel (levelNumber);
			}
			else{
				//There is no saved file or we don't know if we want to continue or not
				//We load menu selection
				PlayerPrefs.SetInt ("LastAvailableLevel", progMan.GetLastAvailableLevelFromFile());
				Application.LoadLevel (levelNumber);
			}
		}
	}
	private bool AudioOn(){
		if(PlayerPrefs.GetInt("AudioEnabled")==1)
			return true;
		else 
			return false;
	}
}
