using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[System.Serializable] 

public enum PlayerState
{
	ChoosingWorld, ChoosingLevel
}
[System.Serializable] 
public class Player {

	public string name; 
	public int score; 
	public string currLevel;          // file that should be loaded upon continue 
	public List<List<string>> levelsList;   // keeps track of current unlocked levels for level select menu 
	public string highestLevel;       // highest level reached 
	public PlayerState state;
	public int world;
	public List<string> temp;


	// only call this when making a new player 
	public Player () { 

		this.name = "New Game";
		this.score = 0; 
		this.levelsList = new List<List<string>> ();
		this.currLevel = "World - 6/Level - 07.csv";
		this.highestLevel = "World - 6/Level - 07.csv"; 
		this.world = 6;
		this.temp = new List<string> ();
		this.temp.Add(currLevel);
		this.state = PlayerState.ChoosingWorld;
		this.levelsList.Add (temp); 

	} // end constructor  

} // end class Player
