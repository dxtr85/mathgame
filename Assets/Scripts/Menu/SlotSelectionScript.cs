using UnityEngine;
using System.Collections;
using System.IO;

public class SlotSelectionScript : MonoBehaviour {
	public string slotName;
	private ProgressManager progMan;
	private GUIText guiText;

	Color currentColor;
	// Use this for initialization
	void Start () {
		//When we entered this menu by clicking "New Game"
		if (PlayerPrefs.GetInt("Continue")==0){
			if (File.Exists(Application.persistentDataPath +"/"+slotName+ "mathGame.dat"))
			    currentColor = Color.red;
			else
			    currentColor = Color.green;
		}
		else if (PlayerPrefs.GetInt("Continue")==1){
				if (File.Exists(Application.persistentDataPath +"/"+slotName+ "mathGame.dat"))
				    currentColor = Color.green;
				    else
				    currentColor = Color.gray;

		}

		//currentColor = Color.green;
		progMan = new ProgressManager ();
		guiText = GetComponent<GUIText> ();
		guiText.material.color = currentColor;	
	}
	
	// Update is called once per frame
	void Update() {
		foreach(Touch touch in Input.touches)
			if (guiText.HitTest(touch.position) && touch.phase == TouchPhase.Stationary) {
				OnMouseUp();
			}

	}

	void OnMouseEnter(){
		if (currentColor!=Color.gray)
		GetComponent<GUIText>().material.color = Color.white;
	}
	
	void OnMouseExit(){
		GetComponent<GUIText>().material.color = currentColor;
	}

	void OnMouseUp(){
		//foreach (GUIText gt in GameObject.FindObjectsOfType<GUIText>())
		//	gt.material.color = Color.green;
		//currentColor = Color.white;
		//GetComponent<GUIText>().material.color = currentColor;
		//Debug.Log ("Continue = "+PlayerPrefs.GetInt("Continue").ToString());
		//Debug.Log ("Color = " + GetComponent<GUIText>().material.color.ToString ());

		if(currentColor!=Color.gray){
		//Debug.Log ("Dupa ");
		PlayerPrefs.SetString ("Player", slotName);

		if (PlayerPrefs.GetString ("Player").Length > 0) {
			if (File.Exists(Application.persistentDataPath +"/"+PlayerPrefs.GetString("Player")+ "mathGame.dat")
			    & PlayerPrefs.HasKey("Continue"))
			{
				// Are you sure you want to erase saved data?
				if (PlayerPrefs.GetInt("Continue")==0)
					Application.LoadLevel (19);
				// We continue playing, no data removal
				else if (PlayerPrefs.GetInt("Continue")==1){
					PlayerPrefs.SetInt ("LastAvailableLevel", progMan.GetLastAvailableLevelFromFile());
					Application.LoadLevel (20);
				}
			}
			else{
				//There is no saved file or we don't know if we want to continue or not
				//We load menu selection
				PlayerPrefs.SetInt ("LastAvailableLevel", progMan.GetLastAvailableLevelFromFile());
				Application.LoadLevel (20);
			}
		}
		}
	}
}
