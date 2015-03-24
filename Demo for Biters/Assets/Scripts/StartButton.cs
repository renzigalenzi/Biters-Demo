using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Time.timeScale = 1; 
		AudioListener.volume = 10; 
	}
	
	// Update is called once per frame
	public void OnClick () {
		Save.LoadThis ();
		Game.current = Save.save1; 
		//Game.current.id = 1; 
		Game.current.player.currLevel = Game.current.player.highestLevel; 
		Save.SaveThis ();
		Application.LoadLevel ("Demo");
	
	}
}
