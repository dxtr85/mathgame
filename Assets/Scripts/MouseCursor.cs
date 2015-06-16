using UnityEngine;
using System.Collections;

public class MouseCursor : MonoBehaviour {
	public Texture2D cursorImage;
	private int cursorWidth = 32;
	private int cursorHeight = 32;
	
	void Start()
	{
		Screen.showCursor = false;
	}
	
	
	void OnGUI()
	{
		GUI.depth = 0;
		if (Input.mousePresent) 
		{
			Screen.showCursor = false;
			GUI.DrawTexture (new Rect (Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursorWidth, cursorHeight), cursorImage);
		}
	}
}
