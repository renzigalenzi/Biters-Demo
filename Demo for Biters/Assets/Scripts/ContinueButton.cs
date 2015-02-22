using UnityEngine;
using System.Collections;

public class ContinueButton : MonoBehaviour {
	
	// Use this for initialization
	void Start () {

		Time.timeScale = 1; 
		AudioListener.volume = 10; 
	
	} // end Start
	
	// Called when button is clicked
	public void OnClick () {
	
		/*
		if (PlayerPrefs.HasKey ("SavedLevel")) {

				// there is a saved level, so load it
				Application.LoadLevel (PlayerPrefs.GetString ("SavedLevel"));

		} else {

				// no saved level, act as if start was pressed instead 
				Application.LoadLevel ("Demo");

		} // end if else statement
		*/ 

		Application.LoadLevel ("ContinueMenu"); 

	} // end OnClick 
	
} // end ContinueButton
