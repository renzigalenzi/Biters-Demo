using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Time.timeScale = 1; 
		AudioListener.volume = 5; 
	}
	
	// Update is called once per frame
	public void OnClick () {
		Save.LoadThis ();
		Game.current = Save.save1; 
		//Game.current.id = 1; 
		Game.current.player.currLevel = Game.current.player.highestLevel; 
		Save.SaveThis ();
		if (Game.current.player.currLevel == "World - 0/Level - 01.csv") { 

			Application.LoadLevel ("Clip1"); 

		} else if (Game.current.player.currLevel == "World - 1/Level - 01.csv") { 

			Application.LoadLevel ("Clip2");

		} else {

			Application.LoadLevel ("Demo");

		} // end if else statement  
	
	} // end OnClick 
} // end StartButton 
