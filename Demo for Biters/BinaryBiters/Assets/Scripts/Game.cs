using UnityEngine;
using System.Collections;

[System.Serializable] 
public class Game {

	public static Game current = new Game(); 
	public Player player; 
	public int id;          // stores which save file this instance of Game belongs to 

	public Game () { 

		player = new Player (); 
		id = 0; 

	} // end constructor  


} // end class Game
