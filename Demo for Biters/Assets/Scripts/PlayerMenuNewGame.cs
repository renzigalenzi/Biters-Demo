using UnityEngine;
using System.Collections;

public class PlayerMenuNewGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void OnClick() {
		
		// Note: I believe this is a memory leak. 
		// PlayerPrefs.DeleteAll (); 
		int temp = Game.current.id; 
		Game.current = new Game ();
		Game.current.player.name = "Play";
		Game.current.id = 1; 
		Save.SaveThis (); 
		Application.LoadLevel ("Demo");
		
	} // end OnClick
}
