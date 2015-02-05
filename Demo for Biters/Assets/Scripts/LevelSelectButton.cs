using UnityEngine;
using System.Collections;

public class LevelSelectButton : MonoBehaviour {

	private bool levelMenu = false;


	// Use this for initialization
	public void OnClick () {

		levelMenu = true; 
	
	} // end OnClick 

	void OnGUI() { 

		if (levelMenu) { 
			
			GUI.Window(0, new Rect(0, 0, Screen.width, Screen.height), ShowLevel, "Level Select Menu");
			
		} // end if statement 

	} // end OnGUI

	void ShowLevel (int windowID) { 

		// level 1
		if (GUI.Button (new Rect ((Screen.width/2) - 50, (Screen.height/2) - 100, 100, 25), "Level 1")) { 
			
			Application.LoadLevel ("Demo"); 
			
		} // end if 

		// button that closes Level Select Window 
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 30, 100, 25), "Close")) {
			
			levelMenu = false; 
			
		} // end if 


	} // end ShowLevel 
	
	// Update is called once per frame
	void Update () {
	
	} // end Update 

} // end LevelSelectButton 
