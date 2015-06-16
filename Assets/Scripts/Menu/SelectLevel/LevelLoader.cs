using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {
	public int levelNumber;
	private GUIText guiText;

		void Start () {
		guiText = GetComponent<GUIText> ();
		if (PlayerPrefs.GetInt ("LastAvailableLevel") >= levelNumber)
						guiText.material.color = Color.green;
				else
						guiText.material.color = Color.grey;
		}

	
	// Update is called once per frame
	void Update() {
		int i = 0;
		while (i < Input.touchCount) {
			if (guiText.HitTest(Input.GetTouch(i).position) && Input.GetTouch(i).phase == TouchPhase.Stationary) {
				OnMouseUp();
			}
			++i;
		}
		
	}
		void OnMouseEnter(){
		if (PlayerPrefs.GetInt ("LastAvailableLevel") >= levelNumber)
			GetComponent<GUIText>().material.color = Color.white;
		}
		
		void OnMouseExit(){
		if (PlayerPrefs.GetInt ("LastAvailableLevel") >= levelNumber)
			GetComponent<GUIText>().material.color = Color.green;
		}
		
		void OnMouseUp(){			
		if (PlayerPrefs.GetInt ("LastAvailableLevel") >= levelNumber)
			Application.LoadLevel (levelNumber);
		}
	}
