using UnityEngine;
using System.Collections;

public class LevelSelectButton : MonoBehaviour {
	
	// Use this for initialization
	public void OnClick () {
		Save.LoadThis ();
		Game.current = Save.save1; 
		//Game.current.id = 1; 
		Game.current.player.currLevel = Game.current.player.highestLevel; 
		Save.SaveThis ();
		Game.current.player.state = PlayerState.ChoosingWorld;
		Application.LoadLevel ("LevelSelect"); 
	
	} // end OnClick 
	
	// Update is called once per frame
	void Update () {
	
	} // end Update 

} // end LevelSelectButton 
