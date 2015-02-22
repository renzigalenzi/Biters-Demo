using UnityEngine;
using System.Collections;

[System.Serializable] 
public class Player {

	public string name; 
	public int score; 
	public string currLevel;    // file that should be loaded upon continue 
	public int unlockedLevel;   // keeps track of current unlocked levels for level select menu 

	// only call this when making a new player 
	public Player () { 

		this.name = "New Game";
		this.score = 0; 
		this.unlockedLevel = 1; 
		this.currLevel = "Level - 1.csv";

	} // end constructor  

} // end class Player
