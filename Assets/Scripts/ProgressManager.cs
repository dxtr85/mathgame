using UnityEngine;
using System.Collections;

//You must include these namespaces
//to use BinaryFormatter
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ProgressManager {

	public int numberOfRepetitionsAfterFail = 3;

	public ProgressManager () {
		if (GameObject.FindGameObjectWithTag(Tags.gameController) != null)
			digCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DigitsController> ();
		gameState = new GameState ();
	}

	[Serializable]
	public struct PlayerStats{
		public int numberOfDaysAfterWhichRepeat;
		public uint numberOfCorrectAnswers;
	}

	private DigitsController digCtrl;

/*
 * - dzień i miesiąc gry (DateTime.DayOfYear) X - saved, Y - now, D - difference; D = (365+(Y-X))%365;
 * - level 
 * - doświadczenie
 * - prędkość gracza
 * - najwyższy dostępny poziom gry
 * - tablica ze statystykami - jakie działania do powtórzenia i za ile dni
 * int tab[11][11] = {-1,0,1,2,4,8} -init, potem aktualizacja (-1: jeśli działanie nie jest dostepne) - zapisywanie tak aby j 
 * było zawsz >= i tab[i][j]
 */
	[Serializable]
	public class GameState{
		public int dayOfYearWhenSaved;

		public int playerLevel;
		public int playerExperience, minLvlExp,nextLvlExp;
		public float playerSpeed;
		public bool[] enabledLevels;
		public int maxLevelEnabled;
		public PlayerStats[,] additionStats,substractionStats,multiplicationStats,divisionStats;

		public GameState(){
			/*
			dayOfYearWhenSaved = 0;
			playerLevel = 0;
			playerExperience = 0;
			minLvlExp = 0;
			nextLvlExp = 1000;
			playerSpeed = 0;
			maxLevelEnabled = 1;
			*/
			additionStats = new PlayerStats[11,11];
			substractionStats = new PlayerStats[11,11];
			multiplicationStats = new PlayerStats[11,11];
			divisionStats = new PlayerStats[11,11];

			for (int i=0; i<11; i++)
			for (int j=0; j<11; j++) {
				additionStats[i,j].numberOfCorrectAnswers = 0;
				additionStats[i,j].numberOfDaysAfterWhichRepeat = -1;
				substractionStats[i,j].numberOfCorrectAnswers = 0;
				substractionStats[i,j].numberOfDaysAfterWhichRepeat = -1;
				multiplicationStats[i,j].numberOfCorrectAnswers = 0;
				multiplicationStats[i,j].numberOfDaysAfterWhichRepeat = -1;
				divisionStats[i,j].numberOfCorrectAnswers = 0;
				divisionStats[i,j].numberOfDaysAfterWhichRepeat = -1;
			}

		}

		//Used when saving a game
		public void readCurrentGameState(){
			PlayerExperience pe = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<PlayerExperience> ();

			dayOfYearWhenSaved = DateTime.Now.DayOfYear ;
			playerLevel = pe.Level;
			playerExperience = pe.TotalExperience;
			minLvlExp = pe.MinLevelExp;
			nextLvlExp = pe.NextLevelExp;
			playerSpeed = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerMovement>().moveSpeed;
			maxLevelEnabled = getMaxLevelEnabled ();
			additionStats = calculatePlayerStatsFor ("+");
			substractionStats = calculatePlayerStatsFor ("-");
			multiplicationStats = calculatePlayerStatsFor ("*");
			divisionStats = calculatePlayerStatsFor ("/");
		}

		// After reading from file, check if we have another day, if so, decrease waiting time accordingly
		public void UpdateContents(){
			int difference, dayOfYearNow;
			dayOfYearNow = DateTime.Now.DayOfYear;
			difference = (365 + (dayOfYearNow - dayOfYearWhenSaved)) % 365;

			//For testing purposes
			//difference += 1;

			if (difference > 0) {
				additionStats = updatePlayerStatsFor("+",difference);
				substractionStats = updatePlayerStatsFor("-",difference);
				multiplicationStats = updatePlayerStatsFor("*",difference);
				divisionStats = updatePlayerStatsFor("/",difference);
			}
		}

		public bool isRepetitionAvailable(){
			int difference, dayOfYearNow;
			dayOfYearNow = DateTime.Now.DayOfYear;
			difference = (365 + (dayOfYearNow - dayOfYearWhenSaved)) % 365;
			//Debug.Log ("Difference: "+difference.ToString());

			// For testing only!!!
			//difference += 1;

			if (difference > 0) {
				for (int i=0; i<11; i++)
				for (int j=0; j<11; j++) {
					//Debug.Log ("Mult: "+multiplicationStats[i,j].numberOfDaysAfterWhichRepeat.ToString()+" "+multiplicationStats[i,j].numberOfDaysAfterWhichRepeat.ToString());
					if (((additionStats[i,j].numberOfDaysAfterWhichRepeat != -1) && (additionStats[i,j].numberOfDaysAfterWhichRepeat - difference <= 0)) ||
					    ((substractionStats[i,j].numberOfDaysAfterWhichRepeat != -1) && (substractionStats[i,j].numberOfDaysAfterWhichRepeat - difference <= 0)) ||
					    ((multiplicationStats[i,j].numberOfDaysAfterWhichRepeat != -1) && (multiplicationStats[i,j].numberOfDaysAfterWhichRepeat - difference <= 0)) ||
					    ((divisionStats[i,j].numberOfDaysAfterWhichRepeat != -1) && (divisionStats[i,j].numberOfDaysAfterWhichRepeat - difference <= 0)))
						return true;
				}			
			}
			return false;
		}

		/*
		 * Private functions section
		 * 
		 */


		//Read actual levels that are available to play
		private int getMaxLevelEnabled (){

			//Returns int pointing to last level available to player
			return PlayerPrefs.GetInt ("LastAvailableLevel");

		}

		private InGameOperationTable getCurrentTableFor(string operation){
			InGameOperationTable opTable;
			DigitsController digCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DigitsController> ();
			opTable = digCtrl.GetOperationTableFor(operation);

			return opTable;
		}

		private PlayerStats[,] getCurrentPlayerStatsFor(string operation){
		switch (operation) {
						case "+":
								return additionStats;
								break;
						case "-":
								return substractionStats;
								break;
						case "*":
								return multiplicationStats;
								break;
						case "/":
								return divisionStats;
								break;
						default:
								return divisionStats;
						}

		}

		private PlayerStats[,] calculatePlayerStatsFor(string operation){
			InGameOperationTable opTable = getCurrentTableFor (operation);
			PlayerStats[,] currentStats = getCurrentPlayerStatsFor (operation);
			PlayerStats[,] ps = new PlayerStats[11, 11];


			// Calculate days after which repetition should be performed for every equation
			// If number of repetitions of an operation in game is -1 then enter Max(-1, previous number of days until repetition for this eq)

			for (int i=0; i<11; i++)
				for (int j=0; j<11; j++) {
					ps[i,j].numberOfCorrectAnswers = opTable.GetNumberOfCorrectAnswers(i,j)+ currentStats[i,j].numberOfCorrectAnswers;
					int numberOfDaysUntilRepetition = opTable.GetNumberOfDaysUntilRepetition(i,j);
					if (numberOfDaysUntilRepetition==-1)
						ps[i,j].numberOfDaysAfterWhichRepeat = Math.Max(currentStats[i,j].numberOfDaysAfterWhichRepeat,-1);
					else{
						if (currentStats[i,j].numberOfDaysAfterWhichRepeat<0)
							ps[i,j].numberOfDaysAfterWhichRepeat = numberOfDaysUntilRepetition;
						else
							ps[i,j].numberOfDaysAfterWhichRepeat = Math.Min(numberOfDaysUntilRepetition, currentStats[i,j].numberOfDaysAfterWhichRepeat);
				}
				}
			return ps;
		}

		private PlayerStats[,] updatePlayerStatsFor(string operation, int difference){
			PlayerStats[,] currentPS = new PlayerStats[11,11];

			switch (operation) {
			case "+":
				currentPS = additionStats;
				break;
			case "-":
				currentPS = substractionStats;
				break;
			case "*":
				currentPS = multiplicationStats;
				break;
			case "/":
				currentPS = divisionStats;
				break;
			default:
				break;

			}
			
			for (int i=0; i<11; i++)
			for (int j=0; j<11; j++) {
				if(currentPS[i,j].numberOfDaysAfterWhichRepeat>0){
					currentPS[i,j].numberOfDaysAfterWhichRepeat = Mathf.Max(0, currentPS[i,j].numberOfDaysAfterWhichRepeat - difference);
				}
			}
			return currentPS;
		}

	}

	private GameState gameState;

	public void SaveScoresToFile()
	{
		//Read the data from current game
		gameState.readCurrentGameState ();
		//Get a binary formatter
		BinaryFormatter b = new BinaryFormatter();
		//Create a file
		FileStream f = File.Create(Application.persistentDataPath +"/"+PlayerPrefs.GetString("Player")+ "mathGame.dat");
		//Save the scores
		b.Serialize(f, gameState);
		f.Close();
	}
	
	public void LoadScoresFromFile()
	{
		PlayerExperience pe = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<PlayerExperience> ();

		/*FOR TESTING PURPOSES ONLY!!!
		if (File.Exists (Application.persistentDataPath + "/mathGame.dat")) 
			File.Delete(Application.persistentDataPath + "/mathGame.dat");
		//*/
		//If not blank then load it
		if (File.Exists (Application.persistentDataPath +"/"+PlayerPrefs.GetString("Player")+ "mathGame.dat")) {
						
						//Binary formatter for loading back
						BinaryFormatter b = new BinaryFormatter ();
						//Get the file
			FileStream f = File.Open (Application.persistentDataPath +"/"+PlayerPrefs.GetString("Player")+ "mathGame.dat", FileMode.Open);
						//Load back the data
						gameState = (GameState)b.Deserialize (f);
						
			//Debug.Log("Before update days until repetition 0+10: "+gameState.additionStats[0,10].numberOfDaysAfterWhichRepeat);
						//Update data contents if the day has changed
						
						gameState.UpdateContents ();
			//Debug.Log("After update days until repetition 0+10: "+gameState.additionStats[0,10].numberOfDaysAfterWhichRepeat);
						f.Close ();

						//Update player parameters here
						pe.Level = gameState.playerLevel;
						pe.NextLevelExp = gameState.nextLvlExp;
						pe.MinLevelExp = gameState.minLvlExp;
						pe.TotalExperience = gameState.playerExperience;
						GameObject.FindGameObjectWithTag (Tags.player).GetComponent<PlayerMovement> ().moveSpeed = gameState.playerSpeed;
						
						importLoadedDataIntoWorkingTables();

						//Enabled levels should be updated here, but for now we don't have such a parameter in GameController...

		} else {
			pe.Level 			= 0;
			pe.NextLevelExp 	= 1000;
			pe.MinLevelExp 		= 0;
			pe.TotalExperience  = 0;
			GameObject.FindGameObjectWithTag (Tags.player).GetComponent<PlayerMovement> ().moveSpeed = 15;
		}
	}

	public bool isRepetitionLevelAvailable(){
				if (File.Exists (Application.persistentDataPath + "/" + PlayerPrefs.GetString ("Player") + "mathGame.dat")) {
			
						//Binary formatter for loading back
						BinaryFormatter b = new BinaryFormatter ();
						//Get the file
						FileStream f = File.Open (Application.persistentDataPath + "/" + PlayerPrefs.GetString ("Player") + "mathGame.dat", FileMode.Open);

						gameState = (GameState)b.Deserialize (f);

						f.Close ();

						return gameState.isRepetitionAvailable ();
				}
		return false;
		}

	public int GetLastAvailableLevelFromFile(){
		if (File.Exists (Application.persistentDataPath + "/" + PlayerPrefs.GetString ("Player") + "mathGame.dat")) {
			
						//Binary formatter for loading back
						BinaryFormatter b = new BinaryFormatter ();
						//Get the file
						FileStream f = File.Open (Application.persistentDataPath + "/" + PlayerPrefs.GetString ("Player") + "mathGame.dat", FileMode.Open);
						//Load back the data
						gameState = (GameState)b.Deserialize (f);
						f.Close ();
						return gameState.maxLevelEnabled;
				} else
						return 1;
	}

	public void SetLastAvailableLevel(int levelToUnlock){
		if (levelToUnlock > PlayerPrefs.GetInt ("LastAvailableLevel")) {
			PlayerPrefs.SetInt ("LastAvailableLevel",levelToUnlock);
		}

	}

	private void importLoadedDataIntoWorkingTables(){
		if (File.Exists (Application.persistentDataPath+"/"+PlayerPrefs.GetString("Player")+ "mathGame.dat")) {
						// Addition
						InGameOperationTable table = new InGameOperationTable(0,0);
						table = intersectDataFromTables (gameState.additionStats, 
			                                 			 digCtrl.GetOperationTableFor ("+"));
						//Reset number of wrong answers
						table.resetNumberOfWrongAnswers();
						digCtrl.SetOperationTableFor ("+", table);
						table = null;

						// Substraction
						table = intersectDataFromTables (gameState.substractionStats, digCtrl.GetOperationTableFor ("-"));
						//Reset number of wrong answers
						table.resetNumberOfWrongAnswers();
						digCtrl.SetOperationTableFor ("-", table);
						table = null;

						// Multiplication
						table = intersectDataFromTables (gameState.multiplicationStats, digCtrl.GetOperationTableFor ("*"));
						//Reset number of wrong answers
						table.resetNumberOfWrongAnswers();
						digCtrl.SetOperationTableFor ("*", table);
						table = null;

						// Division
						table = intersectDataFromTables (gameState.divisionStats, digCtrl.GetOperationTableFor ("/"));
						//Reset number of wrong answers
						table.resetNumberOfWrongAnswers();
						digCtrl.SetOperationTableFor ("/", table);
						table = null;
				}
	}

	/*	
	 * 
	 * 	Private functions
	 * 
	 * 
	 */

	private InGameOperationTable intersectDataFromTables(PlayerStats[,] ps, InGameOperationTable table){

		for (int i=0; i<11; i++)
		for (int j=0; j<11; j++) {
			if (ps[i,j].numberOfDaysAfterWhichRepeat == 0){
				table.updateNumberOfRepetitionsFor( i, j, numberOfRepetitionsAfterFail);
			}
		}
		return table;
	}
	
}
