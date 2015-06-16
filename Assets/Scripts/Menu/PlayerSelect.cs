using UnityEngine;
using System.Collections;
using System.IO;

public class PlayerSelect : MonoBehaviour {
	private GUIText guiText;

	// Use this for initialization
	void Start () {
		guiText = GetComponent<GUIText> ();
		guiText.material.color =  Color.green;	
		PlayerPrefs.SetString ("Player", "");
	}
	
	// Update is called once per frame
	void Update() {
		foreach(Touch touch in Input.touches)
		if (guiText.HitTest(touch.position) && touch.phase == TouchPhase.Stationary) {
			OnMouseUp();
		}
	
}
	
	void OnMouseEnter(){
		GetComponent<GUIText>().material.color = Color.white;
	}
	
	void OnMouseExit(){
		GetComponent<GUIText>().material.color = Color.green;
	}
	void OnMouseUp(){
		if (PlayerPrefs.GetString ("Player").Length > 0) {
			if (File.Exists(Application.persistentDataPath +"/"+PlayerPrefs.GetString("Player")+ "mathGame.dat")
			    & PlayerPrefs.HasKey("Continue"))
			{
				// Are you sure you want to erase saved data?
				if (PlayerPrefs.GetInt("Continue")==0)
						Application.LoadLevel (19);
				// We continue playing, no data removal
				else if (PlayerPrefs.GetInt("Continue")==1)
					Application.LoadLevel (2);
			}
			else
				//There is no saved file or we don't know if we want to continue or not
				//We load menu selection, and act as we're continuing play
				PlayerPrefs.SetInt("Continue",1);
				Application.LoadLevel (2);

		}
	}
}
