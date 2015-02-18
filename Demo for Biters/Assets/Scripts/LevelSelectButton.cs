using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class LevelSelectButton : MonoBehaviour {

	private bool levelMenu = false;
	private Vector2 scrollPosition = Vector2.zero;
	private List<string> levelsList;
	PersistentScript persistentScript;
	// Use this for initialization
	public void OnClick () {

		levelMenu = true; 
	
	} // end OnClick 

	void OnGUI() { 

		if (levelMenu) { 
			
			GUI.Window(0, new Rect(0, 0, Screen.width, Screen.height), ShowLevel, "Level Select Menu");
			
		} // end if statement 

	} // end OnGUI

	void ShowLevel (int windowID) { 

		// level 1
		GetLevels();

		scrollPosition = GUI.BeginScrollView(new Rect(Math.Min(Screen.width-50,40), Math.Min(Screen.height,20),
		                                              Screen.width-40, Screen.height-20), 
		                                     		scrollPosition, new Rect(0, 0, Screen.width-90, levelsList.Count * 70));
			int i = 0;
			foreach(string levelName in levelsList)
			{
				if(GUI.Button(new Rect(0, i*70, Screen.width-90, 60),levelName))
				{
					persistentScript.SelectedLevel = levelName; 
					Application.LoadLevel ("Demo"); 
				}
				i++;
			}
		GUI.EndScrollView();


		if (GUI.Button (new Rect ((Screen.width/2) - 50, (Screen.height/2) - 100, 100, 25), "Level 1")) { 
			
			Application.LoadLevel ("Demo"); 
			
		} // end if 

		// button that closes Level Select Window 
		if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 30, 100, 25), "Close")) {
			
			levelMenu = false; 
			
		} // end if 


	} // end ShowLevel 

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

} // end LevelSelectButton 
