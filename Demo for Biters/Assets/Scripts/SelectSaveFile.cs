using UnityEngine;
using System.Collections;

public class SelectSaveFile : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	} // end Start
	
	// Update is called once per frame
	void Update () {
	
	} // end Update

	void OnGUI () { 

		// instructions 
		GUI.Box (new Rect ((Screen.width / 2) - 50, (Screen.height / 2) - 100, 150, 25), "Select Save File");
		
		// load file 
		Save.LoadThis (); 
		
		// get save files 
		if (GUI.Button (new Rect ((Screen.width / 2) - 100, (Screen.height / 2) - 60, 250, 100), Save.save1.player.name)) { 
			
			Game.current = Save.save1; 
			if (Game.current.player.name == "New Game") { 
				
				Game.current.id = 1; 
				Application.LoadLevel ("NewGame"); 
				
			} else { 
				
				Application.LoadLevel ("PlayerMenu"); 
				
			} // end if else statement 
			
		} // end if statement  
		
		if (GUI.Button (new Rect ((Screen.width / 2) - 100, (Screen.height / 2) + 45, 250, 100), Save.save2.player.name)) { 
			
			Game.current = Save.save2; 
			if (Game.current.player.name == "New Game") { 
				
				Game.current.id = 2; 
				Application.LoadLevel ("NewGame"); 
				
			} else { 
				
				Application.LoadLevel ("PlayerMenu"); 
				
			} // end if else statement 
			
		} // end if statement  
		
		if (GUI.Button (new Rect ((Screen.width / 2) - 100, (Screen.height / 2) + 150, 250, 100), Save.save3.player.name)) { 
			
			Game.current = Save.save3; 
			if (Game.current.player.name == "New Game") { 
				
				Game.current.id = 3; 
				Application.LoadLevel ("NewGame"); 
				
			} else { 
				
				Application.LoadLevel ("PlayerMenu"); 
				
			} // end if else statement 
			
		} // end if statement  
		
		/* 
		foreach (Game g in Save.games) { 

			if (GUI.Button (new Rect ((Screen.width / 2) - 50, (Screen.height / 2) - height, 150, 25), g.player.name)) { 
				
				Game.current = g;
				Application.LoadLevel ("Demo"); 
				
			} // end if statement  

			height = height - 30; 

		} // end for loop 
		*/ 
		
		// button that closes new game Window 
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 30, 100, 25), "Cancel")) {
			
			Application.LoadLevel ("MainMenu"); 
			
		} // end if 

	} // end OnGUI 

} // end SelectSaveFile
