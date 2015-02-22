using UnityEngine;
using System.Collections;

public class NewGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	} // end Start 
	
	// Update is called once per frame
	void Update () {
	
	} // end Update 

	void OnGUI () { 

		// instructions 
		GUI.Box (new Rect ((Screen.width / 2) - 50, (Screen.height / 2) - 100, 150, 25), "Enter a Username");
		
		// enter name 
		Game.current.player.name = GUI.TextField (new Rect((Screen.width/2) - 50, (Screen.height/2) - 60, 150, 25), Game.current.player.name, 25);
		
		// save button 
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 100, 25), "Play")) { 
			
			// save current game as a new save file 
			Save.SaveThis (); 
			
			// enter game 
			Application.LoadLevel("PlayerMenu"); 
			
		} // end if statement  
		
		// button that closes new game Window 
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 30, 100, 25), "Cancel")) {
			
			Application.LoadLevel ("MainMenu"); 
			
		} // end if 

	} // end OnGUI 

} // end NewGame 
