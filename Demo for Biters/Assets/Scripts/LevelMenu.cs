using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class LevelMenu : MonoBehaviour {

	private Vector2 scrollPosition = Vector2.zero;

	private static List<List<string>> levelsList;

	public int WorldCount = 0;
	// Use this for initialization
	public static List<List<string>> GetLevels()
	{
		if (levelsList == null)
		{
			levelsList = new List<List<string>> ();
			string dirName = Directory.GetCurrentDirectory () + "/Assets/Levels";
			DirectoryInfo dir = new DirectoryInfo(dirName);
			DirectoryInfo[] worlds = dir.GetDirectories();
			foreach(DirectoryInfo world in worlds)
			{
				List<string> temp = new List<string>();
				FileInfo[] info = world.GetFiles("*.csv");
				foreach (FileInfo f in info) 
				{
					temp.Add(world.Name + "/" + f.Name);
				}
				levelsList.Add(temp);
			}
		}
		return levelsList;
	}
	
	public void Start()
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	} // end Update 

	void OnGUI () { 

		// instructions 
		GUI.Box (new Rect ((Screen.width /2) - 50, 100, 150, 25), "Select a Level");
		// GetLevels();
		levelsList = Game.current.player.levelsList; 
		List<string> displayList;

		int xStart = Math.Min (Screen.width - 50, 40);
		int yStart = Math.Min (Screen.height, 200);
		int width = Screen.width - xStart - 40;
		int height = Screen.height - yStart - 20;
		if(Game.current.player.state == PlayerState.ChoosingWorld)
		{
			displayList = new List<string>();
			for(int it = 0; it < levelsList.Count; it++)
			{
				displayList.Add ("World " + it.ToString());
			}
		}
		else
		{
			displayList = levelsList[Game.current.player.world];
		}
		scrollPosition = GUI.BeginScrollView(new Rect(xStart, yStart, width, height-100), 
		                                     scrollPosition, new Rect(0, 0, (displayList.Count + 1) * 200, height-100));
		
		GUI.color = Color.yellow;
		if(Game.current.player.state == PlayerState.ChoosingWorld && GUI.Button(new Rect(0, 0, 190, height-100),"LevelCreator"))
		{
			Application.LoadLevel ("LevelBuilder"); 
		}
		else if(Game.current.player.state == PlayerState.ChoosingLevel && GUI.Button(new Rect(0, 0, 190, height-100),"Back"))
		{
			Game.current.player.state = PlayerState.ChoosingWorld;
			Application.LoadLevel ("LevelBuilder"); 
		}
		GUI.color = Color.white;
		string levelName;
		for(int i = 0; i < displayList.Count; i++)
		{
			levelName = displayList[i];
			if(GUI.Button(new Rect((i+1)*200, 0, 190, height-100),levelName))
			{
				if(Game.current.player.state == PlayerState.ChoosingWorld)
				{
					Game.current.player.world = i;
					Game.current.player.state = PlayerState.ChoosingLevel;
					Save.SaveThis ();
					Application.LoadLevel ("LevelSelect");
				}
				else
				{
					Game.current.player.currLevel = levelName; 
					Save.SaveThis ();
					Application.LoadLevel ("Demo");
				}
			}
		}
		GUI.EndScrollView();
		
		// button that closes Level Select Window 
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 30, 100, 25), "Close")) {
			
			Application.LoadLevel ("PlayerMenu"); 
			
		} // end if 


	} // end OnGUI 

} // end LevelMenu 
