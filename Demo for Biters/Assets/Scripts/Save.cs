using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary;
using System.IO; 

public static class Save {

	[SerializeField] 
	public static List<Game> games = new List<Game>(); 
	public static Game save1 = new Game(); 
	public static Game save2 = new Game(); 
	public static Game save3 = new Game(); 

	// call to save game 
	public static void SaveThis () { 

		// Save.games.Add (Game.current); 

		// Save to the correct game file 
		if (Game.current.id == 1) { 

			Save.save1 = Game.current; 

		} else if (Game.current.id == 2) { 

			Save.save2 = Game.current; 

		} else if (Game.current.id == 3) { 

			Save.save3 = Game.current; 

		} // end if else statement 

		// Store list of save files again to account for update 
		Save.games.Clear (); 
		Save.games.Add (Save.save1); 
		Save.games.Add (Save.save2); 
		Save.games.Add (Save.save3); 

		BinaryFormatter bf = new BinaryFormatter (); 
		FileStream file = File.Create (Application.persistentDataPath + "/saveFiles.dat");
		bf.Serialize (file, games); 
		file.Close (); 

	} // end SaveThis  

	public static void LoadThis () { 

		if (File.Exists (Application.persistentDataPath + "/saveFiles.dat")) { 

			BinaryFormatter bf = new BinaryFormatter(); 
			FileStream file = File.Open (Application.persistentDataPath + "/saveFiles.dat", FileMode.Open); 
			games = (List<Game>) bf.Deserialize (file); 
			file.Close (); 

			// restore game files to proper place 
			foreach (Game g in Save.games) { 

				if (g.id == 1) { 

					Save.save1 = g;

				} else if (g.id == 2) { 

					Save.save2 = g; 

				} else if (g.id == 3) { 

					Save.save3 = g; 

				} // end if statement 

			} // end for loop 

		} // end if statement  

	} // end LoadThis 
	
} // end Save 
