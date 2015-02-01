using UnityEngine;
using System.Collections;

public class ContinueButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	} // end Start
	
	// Called when button is clicked
	public void OnClick () {
	
		if (PlayerPrefs.HasKey ("SavedLevel")) {

				// there is a saved level, so load it
				Application.LoadLevel (PlayerPrefs.GetString ("SavedLevel"));

		} else {

				// no saved level, act as if start was pressed instead 
				Application.LoadLevel ("mini-game");

		} // end if else statement 

	} // end OnClick 

} // end ContinueButton
