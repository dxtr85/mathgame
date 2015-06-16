using UnityEngine;
using System.Collections.Generic;
using System.Timers;
using GoogleMobileAds.Api;

public class DigitsController : MonoBehaviour {
	public GameObject[] digitsAvailableToPlayer;
	public GameObject[] symbolsAvailableToPlayer;
	public GameObject[] respawnPts = new GameObject[16];
	public int levelToUnlock;
	public int addToExcl,substractToExcl,multiplicateToExcl,divideToExcl;
	public int addFrom,substractFrom,multiplicateFrom,divideFrom;
	public int repetitionsAddition, repetitionsSubstraction, repetitionsMultiplication, repetitionsDivision;
	public int maxNumberOfDigitsInGame;
	public int nextLevel;

	private ProgressManager progMgr;
	private List<GameObject> digitsInGame;
	private InGameOperationTable inGameMultiplicationTable,inGameAdditionTable,inGameDivisionTable,inGameSubstractionTable;
	private GameObject player, digit1,action,digit2;
	private string receivedAnswer,equation;
	//private GUIText displayedText;
	private CameraController camCtrl;
	private GUIController guiCtrl;
	private PlayerMovement playerMovement;
	private ShootingController shootingCtrl;
	private MouseController mouseCtrl;
	private TutorialController tutCtrl;
	private PlayerExperience playerExp;
	private int totalEquationsLeft;
	private bool levelComplete, levelCompleteAudioPlayed;

	private AudioSource correct,wrong, levelCompleteAudio;
	private AudioSource[] music;
	private int songIndex;

	//Google Ads
	private BannerView BottomBannerInGame;
	//private InterstitialAd interstitial;

	// Use this for initialization
	void Start () {

		//Initialize tables with data from Inspector fields
		//Debug.Log ("Add from:" + addFrom + " add to:" + addToExcl+" rep:"+repetitionsAddition);
		inGameAdditionTable 		= new InGameOperationTable (addFrom, addToExcl,false,repetitionsAddition);
		inGameSubstractionTable		= new InGameOperationTable (substractFrom, substractToExcl,false,repetitionsSubstraction);
		inGameMultiplicationTable 	= new InGameOperationTable (multiplicateFrom,multiplicateToExcl,false,repetitionsMultiplication);
		inGameDivisionTable 		= new InGameOperationTable (divideFrom, divideToExcl,true,repetitionsDivision);

		//Level not yet complete
		levelComplete = false;
		levelCompleteAudioPlayed = false;

		//Find a player
		player = GameObject.FindGameObjectWithTag (Tags.player);

		//Make sure there can be some digits in game
		if (maxNumberOfDigitsInGame==null)
			maxNumberOfDigitsInGame = 4;

		//Initialize Audio
		FillAudioSourcesTables();

		//Cleanup answer
		receivedAnswer = "";

		//Initialize private variables
		camCtrl = Camera.main.GetComponent<CameraController>();
		guiCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GUIController> ();
		playerMovement = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<PlayerMovement> ();
		shootingCtrl = GameObject.FindGameObjectWithTag(Tags.spawnPoint).GetComponent<ShootingController> ();
		mouseCtrl = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<MouseController> ();
		tutCtrl = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<TutorialController> ();
		playerExp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<PlayerExperience> ();

		//It has to be after setting number of total equations left (work as designed)
		InitializeAreaWithDigits (maxNumberOfDigitsInGame,digitsAvailableToPlayer);

		digitsInGame = new List<GameObject> ();
		foreach (GameObject digit in GameObject.FindGameObjectsWithTag (Tags.digit)) {
			digitsInGame.Add (digit);
		}

		//Debug.Log("Not a tutorial, initializing ProgressManager...");
		//Create instance of ProgressManager class
		progMgr = new ProgressManager();
	
		//Debug.Log ("Before update digCtrl: " + inGameAdditionTable.GetNumberOfRepetitionsFor(0, 10));

		//Load data from file into ingame objects
		progMgr.LoadScoresFromFile();

		//Debug.Log ("After update digCtrl: " + inGameAdditionTable.GetNumberOfRepetitionsFor(0, 10));
		updateNumberOfTotalEquationsLeft ();


	
		//Initialize Google Ads
		AdSize adSize = new AdSize(320, 100);
		BottomBannerInGame = new BannerView(
			"ca-app-pub-8079075054121466/1696375434", adSize, AdPosition.Bottom);
		//InterstitialAd interstitial = new InterstitialAd("ca-app-pub-8079075054121466/4074432231");

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();

		//Above is normal, below is for testing
		//AdRequest request = new AdRequest.Builder()
		//	.AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
		//		.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")  // Test Device 1.
		//		.Build();

		// Load the banner with the request.
		BottomBannerInGame.LoadAd(request);
		//interstitial.LoadAd(request);
		BottomBannerInGame.Show ();


	}
	
	// Update is called once per frame
	void Update () {
		DisplayInfoAboutItemsLeft ();
		UpdateMusicInBackground ();

		foreach(GameObject digit in GameObject.FindGameObjectsWithTag(Tags.digit)){
			digit.transform.LookAt(player.transform);
		}

		levelComplete = (totalEquationsLeft == 0 && GameObject.FindGameObjectsWithTag(Tags.tutorialItem).Length == 0);

		if (levelComplete)
		{
			//Debug.Log ("That's it, I'm done here, SAVING...!");
			if (progMgr!=null && tutCtrl==null){
				progMgr.SetLastAvailableLevel(levelToUnlock);
				progMgr.SaveScoresToFile();
			}

			OnLevelComplete();
		}

	}

	public void CreateEquation(GameObject dig1,GameObject act,GameObject dig2){
		digit1 = dig1;
		action = act;
		digit2 = dig2;

		if (action.GetComponent<SymbolContext> ().value != "tut")
						equation = dig1.GetComponent<DigitContext> ().value + " " + act.GetComponent<SymbolContext> ().value + " " + dig2.GetComponent<DigitContext> ().value + " = ";
		else {
			switch (digit1.GetComponent<DigitContext>().value){
			case 0:
				equation = "0 = ";
				break;
			case 1:
				equation = "1 = ";
				break;
			case 2:
				equation = "2 = ";
				break;
			case 3:
				equation = "3 = ";
				break;
			case 4:
				equation = "4 = ";
				break;
			case 5:
				equation = "5 = ";
				break;
			case 6:
				equation = "6 = ";
				break;
			case 7:
				equation = "7 = ";
				break;
			case 8:
				equation = "8 = ";
				break;
			case 9:
				equation = "9 = ";
				break;
			case 10:
				equation = "r = ";
				break;
			case 11:
				equation = "123 = ";
				receivedAnswer = "1234";
				break;
			case 12:
				equation = "1 = ";
				receivedAnswer = "1";
				break;
			case 13:
				equation = "- = ";
				break;
			default:
				break;
			}
		}
		guiCtrl.SetGuiText (equation+receivedAnswer, guiCtrl.CounterMax, Color.blue);
	}
	public void AddInGameDigit(GameObject digit){
		digitsInGame.Add (digit);
	}
	
	public void addSymbol(string symbol){

		switch (symbol){
		case "DOT":
			if (receivedAnswer.Length>0){
				if (isProvidedAnswerRightResult()){
					playerMovement.SetHappy();
					if (AudioOn ()) correct.Play ();
				}else{
					playerMovement.SetSad();
					if (AudioOn ()) wrong.Play ();
				}
			}
			break;
		case "BACKSPACE":
			removeLastSymbolFromReceivedAnswer();
			break;
		default:
			receivedAnswer+=symbol;
			guiCtrl.SetGuiText (equation+receivedAnswer, guiCtrl.CounterMax, Color.blue);
			break;
		}

	}

	public InGameOperationTable GetOperationTableFor(string operation){
		switch (operation) {
				case "+":
						return inGameAdditionTable;
						break;
				case "-":
						return inGameSubstractionTable;
						break;
				case "*":
						return inGameMultiplicationTable;
						break;
				case "/":
						return inGameDivisionTable;
						break;
				default:
						return null;
						break;
		}

	}

	public void SetOperationTableFor(string operation, InGameOperationTable table){
		switch (operation) {
		case "+":
			inGameAdditionTable = table;
			break;
		case "-":
			inGameSubstractionTable = table;
			break;
		case "*":
			inGameMultiplicationTable = table;
			break;
		case "/":
			inGameDivisionTable  = table;
			break;
		default:
			break;
		}
		
	}

	private bool isProvidedAnswerRightResult(){
	
	if (action.GetComponent<SymbolContext> ().value != "tut") {
						int d1, d2, pointsToAdd, rightResult = 0, rightReminderForDivision = 0;
						string rightResultString = "";
						string operation = "";
						bool result = false;
						d1 = digit1.GetComponent<DigitContext> ().value;
						d2 = digit2.GetComponent<DigitContext> ().value;
						operation = action.GetComponent<SymbolContext> ().value;
				
						switch (operation) {
						case "+":
								rightResult = d1 + d2;
								break;
						case "-":
								rightResult = d1 - d2;
								break;
						case "*":
								rightResult = d1 * d2;
								break;
						case "/":
								rightResult = (int)d1 / d2;
								rightReminderForDivision = d1 % d2;
								break;
						default:
								break;
						}
						rightResultString = rightResult.ToString ();

						if (rightReminderForDivision != 0) {
								rightResultString += "r" + rightReminderForDivision.ToString ();
								rightReminderForDivision = 0;
						}
						rightResult = 0;

						result = string.Equals (receivedAnswer, rightResultString);

						pointsToAdd = (int)(guiCtrl.CounterMax - guiCtrl.Counter);//(int)((guiCtrl.CounterMax - guiCtrl.Counter)/guiCtrl.CounterMax * maxScore);
						pointsToAdd+= Mathf.CeilToInt(shootingCtrl.TimeAfterShotFired);
			
						if (result) {
								guiCtrl.SetGuiText (equation + receivedAnswer, 2.0f, Color.green);
								//Debug.Log ("Right answer: " + receivedAnswer + " adding: " + pointsToAdd);
								playerExp.AddExperiencePionts (pointsToAdd);
						} else {
								guiCtrl.SetGuiText ("Correct: " + equation + rightResultString, 3.0f, Color.red);
								//Debug.Log ("Wrong answer: " + receivedAnswer + ", right is: " + rightResultString);
								playerExp.SubstractExperiencePionts (Mathf.FloorToInt ((float)0.5 * pointsToAdd));
						}
						updateAccordingOperationTable(d1,operation,d2,result);
						updateNumberOfTotalEquationsLeft ();
						receivedAnswer = "";
						rightResultString = "";
						equation = "";
			
						removeEquation ();
						SelectAndAddDigitToScene ();
						playerMovement.WalkingEnabled = true;
						shootingCtrl.ShootingEnabled = true;
						mouseCtrl.gestureCapturingEnabled = false;
						guiCtrl.DestroyCounter ();
		
						return result;
	} 
	else 
		return IsTutorialItemSolvedCorrectly ();
	}

	private bool IsTutorialItemSolvedCorrectly(){
		string rightResult = equation.Substring(0,equation.Length-3);	
		bool result = string.Equals(receivedAnswer, rightResult);
		
		if (result) {
			guiCtrl.SetGuiText ("Great! Keep it up! ", 2.0f, Color.green);
			//Debug.Log ("Right answer: " + receivedAnswer +"!");
		} 
		else {
			guiCtrl.SetGuiText ("No, sorry, try again.", 3.0f, Color.red);
			//Debug.Log ("Wrong answer: " + receivedAnswer + ", right is: " + rightResult);
		}
		receivedAnswer = "";
		
		removeTutItem ();
		playerMovement.WalkingEnabled = true;
		shootingCtrl.ShootingEnabled = true;
		mouseCtrl.gestureCapturingEnabled = false;
		tutCtrl.UpdateItem(result);
		return result;
	}

	private void removeLastSymbolFromReceivedAnswer(){
		if (receivedAnswer.Length > 0)
						receivedAnswer = receivedAnswer.Substring (0, receivedAnswer.Length - 1);
		guiCtrl.SetGuiText (equation+receivedAnswer, guiCtrl.CounterMax, Color.blue);
		}

	private void removeEquation(){
		camCtrl.SetNewTargetTransform(player.transform);
		removeInGameDigit (digit1);
		GameObject.Destroy (action);
		removeInGameDigit (digit2);
		}

	private void removeTutItem(){
		camCtrl.SetNewTargetTransform(player.transform);
		GameObject.Destroy (digit1);
		}

	public void removeInGameDigit(GameObject digit){
		digitsInGame.Remove (digit);
		GameObject.Destroy (digit);
	}

	public void ForceVerification(){
		mouseCtrl.ResetGestureCapturing ();
		if (isProvidedAnswerRightResult ()) {
			playerMovement.SetHappy();
			//	correct[Mathf.FloorToInt(Random.Range(0,8))].Play();
			if (AudioOn ())
				correct.Play ();
		} else {
			playerMovement.SetSad();
			//	wrong[Mathf.FloorToInt(Random.Range(0,3))].Play();
			if (AudioOn ())
				wrong.Play ();
		}
		//playerMovement.ResetFeelings();
	}

	public int returnIndexOfDigitBestSuitableForValueOf(int val,string operation){
		int index = -1;
		switch (operation) {
		case "+":	
			index = inGameAdditionTable.returnBestDigitForValueOf(val);
			break;
		case "-":
			index = inGameSubstractionTable.returnBestDigitForValueOf(val);
			break;
		case "*":
			index = inGameMultiplicationTable.returnBestDigitForValueOf(val);
			break;
		case "/":
			index = inGameDivisionTable.returnBestDigitForValueOf(val);
			break;
		default:
			break;

		}
		return index;
	}
	public GameObject returnOperationBestSuitableForDigit(int digit){
		GameObject operation = symbolsAvailableToPlayer[0]; // object responsible for addition
		if(inGameSubstractionTable.GetSumOfAllRemainingEquationsForDigit(digit) > inGameAdditionTable.GetSumOfAllRemainingEquationsForDigit(digit)){
			operation = symbolsAvailableToPlayer[1];
		}else if (inGameMultiplicationTable.GetSumOfAllRemainingEquationsForDigit(digit) > inGameSubstractionTable.GetSumOfAllRemainingEquationsForDigit(digit)){
			operation = symbolsAvailableToPlayer[2];
		} else if(inGameDivisionTable.GetSumOfAllRemainingEquationsForDigit(digit) > inGameMultiplicationTable.GetSumOfAllRemainingEquationsForDigit(digit)){
			operation = symbolsAvailableToPlayer[3];
		}
		return operation;
	}

	public void ReinitializeAreaWithTutItems(GameObject[] items){
		
		foreach(GameObject go in respawnPts)
			GameObject.Destroy(go.GetComponent<RespawnController> ().content);

		InitializeAreaWithDigits (maxNumberOfDigitsInGame, items,true);
	}

	/*
	 * public void SaveScore(){
		if (progMgr!=null){
			progMgr.SaveScoresToFile();
		}
	}
	*/

	public void AdsKill(){
		BottomBannerInGame.Hide ();
		BottomBannerInGame.Destroy ();
	}
	public void AdsHide(){
		BottomBannerInGame.Hide ();
	}
	public void AdsShow(){
		BottomBannerInGame.Show();
	}
	
	private int getDigitWithMostRemainingEquations(){
		int maxRemainingEq = 0;
		int digit = -1;

		while (digit==-1) {
			int i = Mathf.FloorToInt(Random.Range(0,11));	
			int tempSum = inGameAdditionTable.GetSumOfAllRemainingEquationsForDigit(i)
				+ inGameSubstractionTable.GetSumOfAllRemainingEquationsForDigit(i)
					+ inGameMultiplicationTable.GetSumOfAllRemainingEquationsForDigit(i)
					+ inGameDivisionTable.GetSumOfAllRemainingEquationsForDigit(i);
			digit=(tempSum>0)?i:-1;
		}

		/*
		 * 
		 * Old solution, not randomized...
		 * 
		 * 
		for (int i=0; i<11; i++) {
			int tempSum = inGameAdditionTable.GetSumOfAllRemainingEquationsForDigit(i)
				+ inGameSubstractionTable.GetSumOfAllRemainingEquationsForDigit(i)
					+ inGameMultiplicationTable.GetSumOfAllRemainingEquationsForDigit(i)
					+ inGameDivisionTable.GetSumOfAllRemainingEquationsForDigit(i);
			if(tempSum>maxRemainingEq){
				maxRemainingEq = tempSum;
				digit = i;
			}
					
		}

		*/
		return digit;
	}

	private void updateNumberOfTotalEquationsLeft (){
		totalEquationsLeft = inGameAdditionTable.GetSumOfAllRemainingEquations ()
						+ inGameSubstractionTable.GetSumOfAllRemainingEquations ()
						+ inGameMultiplicationTable.GetSumOfAllRemainingEquations ()
						+ inGameDivisionTable.GetSumOfAllRemainingEquations ();
	}

	private void updateAccordingOperationTable(int digit1, string operation,int digit2, bool correctlySolved){
		if (correctlySolved) {
			switch(operation){
			case "+":	
				inGameAdditionTable.correctlySolved(digit1,digit2);
				break;
			case "-":
				inGameSubstractionTable.correctlySolved(digit1,digit2);
				break;
			case "*":
				inGameMultiplicationTable.correctlySolved(digit1,digit2);
				break;
			case "/":
				inGameDivisionTable.correctlySolved(digit1,digit2);
				break;
			default:
				break;
			}
		} else {
			switch(operation){
			case "+":	
				inGameAdditionTable.incorrectlySolved(digit1,digit2);
				break;
			case "-":
				inGameSubstractionTable.incorrectlySolved(digit1,digit2);
				break;
			case "*":
				inGameMultiplicationTable.incorrectlySolved(digit1,digit2);
				break;
			case "/":
				inGameDivisionTable.incorrectlySolved(digit1,digit2);
				break;
			default:
				break;
			}
		}
	}

	private void SelectAndAddDigitToScene (){
		// There are always 2 instances of GameObjects with Tags.digit as tag, - those two are used internally by BulletController
		if ((totalEquationsLeft > 0) && (GameObject.FindGameObjectsWithTag(Tags.digit).Length < 2+Mathf.Min(totalEquationsLeft, maxNumberOfDigitsInGame))) {

		GameObject itemToAdd = (GameObject)Instantiate(digitsAvailableToPlayer[getDigitWithMostRemainingEquations()]);
		GameObject placeToAdd = null;
			
		while (placeToAdd ==null) {
				
			int ind = Mathf.FloorToInt (Random.Range (0, 16));
			if (respawnPts [ind].GetComponent<RespawnController> ().Content == null){
				placeToAdd = respawnPts [ind];
				if (Vector3.Distance( placeToAdd.transform.position,player.transform.position)<50.0F)
					placeToAdd = null;
			}
		}
		//Debug.Log ("Adding a tutItem into play.");
		placeToAdd.GetComponent<RespawnController> ().Content = itemToAdd;
		itemToAdd.transform.position = placeToAdd.transform.position;
		}
	}


	private void InitializeAreaWithDigits(int max, GameObject[] sourceOfDigits,bool isTutorial = false){
		int localMax = Mathf.Min (max, sourceOfDigits.Length);
		GameObject instance;

		if (isTutorial) {
						for (int i=0; i<localMax; i++) {
								instance = (GameObject)Instantiate (sourceOfDigits [12]);
								respawnPts [i].GetComponent<RespawnController> ().content = instance;
								instance.transform.position = respawnPts [i].transform.position;
						}
				}
		else{
			for (int i=0; i<localMax; i++) {
				instance = (GameObject)Instantiate (sourceOfDigits [i]);
				respawnPts [i].GetComponent<RespawnController> ().content = instance;
				instance.transform.position = respawnPts[i].transform.position;
			}
		}
	}

	private void DisplayInfoAboutItemsLeft(){
		int numberOfItemsLeft = totalEquationsLeft;
		if (GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<TutorialController> () != null)
			numberOfItemsLeft = Mathf.Max(GameObject.FindGameObjectsWithTag(Tags.tutorialItem).Length, GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<TutorialController> ().ItemsLeft);
		guiCtrl.SetGuiTextInfo ("About " + numberOfItemsLeft.ToString()+" left");

	}

	private void FillAudioSourcesTables(){
		music = new AudioSource[7];
		songIndex = Mathf.FloorToInt (Random.Range (0, music.Length));
		music[0]= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("Music").transform.Find ("Song01").audio;
		music[1]= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("Music").transform.Find ("Song02").audio;
		music[2]= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("Music").transform.Find ("Song03").audio;
		music[3]= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("Music").transform.Find ("Song04").audio;
		music[4]= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("Music").transform.Find ("Song05").audio;
		music[5]= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("Music").transform.Find ("Song06").audio;
		music[6]= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("Music").transform.Find ("Song07").audio;

		correct= GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("correct1").audio;

		wrong = GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("wrong1").audio;

		levelCompleteAudio = GameObject.FindGameObjectWithTag (Tags.player).transform.Find ("Audio").transform.Find ("levelComplete").audio;
		}

	private void OnLevelComplete(){
		//Google Ads destroy
		BottomBannerInGame.Hide() ;
		BottomBannerInGame.Destroy ();

		//if (interstitial.IsLoaded()) {
		//	interstitial.Show();
		//}
		//Show AddBuddiz ad
		//AdBuddizBinding.ShowAd();

		guiCtrl.SetGuiText ("Level complete!",5.0f, Color.green);
		if (!levelCompleteAudio.isPlaying && !levelCompleteAudioPlayed) {
						if (AudioOn ()) levelCompleteAudio.Play ();
						levelCompleteAudioPlayed = true;	
						music[songIndex].Stop();
		}

		if(!levelCompleteAudio.isPlaying && levelCompleteAudioPlayed)
			Application.LoadLevel(nextLevel);
	}

	private void UpdateMusicInBackground (){
		if (AudioOn() && !(music [songIndex].isPlaying) &&!levelComplete) {
			songIndex = Mathf.FloorToInt (Random.Range (0, music.Length));
			music[songIndex].Play();
		} else if (!AudioOn())
			music[songIndex].Stop();
	
	}
	private bool AudioOn(){
		if(PlayerPrefs.GetInt("AudioEnabled")==1)
			return true;
		else 
			return false;
	}
	
}
