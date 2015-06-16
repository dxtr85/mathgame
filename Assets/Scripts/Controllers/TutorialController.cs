using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialController : MonoBehaviour {

	public int numberOfConsecutiveCorrectAnswersRequired;
	public int maximumNumberOfTutItemsInGame;
	public GameObject[] tutItems;// = new GameObject[14];
	//private GameObject[] respawnPts = new GameObject[16];

	private GameObject[] respawnPts;
	private List<GameObject> tutItemsInGame;
	private bool tutFinished;
	private GameObject player;
	private GameObject itemUnderEvaluation = null;
	private GUIController guiCtrl;
	private DigitsController digCtrl;

	public GameObject ItemUnderEvaluation{
		get{ return itemUnderEvaluation;}
		set{itemUnderEvaluation = value;}
	}

	public int ItemsLeft{get{return remainingSum;}}

	private struct LearningArray{
		public GameObject tutItem{ get; set;}
		public int remainingInstantiations{ get; set;}
		public float upperRandLimit{get;set;}
	}

	private LearningArray[] array;
	private int remainingSum;

	// Use this for initialization
	void Start () {
		tutFinished = false;
		player = GameObject.FindGameObjectWithTag (Tags.player);
		remainingSum = tutItems.Length * numberOfConsecutiveCorrectAnswersRequired;
		guiCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GUIController> ();
		digCtrl  = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DigitsController> ();
		array 	 =  new LearningArray[tutItems.Length];

		RecalculateAllLimits();
		respawnPts = digCtrl.respawnPts;
		digCtrl.ReinitializeAreaWithTutItems (tutItems);
		
		for (int i=0;i<tutItems.Length;i++) {
			array[i].tutItem = tutItems[i];
			array[i].remainingInstantiations = numberOfConsecutiveCorrectAnswersRequired;
		}
		guiCtrl.SetGuiTextInfo ("Items left: " + remainingSum.ToString ());

	}

	public void UpdateItem(bool correctlySolved){
		int remaining;
		if (itemUnderEvaluation) {
			int i = itemUnderEvaluation.GetComponent<DigitContext>().value;
			if (correctlySolved) {
				if (array [i].remainingInstantiations > 0){
					array [i].remainingInstantiations -= 1;
					DecreaseSumOfRemainingInstantiations ();
				}
			} else {
				IncreaseSumOfRemainingInstantiations (numberOfConsecutiveCorrectAnswersRequired - array [i].remainingInstantiations);
				array [i].remainingInstantiations = numberOfConsecutiveCorrectAnswersRequired;
			}

			GameObject.Destroy (itemUnderEvaluation);
			RecalculateAllLimits ();
			if (GameObject.FindGameObjectsWithTag(Tags.tutorialItem).Length <= Mathf.Min(maximumNumberOfTutItemsInGame,remainingSum))
				SelectAndAddTutItemToScene ();
		}
		if (remainingSum == 0)
						tutFinished = true;
		itemUnderEvaluation = null;
		remaining = Mathf.Max (remainingSum, GameObject.FindGameObjectsWithTag (Tags.tutorialItem).Length);
		guiCtrl.SetGuiTextInfo ("Items left: " + remaining.ToString ());
	}
	// Update is called once per frame
	void Update () {
		LookAtPlayer ();
		//if (tutFinished)
		//	LoadLevel ();
	
	}

	public void LoadLevel (){
		Application.LoadLevel (1);
	}

	private float CalculateLimitFor(int idx) {
		int j=0;
		for(int i=0;i<=idx;i++)
			j+=array[i].remainingInstantiations;
		return (float)j/remainingSum;

	}

	private void RecalculateAllLimits() {
		if (remainingSum > 0) {
			for (int i=0; i<array.Length; i++)
				array [i].upperRandLimit = CalculateLimitFor (i);
		}
	}

	private void DecreaseSumOfRemainingInstantiations(){
		if (remainingSum > 0) {
			remainingSum -= 1;
		} else {
			tutFinished = true;
		}
		//Debug.Log ("Next scene: " + tutFinished.ToString ());
	}

	private void IncreaseSumOfRemainingInstantiations(int val){
		remainingSum+=val;
	}

	private void SelectAndAddTutItemToScene (){
		if (remainingSum > 0) {
			GameObject itemToAdd = RandomlySelectNextTutItem ();
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

	private GameObject RandomlySelectNextTutItem(){
		GameObject go = null;

		while (go == null) {
			float val = Random.Range (0.0f, 1.0f);
			//Debug.Log("Generated value = "+val.ToString());
			for (int i=0; i<array.Length; i++) {
				//Debug.Log("Limit is "+array[i].upperRandLimit.ToString());
				if (val <= array [i].upperRandLimit && array [i].remainingInstantiations > 0){
					go = array [i].tutItem;
					break;
				}
			}
		}
		go = (GameObject)GameObject.Instantiate (go);
		go.gameObject.tag = Tags.tutorialItem;
		return go;
	}
	private void InitializeAreaWithTutItems(int max){
		for (int i=0; i<max; i++)
			SelectAndAddTutItemToScene ();
	}

	private void LookAtPlayer(){
		foreach(GameObject go in GameObject.FindGameObjectsWithTag(Tags.tutorialItem))
			go.transform.LookAt(Camera.main.transform);
	}
}

