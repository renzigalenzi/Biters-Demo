using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	private bool pauseMenu = false; 
	private bool isPause = false; 
	public GUISkin window;
	static string level; 
	private float timer = 60.0f; 

	// Use this for initialization
	void OnGUI () {

		GUI.skin = window; 

		// button that pauses the game and brings up pause menu
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 30, 100, 25), "Pause")) {

			isPause = true; 
			pauseMenu = true; 

		} // end if 

		if (pauseMenu) { 

			GUI.Window(0, new Rect(0, 0, Screen.width, Screen.height), ShowPause, "Pause Menu");

		} // end if statement 
	
	} // end OnGUI 

	// content of Pause Menu 
	void ShowPause(int windowID) { 

		// reloads the level, thereby restarting it 
		if (GUI.Button (new Rect ((Screen.width/2) - 50, (Screen.height/2) - 100, 100, 25), "Reset Level")) { 
			UnPauseGame (); 
			Application.LoadLevel (Application.loadedLevelName); 


		} // end if 

		// turns sound on and off 
		if (GUI.Button (new Rect ((Screen.width/2) - 50, (Screen.height/2) - 70, 100, 25), "Sound")) { 
			
			if (AudioListener.volume != 0) {

				AudioListener.volume = 0;

			} else {

				AudioListener.volume = 50;

			} // end if else 
			
		} // end if  

		// goes back to main menu, lose game progress in the process 
		if (GUI.Button (new Rect ((Screen.width/2) - 50, (Screen.height/2) - 40, 100, 25), "Player Menu")) { 

			UnPauseGame (); 
			Application.LoadLevel ("PlayerMenu"); 
			
		} // end if 

		// quit the application 
		if (GUI.Button (new Rect ((Screen.width/2) - 50, (Screen.height/2) - 10, 100, 25), "Save and Quit")) { 

			UnPauseGame (); 
			level = Application.loadedLevelName; 
			PlayerPrefs.SetString("SavedLevel",level);
			// Application.Quit (); 
			Application.LoadLevel ("PlayerMenu"); 

		} // end if 

		if (GUI.Button (new Rect ((Screen.width/2) - 50, (Screen.height/2) + 50, 100, 25), "Continue")) { 
			UnPauseGame (); 
			pauseMenu = false; 
			isPause = false; 
			
		} // end if 

	} // end ShowPause 
	
	// Update is called once per frame
	void Update () {

		timer -= Time.deltaTime; 
		
		if (timer <= 0) { 

			timer = 0.0f; 
			
		} // end if  

		// pause the game 
		if (isPause || timer <= 0) { 

			PauseGame(); 

		} else { 

			//UnPauseGame(); 

		} // end if else 
	
	} // end Update 

	void PauseGame() { 

		Time.timeScale = 0.000001f; 

	} // end PauseGame  

	void UnPauseGame() { 

		Time.timeScale = 1.0f; 

	} // end UnPauseGame 

} // end class Pause 
