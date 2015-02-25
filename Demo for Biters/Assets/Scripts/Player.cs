using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[System.Serializable] 
public class Player {

	public string name; 
	public int score; 
	public string currLevel;          // file that should be loaded upon continue 
	public List<string> levelsList;   // keeps track of current unlocked levels for level select menu 
	public string highestLevel;       // highest level reached 

	// only call this when making a new player 
	public Player () { 

		this.name = "New Game";
		this.score = 0; 
		this.levelsList = new List<string> ();
		this.currLevel = "Level - 01.csv";
		this.highestLevel = "Level - 01.csv"; 
		this.levelsList.Add (currLevel); 

	} // end constructor  

} // end class Player
