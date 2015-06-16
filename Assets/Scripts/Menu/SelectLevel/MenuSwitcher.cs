using UnityEngine;
using System.Collections;

public class MenuSwitcher : MonoBehaviour {
	public int levelNumber;
	private GUIText guiText;
	
	// Use this for initialization
	void Start () {
		guiText = GetComponent<GUIText> ();
		guiText.material.color = Color.green;	
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
			GetComponent<GUIText>().material.color = Color.white;
	}
	
	void OnMouseExit(){
			GetComponent<GUIText>().material.color = Color.green;
	}
	
	void OnMouseUp(){			
			Application.LoadLevel (levelNumber);
	}

}
