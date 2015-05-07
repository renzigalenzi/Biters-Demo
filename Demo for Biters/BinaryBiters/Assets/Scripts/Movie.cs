using UnityEngine;
using System.Collections;
using System.IO;

public class Movie : MonoBehaviour {

	private Texture2D[] slides;
	private FileInfo[] info; 
	private Texture currTex;
	// Original Path Extension: "/Users/laurenfuller/Documents/Biters-Demo/DemoBiters/Assets/Resources/Panel1"
	private string myPath;
	private string extention = "png";   
	public AudioSource sound1;
	public AudioSource sound2;
	public AudioSource sound3;
	public AudioSource sound4; 
	public AudioSource sound5; 
	public AudioSource sound6;
	public AudioSource sound7;
	public AudioSource sound8;
	public AudioSource sound9; 
	public AudioSource sound10;
	public AudioSource sound11;
	//private bool play = true; 
	private int counter = 1; 
	public int cutscene;        // make this "1" for cutscene 1, "2" for cutscene 2, or "3" for cutscene 3
	public GUISkin window;
	
	void Start() {

		// get correct pictures for cutscene 
		if (cutscene == 1) myPath = Directory.GetCurrentDirectory () + "/Assets/Resources/Clip1";
		if (cutscene == 2) myPath = Directory.GetCurrentDirectory () + "/Assets/Resources/Clip2";
		if (cutscene == 3) myPath = Directory.GetCurrentDirectory () + "/Assets/Resources/Clip3";

		// get files 
		Debug.Log ("Locating files, please standby.");
		getFiles(); 

		// start the clip 
		if (slides != null) {
			
			currTex = slides[0] as Texture;
			sound1.Play(); 

		} else {

			Debug.Log ("Error 404: File Not Found.");

		} // end if statement 

	} // end void Start() 
	
	void Update() {

		// update picture shown 
		if (slides != null)  {

			if (cutscene == 1 || cutscene == 2) { 

				// start audio files 
				if (!sound1.isPlaying && counter == 1) {

					sound2.Play(); 
					counter++; 
					currTex = slides[1] as Texture;

				} // end if statement 

				if (!sound2.isPlaying && counter == 2) {

					sound3.Play(); 
					counter++; 
					currTex = slides[2] as Texture;

				} // end if statement 

				if (!sound3.isPlaying && counter == 3) {

					sound4.Play(); 
					counter++; 
					currTex = slides[3] as Texture;

				} // end if statement 

				if (!sound4.isPlaying && counter == 4) {

					sound5.Play(); 
					counter++; 
					currTex = slides[4] as Texture;

				} // end if statement 

				if (!sound5.isPlaying && counter == 5) {
					
					sound6.Play(); 
					counter++; 
					currTex = slides[5] as Texture;
					
				} // end if statement 

				if (!sound6.isPlaying && counter == 6) {
					
					sound7.Play(); 
					counter++; 
					currTex = slides[6] as Texture;
					
				} // end if statement 

				if (!sound7.isPlaying && counter == 7) {
					
					sound8.Play(); 
					counter++; 
					currTex = slides[7] as Texture;
					
				} // end if statement 

				if (!sound8.isPlaying && counter == 8) {
					
					sound9.Play(); 
					counter++; 
					currTex = slides[8] as Texture;
					
				} // end if statement 

				if (!sound9.isPlaying && counter == 9) {
					
					Application.LoadLevel ("Demo");
					
				} // end if statement 

		    } else if (cutscene == 3) { 

				// start audio files 
				if (!sound1.isPlaying && counter == 1) {
					
					sound2.Play(); 
					counter++; 
					currTex = slides[1] as Texture;
					
				} // end if statement 
				
				if (!sound2.isPlaying && counter == 2) {
					
					sound3.Play(); 
					counter++; 
					currTex = slides[2] as Texture;
					
				} // end if statement 
				
				if (!sound3.isPlaying && counter == 3) {
					
					sound4.Play(); 
					counter++; 
					currTex = slides[3] as Texture;
					
				} // end if statement 
				
				if (!sound4.isPlaying && counter == 4) {
					
					sound5.Play(); 
					counter++; 
					currTex = slides[4] as Texture;
					
				} // end if statement 
				
				if (!sound5.isPlaying && counter == 5) {
					
					sound6.Play(); 
					counter++; 
					currTex = slides[5] as Texture;
					
				} // end if statement 
				
				if (!sound6.isPlaying && counter == 6) {
					
					sound7.Play(); 
					counter++; 
					currTex = slides[6] as Texture;
					
				} // end if statement 
				
				if (!sound7.isPlaying && counter == 7) {
					
					sound8.Play(); 
					counter++; 
					currTex = slides[7] as Texture;
					
				} // end if statement 
				
				if (!sound8.isPlaying && counter == 8) {
					
					sound9.Play(); 
					counter++; 
					currTex = slides[8] as Texture;
					
				} // end if statement 

				if (!sound9.isPlaying && counter == 9) {
					
					sound10.Play(); 
					counter++; 
					currTex = slides[9] as Texture;
					
				} // end if statement 

				if (!sound10.isPlaying && counter == 10) {
					
					sound11.Play(); 
					counter++; 
					currTex = slides[10] as Texture;
					
				} // end if statement 

				if (!sound11.isPlaying && counter == 11) {
					
					Application.LoadLevel ("PlayerMenu");
					
				} // end if statement 
	
			} // end if else statement for cutscene 

		} // end if statement for null 

	} // end Update()

	void OnGUI() {

		// draw picture 
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), currTex, ScaleMode.ScaleToFit, true, 2.0f);

		// Sarah adding skip button
		GUI.skin = window; 

		if (GUI.Button (new Rect ((Screen.width/2)+500, /*(Screen.height/2)-350*/650, 200, 50), "SKIP")) {
			Application.LoadLevel("Demo");
		}

	} // end OnGUI()
	
	void getFiles() {

		if (Directory.Exists(myPath)) {

			DirectoryInfo dir = new DirectoryInfo(myPath);
			Debug.Log("Looking for files in directory: " + myPath);
			
			info = dir.GetFiles("*." + extention);
			
			// Get number of files, and set the length for the texture2d array
			int totalFiles =  info.Length;
			slides = new Texture2D[totalFiles];

			int i = 0;
			
			//Read all found files
			foreach (FileInfo f in info) {

				string filePath = f.Directory + "/" + f.Name;
				Debug.Log("["+i+"] file found: "+filePath);
				
				var bytes = System.IO.File.ReadAllBytes(filePath);
				var tex = new Texture2D(1, 1);
				
				tex.LoadImage(bytes);
				slides[i] = tex;
				
				i++;

			} // end foreach statement 

		} else {

			Debug.Log ("Directory DOES NOT exist! ");

		} // end if else statement 

	} // end void getFiles()

} // end Movie.cs 