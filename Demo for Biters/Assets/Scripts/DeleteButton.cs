using UnityEngine;
using System.Collections;

public class DeleteButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	} // end Start
	
	public void OnClick() {
		
		// PlayerPrefs.DeleteAll (); 
		int temp = Game.current.id; 
		Game.current = new Game ();
		Game.current.id = temp; 
		Save.SaveThis (); 
		Application.LoadLevel ("MainMenu"); 
		
	} // end OnClick

} // end DeleteButton
