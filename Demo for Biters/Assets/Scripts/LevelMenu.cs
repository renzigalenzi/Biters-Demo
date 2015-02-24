using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class LevelMenu : MonoBehaviour {

	private Vector2 scrollPosition = Vector2.zero;
	private List<string> levelsList;
	PersistentScript persistentScript;


	// Use this for initialization
	void GetLevels()
	{
		if (levelsList != null)
			levelsList.Clear ();
		levelsList = new List<string> ();
		string dirName = Directory.GetCurrentDirectory () + "/Assets/Levels";
		DirectoryInfo dir = new DirectoryInfo(dirName);
		FileInfo[] info = dir.GetFiles("*.csv");
		foreach (FileInfo f in info) 
		{ 
			levelsList.Add(f.Name);
		}
	}
	
	public void Start()
	{
		GameObject persistentGameObject = GameObject.Find("PersistentData");
		persistentScript = (PersistentScript)persistentGameObject.GetComponent(typeof(PersistentScript));
	}
	
	// Update is called once per frame
	void Update () {
	
	} // end Update 

	void OnGUI () { 

		// instructions 
		GUI.Box (new Rect ((Screen.width /2) - 50, 100, 150, 25), "Select a Level");

		GetLevels();
		int xStart = Math.Min (Screen.width - 50, 40);
		int yStart = Math.Min (Screen.height, 200);
		int width = Screen.width - xStart - 40;
		int height = Screen.height - yStart - 20;
		
		scrollPosition = GUI.BeginScrollView(new Rect(xStart, yStart, width, height-100), 
		                                     scrollPosition, new Rect(0, 0, (levelsList.Count + 1) * 200, height-100));
		
		GUI.color = Color.yellow;
		if(GUI.Button(new Rect(0, 0, 190, height-100),"LevelCreator"))
		{
			Application.LoadLevel ("LevelBuilder"); 
		}
		int i = 1;
		GUI.color = Color.white;
		foreach(string levelName in levelsList)
		{
			if(GUI.Button(new Rect(i*200, 0, 190, height-100),levelName))
			{
				Game.current.player.currLevel = levelName; 
				Application.LoadLevel ("Demo"); 
			}
			i++;
		}
		GUI.EndScrollView();
		
		// button that closes Level Select Window 
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 30, 100, 25), "Close")) {
			
			Application.LoadLevel ("PlayerMenu"); 
			
		} // end if 


	} // end OnGUI 

} // end LevelMenu 
