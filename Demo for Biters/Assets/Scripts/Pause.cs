using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	private bool pauseMenu = false; 
	private bool isPause = false; 
	public GUISkin window;
	public AudioSource sound;
	static string level;  

	// Use this for initialization
	void OnGUI () {

		GUI.skin = window; 

		// button that pauses the game and brings up pause menu
		if (GUI.Button (new Rect ((Screen.width/2)-700, /*(Screen.height/2)-350*/50, 200, 50), "Menu")) {

			isPause = true; 
			pauseMenu = true; 
			sound.Play (); 

		} // end if 

		if (pauseMenu) { 

			GUI.Window(0, new Rect(0, 0, Screen.width, Screen.height), ShowPause, "Game Menu");

		} // end if statement 
	
	} // end OnGUI 

	// content of Pause Menu 
	void ShowPause(int windowID) { 

		// reloads the level, thereby restarting it 
		if (GUI.Button (new Rect ((Screen.width/2) - 100, (Screen.height/2) - 90, 200, 50), "Restart")) { 
			UnPauseGame (); 
			sound.Play (); 
			Application.LoadLevel (Application.loadedLevelName); 


		} // end if 

		// turns sound on and off 
		if (GUI.Button (new Rect ((Screen.width/2) - 100, (Screen.height/2) + 30, 200, 50), "Sound")) { 

			sound.Play (); 
			
			if (AudioListener.volume != 0) {

				AudioListener.volume = 0;

			} else {

				AudioListener.volume = 15;

			} // end if else 
			
		} // end if  

		// goes back to main menu, lose game progress in the process 
		if (GUI.Button (new Rect ((Screen.width/2) - 100, (Screen.height/2) - 30, 200, 50), "Main Menu")) { 

			UnPauseGame (); 
			sound.Play (); 
			Application.LoadLevel ("PlayerMenu"); 
			
		} // end if 

		// quit the application 
		if (GUI.Button (new Rect ((Screen.width/2) - 100, (Screen.height/2) + 90, 200, 50), "Save and Quit")) { 
		
			UnPauseGame (); 
			sound.Play (); 
			level = Application.loadedLevelName; 
			PlayerPrefs.SetString("SavedLevel",level);
			// Application.Quit (); 
			Application.LoadLevel ("PlayerMenu"); 

		} // end if 

		if (GUI.Button (new Rect ((Screen.width/2) - 100, (Screen.height/2) - 150, 200, 50), "Return")) { 
			UnPauseGame (); 
			pauseMenu = false; 
			isPause = false; 
			sound.Play (); 
			
		} // end if 

	} // end ShowPause 
	
	// Update is called once per frame
	void Update () {

		// pause the game 
		if (isPause) { 

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
